using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using projectman.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using projectman.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using projectman.Util;
using CSHelper.Extensions;

namespace projectman.Areas.Man.Controllers
{
    [Area("man")]
    [Authorize( AuthenticationSchemes = ManDefaults.AuthenticationScheme)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _user;

        public HomeController(ILogger<HomeController> logger, IUserRepository user)
        {
            _logger = logger;
            _user = user;
        }

        public IActionResult Index()
        {
            return View();
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
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        // login
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromServices] ILDAPClient ldapClient, [FromServices]ITransaction tran, string username, string password, string returnUrl = null)
        {
            ViewBag.returnUrl = returnUrl;

            if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
            {
                ViewBag.error = "必須輸入帳號與密碼";
                return View("Login");
            }

            var user = await _user.Get(username);

            string error = null;
            bool overridePassword = false;

            if (ldapClient.LoadOptions("Admin"))
            {
                var r = ldapClient.VerifyUserAndPassword(username, password);
                if (r == null)
                {
                    error = "LDAP設定無法使用";
                }
                else if (r.Error != null)
                {
                    error = $"無法登入 (LDAP - {r.Error.ToString()})";
                }
                else if (!r.LoggedIn)
                {
                    error = "無法登入 (帳號/密碼錯誤)";
                }
                else if (!r.IsMember)
                {
                    error = "無法登入 (使用者不在使用群組內)";
                }
                else
                {
                    overridePassword = true;

                    if (r.AutoCreate)
                    {


                        // ensure account exists
                        if (user == null)
                        {
                            var m = new User()
                            {
                                enabled = true,
                                username = r.UserName
                            };
                            m.SetProperties(r.MapFieldValues);

                            user = await _user.Create(m);
                            await tran.Commit();
                        } else
                        {
                            // update user's details
                            user.username = r.UserName;
                            user.SetProperties(r.MapFieldValues);

                            user = _user.Update(user);
                            await tran.Commit();
                        }
                    }
                }

            }

            try
            {

                if (user == null)
                {
                    throw new ErrorCodeException(ErrorCode.NOT_FOUND_USER);
                }

                if (!user.enabled)
                    throw new ErrorCodeException(ErrorCode.USER_DISABLED);

                if (!overridePassword &&
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
                    new Claim(ClaimTypes.AuthenticationMethod, ManDefaults.AuthenticationScheme )
                };

                foreach (Enum up in typeof(UserPermission).GetEnumValues())
                {
                    if (user.perm.HasFlag(up))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, up.ToString()));
                    }
                }

                var claimsIdentity = new ClaimsIdentity(claims, ManDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                   ManDefaults.AuthenticationScheme,
                   new ClaimsPrincipal(claimsIdentity),
                   new AuthenticationProperties
                   {
                       IsPersistent = true     // stay logged in
                   });

                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);                 
                }
                else
                {
                    return RedirectToAction("Dashboard", "Project");
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
            await HttpContext.SignOutAsync(ManDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }


    }
}
