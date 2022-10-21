using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using repairman.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CSHelper.Extensions;
using repairman.Repositories;
using CSHelper.Authorization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace repairman.Areas.Man.Controllers
{

    [Area("man")]
    [Authorize(AuthenticationSchemes = ManDefaults.AuthenticationScheme)]
    public class MemberController : BaseController
    {
        private readonly IMemberRepository _user;
        private readonly IServiceRequestRepository _sr;

        public MemberController(ITransaction tran, ILookupRepository lookup, IMemberRepository user, IServiceRequestRepository sr) :
            base(tran, lookup)
        {
            _user = user;
            _sr = sr;
        }

        // main user list view
        [AuthorizeRole(UserPermission.ManageMember)]
        public IActionResult Index()
        {
            return View();
        }

        // new user page
        [AuthorizeRole(UserPermission.ManageMember)]
        public IActionResult New()
        {
            ViewData["dept"] = GetDeptList();

            return View(new Member());
        }

        // new user - save
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("New")]
        [AuthorizeRole(UserPermission.ManageMember)]
        public async Task<IActionResult> NewPost()
        {
            var user = new Member();

            if (await TryUpdateModelAsync(
                user,
                "",
                a => a.name,
                a => a.enabled,
                a => a.username,
                a => a.UnencryptedPassword,
                a => a.desc,
                a => a.dept_id
            ))
            {
                user = await _user.Create(user);
            }

            return await CommitModel(user);
        }

        // view user
        [AuthorizeRole(UserPermission.ManageMember)]
        public async Task<IActionResult> View(long ID)
        {
            ViewData["dept"] = GetDeptList();

            var user = await _user.GetMember(ID);

            if( user==null )
            {
                return NotFound();
            }

            return View(user);
        }

        // edit user
        [HttpPost]
        [ActionName("View")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(UserPermission.ManageMember)]
        public async Task<IActionResult> Edit(long ID )
        {
            var user = await _user.GetMember(ID);

            if (user == null)
            {
                return NotFound();
            }

            bool resetPasswordCount = !user.enabled;

            if (await TryUpdateModelAsync(
                user,
                "",
                a => a.name,
                a => a.enabled,
                a => a.username,
                a => a.UnencryptedPassword,
                a => a.desc,
                a => a.dept_id
            ))
            {
                if( user.enabled && resetPasswordCount )
                {
                    user.bad_password_count = 0;
                }
            }

            return await CommitModel(user);
        }

        // setting page
        public IActionResult Setting()
        {
            return View();
        }

        // change user's password
        public IActionResult ChangePassword(long ID)
        {
            ViewData["ID"] = ID;
            return PartialView("_ChangePasswordPopup");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(long ID, [FromForm] string pass )
        {
            var user = await _user.GetMember(ID);

            if (user == null)
            {
                return NotFound();
            }

            if( !UserHasPermission(UserPermission.ManageMember) )
            {
                return Json(new { r = 1, m = "沒有權限改密碼" });
            }

            JsonResult result = ValidateNewPassword(pass);
            if (result != null)
                return result;

            user.UnencryptedPassword = pass;

            ViewData["ID"] = ID;

            await _tran.Commit();

            return Ok();
        }

        // API: change user status
        [Route("[area]/[controller]/{ID}/enable/{status}")]
        [HttpPost]
        [AuthorizeRole(UserPermission.ManageMember)]
        public async Task<IActionResult> EnableAccount(long ID, int status )
        {
            var user = await _user.GetMember(ID);

            if (user == null)
            {
                return NotFound();
            }

            user.enabled = (status==1) ? true : false;

            if( user.enabled )
            {   // reset bad password count
                user.bad_password_count = 0;
            }

            await _tran.Commit();

            return Ok();
        }

        // API: query user list
        [AuthorizeRole(UserPermission.ManageMember)]
        public async Task<IActionResult> Query(QueryVM request)
        {
            var result = _user.FindMembers(request.search);

            return await GetTableReplyAsync(result, request, null, r => new
            {
                ID = r.ID,
                name = r.name,
                username = r.username,
                desc = r.desc,
                enabled = r.enabled
            });
        }
        protected IEnumerable<SelectListItem> GetDeptList()
        {
            var r = _sr.GetDepts()
                .AsNoTracking();

            var result = r.Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.name });

            return result;
        }

    }
}
