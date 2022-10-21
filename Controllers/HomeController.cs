using repairman.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using repairman.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using SixLabors.ImageSharp;
using CSHelper.Extensions;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using repairman.Util;
using CSHelper.Models;
using CSHelper.Extensions.File;
using repairman.ModelOptions;

namespace repairman.Controllers
{
    public static class PublicDefaults
    {
        public const string AuthenticationScheme = "public"; // CookieAuthenticationDefaults.AuthenticationScheme;
    }

    [Authorize(AuthenticationSchemes = PublicDefaults.AuthenticationScheme)]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMemberRepository _member;
        private readonly IServiceRequestRepository _sr;
        private readonly ITransaction _tran;
        private readonly ILookupRepository _lookup;

        public HomeController(ILogger<HomeController> logger, ITransaction tran, ILookupRepository lookup, IMemberRepository member, IServiceRequestRepository sr)
        {
            _logger = logger;
            _member = member;
            _sr = sr;
            _tran = tran;
            _lookup = lookup;
        }

        public async Task<IActionResult> List()
        {
            var r = _sr.Find()
                .Where(r => r.member_id == GetCurrentUserID())
                .Include(r => r.dept)
                .Include(r => r.sub_cat)
                .Include(r => r.sub_cat.cat)
                .OrderByDescending(r => r.modify_date);

            return View(await r.ToListAsync());
        }

        // API: list categories
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ListSubCatsForType(long? type)
        {
            return Json(GetSubCatList(type));
        }

