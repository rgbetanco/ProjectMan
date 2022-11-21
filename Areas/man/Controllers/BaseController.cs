using CSHelper.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using projectman.Models;
using projectman.Repositories;
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

namespace projectman.Areas.Man.Controllers
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


    }
}
