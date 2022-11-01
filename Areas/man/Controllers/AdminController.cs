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
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace repairman.Areas.Man.Controllers
{

    [Area("man")]
    [Authorize(AuthenticationSchemes = ManDefaults.AuthenticationScheme)]
    public class AdminController : BaseController
    {
        private readonly IUserRepository _user;

        public AdminController(ITransaction tran, ILookupRepository lookup, IUserRepository user) :
            base(tran, lookup)
        {
            _user = user;
        }

        // main user list view
        [AuthorizeRole(UserPermission.ManageUser)]
        public IActionResult Index()
        {
            return View();
        }

        // new user page
        [AuthorizeRole(UserPermission.ManageUser)]
        public IActionResult New()
        {
            ViewData["groups"] = GetGroupList();
            return View(new User());
        }

        // new user - save
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("New")]
        [AuthorizeRole(UserPermission.ManageUser)]
        public async Task<IActionResult> NewPost(List<long> groups)
        {
            User user = new User();

            if (await TryUpdateModelAsync<User>(
                user,
                "",
                a => a.name,
                a => a.enabled,
                a => a.username,
                a => a.UnencryptedPassword,
                a => a.desc,
                a => a.isSalePerson,
                a => a.perm
            ))
            {
                if (groups != null)
                {
                    user.groups = new List<UserGroup>();
                    groups.ForEach(m => user.groups.Add(new UserGroup { group_id = m }));
                }

                user = await _user.Create(user);
            }

            return await CommitModel(user);
        }

        // view user
        [AuthorizeRole(UserPermission.ManageUser)]
        public async Task<IActionResult> View(long ID)
        {
            User user = await _user.Get(ID, "groups");

            if( user==null )
            {
                return NotFound();
            }

            ViewData["groups"] = GetGroupList(user.groups.Select(m => m.group_id).ToList());

            return View(user);
        }

        // edit user
        [HttpPost]
        [ActionName("View")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(UserPermission.ManageUser)]
        public async Task<IActionResult> Edit(long ID, [FromForm] List<long> groups )
        {
            bool isCurrUser = IsIDOfCurrentUser(ID);

            User user = await _user.Get(ID, "groups");

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
                a => a.isSalePerson,
                a => a.perm
            ))
            {
                if (isCurrUser)
                {   // current user can't disable himself, nor remove manager permission
                    user.enabled = true;
                    user.perm |= UserPermission.ManageUser;
                }

                if( user.enabled && resetPasswordCount )
                {
                    user.bad_password_count = 0;
                }

                if (groups != null)
                {
                    var toDelete = user.groups.Where(m => !groups.Contains(m.group_id)).ToList();
                    var toAdd = groups.Where(m => !user.groups.Select(m => m.group_id).Contains(m)).ToList();

                    foreach (var t in toDelete)
                    {
                        user.groups.Remove(t);
                    }

                    foreach (var u in toAdd)
                    {
                        user.groups.Add(new UserGroup { group_id = u });
                    }

                    groups.ForEach(m => user.groups.Add(new UserGroup { group_id = m }));
                }
                else
                {
                    user.groups = new List<UserGroup>();
                }

            }

            return await CommitModel(user);
        }

        // groups

        public async Task<IActionResult> Group()
        {
            return View(await _user.GetGroups().ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Group")]
        [AuthorizeRole(UserPermission.ManageUser)]
        public async Task<IActionResult> GroupPost()
        {
            await this.TryUpdateTableModelAsync<Group, long>("Group",
                async t => await _user.Create(t) != null,
                t => Task.FromResult(_user.DelGroupUnsafe(t)),
                async t => await _user.GetGroup(t)
            );

            var result = await CommitModel(null);

            return result;
        }

        protected MultiSelectList GetGroupList(IEnumerable<long> selectedValues = null)
        {
            return new MultiSelectList(_user.GetGroups().Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.name }), "Value", "Text", selectedValues);
        }

        // setting page
        public IActionResult Setting()
        {
            return View();
        }

        // change user's password
        public IActionResult ChangePassword(long ID)
        {
            if (ID == 0)
                ID = GetCurrentUserID();
            bool isCurrUser = IsIDOfCurrentUser(ID);

            ViewData["ID"] = ID;
            ViewData["IsCurrUser"] = isCurrUser;
            return PartialView("_ChangePasswordPopup");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(long ID, [FromForm] string oldpass, [FromForm] string pass )
        {
            bool isCurrUser = IsIDOfCurrentUser(ID);

            User user = await _user.Get(ID);

            if (user == null)
            {
                return NotFound();
            }

            if( isCurrUser )
            {
                if ( string.IsNullOrEmpty(oldpass) || !user.VerifyPassword(oldpass))
                    return Json( new { r = 1, m = "舊密碼錯誤" } );
            }
            else if( !UserHasPermission(UserPermission.ManageUser) )
            {
                return Json(new { r = 1, m = "沒有權限改密碼" });
            }

            JsonResult result = ValidateNewPassword(pass);
            if (result != null)
                return result;

            user.UnencryptedPassword = pass;

            ViewData["ID"] = ID;
            ViewData["IsCurrUser"] = isCurrUser;

            await _tran.Commit();

            return Ok();
        }

        // API: change user status
        [Route("[area]/[controller]/{ID}/enable/{status}")]
        [HttpPost]
        [AuthorizeRole(UserPermission.ManageUser)]
        public async Task<IActionResult> EnableAccount(long ID, int status )
        {
            if (status != 1 && IsIDOfCurrentUser(ID))
                return BadRequest();

            User user = await _user.Get(ID);

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

        // API: change user status
        [Route("[area]/[controller]/{ID}/change/{isSale}")]
        [HttpPost]
//        [AuthorizeRole(UserPermission.ManageUser)]
        public async Task<IActionResult> ChangeIsSale(long ID, int isSale)
        {

            User user = await _user.Get(ID);

            if (user == null)
            {
                return NotFound();
            }

            user.isSalePerson = (isSale == 1) ? true : false;

            await _tran.Commit();

            return Ok();
        }

        // API: query user list
        [AuthorizeRole(UserPermission.ManageUser)]
        public async Task<IActionResult> Query(QueryVM request, UserPermission? perm = null)
        {
            var result = _user.Find(request.search, perm);

            return await GetTableReplyAsync(result, request, null, r => new
            {
                ID = r.ID,
                name = r.name,
                username = r.username,
                desc = r.desc,
                enabled = r.enabled,
                perm = r.perm.GetDisplayName(),
                isSale = r.isSalePerson
            });
        }

    }
}