        protected IEnumerable<SelectListItem> GetCatList()
        {
            var r = _sr.FindCats()
                .AsNoTracking();

            var result = r.Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.name });

            return result;
        }

        protected IEnumerable<SelectListItem> GetSubCatList(long? type)
        {
            var r = _sr.GetSubCats(type)
                .AsNoTracking();

            var result = r.Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.name });

            return result;
        }
        protected IEnumerable<SelectListItem> GetDeptList()
        {
            var r = _sr.GetDepts()
                .AsNoTracking();

            var result = r.Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.name });

            return result;
        }

        public async Task<IActionResult> Index()
        {
            var l = GetCatList();
            ViewData["cat"] = l;
            ViewData["subcat"] = GetSubCatList(long.Parse(l.First().Value));
            ViewData["dept"] = GetDeptList();

            var m = await _member.GetMember(GetCurrentUserID());

            var a = new ServiceRequest();
            a.date = DateTime.UtcNow;
            if (m != null)
            {
                a.email = m.email;
                a.name = m.name;
                a.phone = m.phone;
                if( m.dept_id.HasValue )
                    a.dept_id = m.dept_id.Value;
            }

            return View(a);
        }

        protected long GetCurrentUserID()
        {
            long val = -1;
            long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out val);
            return val;
        }


        protected async Task<IActionResult> CommitModel(UsesID source, List<string> pendingFiles = null)
        {
            if (ModelState.IsValid)
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

        // new message - save
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("New")]
        public async Task<IActionResult> NewPost([FromServices] IWebHostEnvironment env)
        {
            var m = new ServiceRequest();

            await this.TryUpdateModelListAsync(m, a => a.files, b => b.file);
            await this.TryUpdateModelListAsync(m, a => a.pics, b => b.file);
            await TryUpdateModelAsync<ServiceRequest>(
                m,
                "",
                a => a.desc,
                a => a.sub_cat_id,
                a => a.dept_id,
                a => a.name,
                a => a.email,
                a => a.phone
            );

            m.status = ServiceRequestStatus.Pending;
            m.member_id = GetCurrentUserID();

            List<string> paths = null;
            try
            {
                this.processUploadedFiles(m, m => m.files, new FileModelOptions()
                {
                    StoragePath = Path.Join(env.WebRootPath, "dl", "f", "p")
                }, ref paths);

                this.processUploadedFiles(m, m => m.pics, new PicFileModelOptions()
                {
                    StoragePath = Path.Join(env.ContentRootPath, "AppData", "uploads", "request"),
                    OutputThumbnailPath = Path.Join(env.WebRootPath, "dl", "img", "a"),
                    OutputFullImagePath = Path.Join(env.WebRootPath, "dl", "img", "a")
                }, ref paths);

                m = await _sr.Create(m);
            }
            catch (SaveProcessUploadError)
            {
                ModelState.AddModelError("file", "無法儲存上傳的檔案");
            }
            catch (ImageProcessUploadError)
            {
                ModelState.AddModelError("file", "無法儲存縮圖檔");
            }
            catch (MissingProcessUploadError)
            {
                ModelState.AddModelError("file", "上傳檔案必填");
            }
            catch (ProcessUploadError)
            {
                ModelState.AddModelError("file", "無法讀取上傳的圖檔");
            }

            var result = await CommitModel(m, paths);

            return result;
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorVM { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // access denied
        [AllowAnonymous]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        // login view
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return RedirectToAction("Login", "Home", new { area = "Man" });
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        // login
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromServices] ILDAPClient ldapClient, string username, string password, string returnUrl = null)
        {
            ViewBag.returnUrl = returnUrl;

            if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
            {
                ViewBag.error = "必須輸入帳號與密碼";
                return View("Login");
            }

            var user = await _member.GetMember(username);

            string error = null;
            bool overridePassword = false;

            if ( ldapClient.LoadOptions("Member") )
            {
                var r = ldapClient.VerifyUserAndPassword(username, password);
                if (r==null)
                {
                    error = "LDAP設定無法使用";
                } else if( r.Error!=null ) {
                    error = $"無法登入 (LDAP - {r.Error.ToString()})";
                } else if( !r.LoggedIn )
                {
                    error = $"無法登入 (帳號/密碼錯誤)";
                } else if( !r.IsMember )
                {
                    error = $"無法登入 (使用者不在使用群組內)";
                } else
                {
                    overridePassword = true;

                    if (r.AutoCreate)
                    {
                        // ensure account exists
                        if (user == null)
                        {
                            var m = new Member()
                            {
                                enabled = true,
                                username = r.UserName
                            };

                            m.SetProperties(r.MapFieldValues);

                            var n = r.MapFieldValues["$name"];
                            if (!string.IsNullOrWhiteSpace(n))
                            {
                                var n2 = n.Split('-');
                                if (n2.Length >= 2)
                                {
                                    user.name = n2[1];

                                    var dept = await _sr.GetDepts()
                                        .Where(r => r.name == n2[0])
                                        .FirstOrDefaultAsync();

                                    if (dept != null)
                                    {
                                        user.dept_id = dept.ID;
                                    }
                                }
                            }

                            user = await _member.Create(m);
                            await _tran.Commit();
                        } else
                        {
                            // update user's details
                            user.username = r.UserName;
                            user.SetProperties(r.MapFieldValues);

                            var n = r.MapFieldValues["$name"];
                            if( !string.IsNullOrWhiteSpace(n))
                            {
                                var n2 = n.Split('-');
                                if( n2.Length>=2)
                                {
                                    user.name = n2[1];

                                    var dept = await _sr.GetDepts()
                                        .Where(r => r.name == n2[0])
                                        .FirstOrDefaultAsync();

                                    if( dept!=null )
                                    {
                                        user.dept_id = dept.ID;
                                    }
                                }
                            }

                            user = _member.Update(user);
                            await _tran.Commit();
                        }
                    }
                }

            }

            if( error!=null )
            {
                ViewBag.error = error;

                return View("Login");
            }

            try
            {

                if (user == null)
                {
                    throw new ErrorCodeException(ErrorCode.NOT_FOUND_USER);
                }

                if (!user.enabled)
                    throw new ErrorCodeException(ErrorCode.USER_DISABLED);

                if (
                    !overridePassword &&
                    !user.VerifyPassword(password))
                {
                    user.bad_password_count++;
                    if (user.bad_password_count == 5)
                        user.enabled = false;

                    throw new ErrorCodeException(ErrorCode.USER_PASSWORD_MISMATCH);
                }

                user.bad_password_count = 0;

                // ok
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.username),
                    new Claim(ClaimTypes.NameIdentifier, user.ID.ToString() ),
                    new Claim(ClaimTypes.AuthenticationMethod, PublicDefaults.AuthenticationScheme )
                };

                var claimsIdentity = new ClaimsIdentity(claims, PublicDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                   PublicDefaults.AuthenticationScheme,
                   new ClaimsPrincipal(claimsIdentity),
                   new AuthenticationProperties
                   {
                       IsPersistent = true
                   });

                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

            }
            catch (ErrorCodeException e)
            {
                ViewBag.returnUrl = returnUrl;

                ViewBag.error = e.GetDisplayName();
                return View("Login");
            }
        }

        // logout
        [HttpGet]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(PublicDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

    }
}
