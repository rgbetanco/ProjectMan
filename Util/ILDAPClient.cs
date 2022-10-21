using System;
using System.Collections.Generic;

namespace repairman.Util
{
    public interface ILDAPClient
    {

        public class VerificationResults
        {
            public Exception Error { get; set; }

            public bool LoggedIn { get; set; } = false;
            public bool IsMember { get; set; } = false;
            public bool AutoCreate { get; set; } = false;

            public string UserName { get; set; }

            public Dictionary<string, string> MapFieldValues { get; set; }
        }


        bool LoadOptions(string section);
        VerificationResults VerifyUserAndPassword(string username, string password);
    }
}