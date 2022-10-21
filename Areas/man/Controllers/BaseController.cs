using CSHelper.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using repairman.Models;
using repairman.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using CSHelper.Extensions;
using CSHelper.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;
using System.Diagnostics;

namespace repairman.Areas.Man.Controllers
{
    public static class ManDefaults {
        public const string AuthenticationScheme = "backstage"; // CookieAuthenticationDefaults.AuthenticationScheme;
    }

    public abstract class BaseController : Controller
    {

        protected readonly ITransaction _tran;
        protected readonly ILookupRepository _lookup;
        public BaseController(ITransaction tran, ILookupRepository lookup)
        {
            _tran = tran;
            _lookup = lookup;
        }

        protected bool IsIDOfCurrentUser(long ID)
        {
            bool isCurrUser = GetCurrentUserID() == ID;

            return isCurrUser;
        }

        protected long GetCurrentUserID()
        {
            long val = -1;
            long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out val);
            return val;
        }

        protected string GetCurrentUserGroup()
        {
            var result = User.FindFirst(ClaimTypes.PrimaryGroupSid);
            var userGroup = (result != null) ? (result.Value != null ? result.Value : "") : "";
            return userGroup;
        }

        protected bool UserHasPermission(UserPermission perm)
        {
            return User.HasPermission(perm);
        }

        protected bool UserIsInGroup(string group)
        {
            if (group == null)
                group = "";

            return group.StartsWith(GetCurrentUserGroup());
        }

        protected JsonResult ValidateNewPassword(string pass)
        {

            if (string.IsNullOrWhiteSpace(pass))
            {
                return Json(new { r = 1, m = "必填新密碼" });
            }

            if (pass.Length < 8)
            {
                return Json(new { r = 1, m = "新密碼必須8字或以上" });
            }

            return null;
        }

        // helper functions

        protected async Task<IActionResult> CommitModel(UsesID source, List<string> pendingFiles = null )
        {
            if( ModelState.IsValid )
            {
                try
                {

                    await _tran.Commit();

                    return Ok();
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("=", "存檔失敗: " + e.InnerException.Message);
                    _tran.ResetCommit();
                }
            }

            this.handlePendingFiles(pendingFiles);

            return Json(new
            {
                r = 1,
                m = ModelState.Values.SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage)
            });
        }


        protected async Task<JsonResult> GetTableReplyAsync<T,TResult>( IQueryable<T> result, QueryVM request, IDictionary<string, string> fieldMap = null, Expression<Func<T, TResult>> newExp = null, bool listBeforeSelect = false)
        {
            result = request.Apply(result, fieldMap);

            if( newExp!=null )
            {
                if (listBeforeSelect)
                {
                    var fnc = newExp.Compile();
                    var listOut = (await result.ToListAsync()).Select(fnc).ToList();
                    return Json(listOut);
                }
                else
                {
                    var listOut = await result.Select(newExp).ToListAsync();
                    return Json(listOut);
                }
            } else
            {
                var listOut = await result.ToListAsync();
                return Json(listOut);
            }

        }

        protected JsonResult TableReply( int total, object list )
        {
            var type = typeof(TableReplyVM<>).MakeGenericType(list.GetType().GetGenericArguments()[0]);
            var reply = Activator.CreateInstance(type, new object[] {
                total,total, list
            });

            return Json(reply);
        }

        protected async Task<bool> TryUpdateModelListAsync<TModel, TProp>(TModel dest, Expression<Func<TModel, IList<TProp>>> fieldExp) where TProp : UsesID, new()
        {
            return await TryUpdateModelListAsync(dest, fieldExp, null);
        }

        protected async Task<bool> TryUpdateModelListAsync<TModel, TProp>(TModel dest, Expression<Func<TModel, IList<TProp>>> fieldExp, params Expression<Func<TProp, object>>[] includeExpressions) where TProp : UsesID, new()
        {
            var expr = (MemberExpression)fieldExp.Body;
            var prop = (PropertyInfo)expr.Member;
            var target = (IList<TProp>)prop.GetValue(dest);
            var name = prop.Name;
            

            if (target == null)
            {
                target = new List<TProp>();
                prop.SetValue(dest, target);
            }

            var listIDs = new List<UsesIDVM>();
            var listData = new List<TProp>();

            // 1. load list of .ID and .Delete to process from the value provider
            // 2. load list of properties from the value provider
            if (await TryUpdateModelAsync(listIDs, name))
            {
                await TryUpdateModelAsync(listData, name);

                int count = listIDs.Count;

                int targetLen = target.Count();
                Dictionary<long, int> mapIDtoIndex = new Dictionary<long, int>(targetLen);

                // create lookup table so we can quickly delete
                for (int i = 0; i < targetLen; i++)
                {
                    mapIDtoIndex[target[i].ID] = i;
                }

                List<PropertyInfo> propToCopy = null;

                if (includeExpressions != null)
                    // if a property is an object (e.g. string), then it is treated as a member expression;
                    // if it's a simple type like int, then it is an unary expression.
                    propToCopy = includeExpressions.Select(
                        p => (PropertyInfo)(
                            (p.Body is MemberExpression) ?
                                ((MemberExpression)p.Body) :
                                ((MemberExpression)((UnaryExpression)p.Body).Operand)
                           ).Member
                        ).ToList();
                else
                {
                    propToCopy = prop.GetType().GetProperties().Where(p => p.CanWrite && p.CanRead).ToList();
                }

                List<int> dstToRemove = new List<int>();

                for (int i = 0; i < count; i++)
                {
                    if (listIDs[i] != null)
                    {
                        // existing entry; see if we're deleting
                        if (listIDs[i].Deleted)
                        {
                            dstToRemove.Add(mapIDtoIndex[listIDs[i].ID.Value]);

                            // mark attributes in ModelState as valid
                            propToCopy.ForEach(p => {
                                string key = name + "[" + i + "]." + p.Name;
                                if (ModelState.ContainsKey(key))
                                {
                                    ModelState[key].ValidationState = ModelValidationState.Valid;
                                    ModelState[key].Errors.Clear();
                                }
                            });
                        }
                        else
                        {
                            // existing entry; see if we're deleting
                            if (!listIDs[i].Deleted)
                            {
                                var src = listData[i];
                                var dst = target[mapIDtoIndex[listIDs[i].ID.Value]];

                                propToCopy.ForEach(p => p.SetValue(dst, p.GetValue(src, null), null));
                            }
                        }
                    }
                    else
                    {
                        var src = listData[i];
                        var dst = src;

                        // new entry
                        if (includeExpressions != null)
                        {
                            dst = new TProp();

                            propToCopy.ForEach(p => p.SetValue(dst, p.GetValue(src, null), null));
                        }

                        target.Add(dst);
                    }
                }

                dstToRemove.Sort();

                for (int i = dstToRemove.Count() - 1; i >= 0; i--)
                {
                    target.RemoveAt(dstToRemove[i]);
                }

                return true;
            }

            return false;
        }


    }
}
