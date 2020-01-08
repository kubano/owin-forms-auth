using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace owin_demo.Controllers
{
    public class LoginViewModel
    {
        [EmailAddress]
        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginViewModel vm)
        {
            var userid = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, vm.EmailAddress),
                new Claim(ClaimTypes.Sid, userid ),
                new Claim(ClaimTypes.Role, "User"),
            };

            if (userid.Contains("0"))
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            var ctx = HttpContext.GetOwinContext();
            var authenticationManager = ctx.Authentication;

            authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = vm.RememberMe }, identity);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logoff()
        {
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut();

            return RedirectToAction(nameof(Login));
        }
    }
}