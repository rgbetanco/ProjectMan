using Microsoft.Extensions.Configuration;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace projectman.Util
{
    public class LDAPClient : ILDAPClient
    {
        private readonly IConfiguration _cfg;
        LDAPOptions _options;

        public class LDAPOptions
        {
            public string Server { get; set; }                      // ldap.chinsoft.com
            public short Port { get; set; } = 389;                  // 389
            public string UserDN { get; set; }                      // cn=user,dc=example,dc=com or example\user
            public string Password { get; set; }                    // password
            public string BaseDN { get; set; }                      // dc=example,dc=com
            public bool AutoCreateAccount { get; set; } = true;
            public string RequiredGroup { get; set; }
            public string GroupField { get; set; } = "MemberOf";

            public string UserNameField { get; set; } = "SamAccountName";   // name of username field in LDAP
            public Dictionary<string, string> MapFields { get; set; }       // fields to map to DB from LDAP
                                                                            // This is an array of [database field name] = [ldap field name]
        }


        public LDAPClient(IConfiguration configuration)
        {
            _cfg = configuration;
        }

        public bool LoadOptions(string section)
        {
            _options = _cfg
                .GetSection("LDAP")
                .GetSection(section)
                .Get<LDAPOptions>();

            return _options != null;
        }

        static string LdapEscape(string ldap)
        {
            if (ldap == null) return "";
            return ldap
                .Replace("\\", "\\5C")
                .Replace("*", "\\2A")
                .Replace("(", "\\28")
                .Replace(")", "\\29")
                .Replace("\000", "\\00");
        }

        public ILDAPClient.VerificationResults VerifyUserAndPassword(string username, string password)
        {
            if (_options == null)
                return null;

            var vr = new ILDAPClient.VerificationResults
            {
                AutoCreate = _options.AutoCreateAccount
            };

            try
            {
                using (var connection = new LdapConnection())
                {
                    var constraints = connection.SearchConstraints;
                    constraints.ReferralFollowing = true;
                    connection.Constraints = constraints;

                    connection.Connect(_options.Server, _options.Port);
                    connection.Bind(_options.UserDN, _options.Password);

                    string[] fields = new[] {
                        _options.GroupField,
                        _options.UserNameField
                    };

                    if (_options.MapFields.Count > 0)
                    {
                        fields = fields.Concat(_options.MapFields.Values.ToArray()).ToArray();
                    }

                    var escapedUsername = LdapEscape(username);
                    var searchFilter = $"(&(ObjectClass=user)({_options.UserNameField}={escapedUsername}))";
                    var entities = connection.Search(_options.BaseDN, LdapConnection.ScopeSub, searchFilter, fields, false);

                    LdapEntry foundEntity = null;
                    while (entities.HasMore())
                    {
                        var entity = entities.Next();
                        try
                        {
                            var account = entity.GetAttribute(_options.UserNameField);
                            if (account != null && account.StringValue == username)
                            {
                                foundEntity = entity;
                                vr.UserName = username;

                                if(_options.MapFields!=null )
                                {
                                    vr.MapFieldValues = new();
                                    foreach (var kv in _options.MapFields)
                                    {
                                        try
                                        {
                                            var v = entity.GetAttribute(kv.Value).StringValue;
                                            var k = kv.Key;
                                            vr.MapFieldValues.Add(k, v);
                                        }
                                        catch ( KeyNotFoundException )
                                        {

                                        }

                                    }
                                }

                                break;
                            }
                        }
                        catch (LdapException e)
                        {
                            vr.Error = new Exception($"Unable to retrieve DN for user.", e);
                            return vr;
                        }

                    }

                    if( foundEntity!=null )
                    {
                        string userDn = foundEntity.Dn;

                        if (!string.IsNullOrWhiteSpace(userDn))
                        {
                            try
                            {
                                connection.Bind(userDn, password);
                                vr.LoggedIn = connection.Bound;
                            } catch (LdapException e)
                            {
                                if( e.ResultCode==49 )
                                {
                                    vr.Error = null;
                                    return vr;
                                }

                                // unable to login with user/password
                                vr.Error = new Exception($"Unable to validate user.", e);
                                return vr;
                            }
                        }

                        if( vr.LoggedIn )
                        {
                            // check membership if needed
                            if (!string.IsNullOrWhiteSpace(_options.RequiredGroup))
                            {
                                // resolve required group
                                var escapedGroupName = LdapEscape(_options.RequiredGroup);
                                var searchGroupFilter = $"(&(ObjectClass=group)(|({_options.UserNameField}={escapedGroupName})(DistinguishedName={escapedGroupName})))";
                                var groupEntities = connection.Search(_options.BaseDN, LdapConnection.ScopeSub, searchGroupFilter, new[] {
                                    "DistinguishedName"
                                }, false);

                                string groupDN = null;
                                while (groupEntities.HasMore())
                                {
                                    var entity = groupEntities.Next();
                                    try
                                    {
                                        var account = entity.GetAttribute("DistinguishedName").StringValue;
                                        if (!string.IsNullOrWhiteSpace(account) )
                                        {
                                            groupDN = account;
                                            break;
                                        }
                                    }
                                    catch(KeyNotFoundException)
                                    {

                                    }
                                    catch (LdapException e)
                                    {
                                        vr.Error = new Exception($"Unable to retrieve DN for required group.", e);
                                        return vr;
                                    }
                                }

                                try
                                {
                                    var result = foundEntity.GetAttribute(_options.GroupField).StringValueArray;

                                    foreach (var dn in result)
                                    {
                                        if (dn == groupDN)
                                        {
                                            vr.IsMember = true;
                                            break;
                                        }
                                    }
                                }
                                catch (KeyNotFoundException)
                                {

                                }

                            } else
                            {
                                vr.IsMember = true;
                            }
                        }

                    }
                }
            }
            catch (LdapException e)
            {
                vr.Error = new Exception($"LDAP Connection failed.", e);
            }

            return vr;
        }

    }
}
