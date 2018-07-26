using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SimpleSecurity.AspNetIdentity.RefApp.Models;
using SimpleSecurity;
using SimpleSecurity.AspNetIdentity.Filters;
using SimpleSecurity.AspNetIdentity.Entities;
using System.Threading;
using System.Security.Claims;
using System.Web.Configuration;
using System.Security;
using Microsoft.Owin;

namespace SimpleSecurity.AspNetIdentity.RefApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        [Authorize(Roles = "admin,user")]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            //This code demonstrates how to get information on the current user to display.
            //Commented this out to show we can get the email from the current principal.
            //var user = WebSecurity.GetCurrentUser();
            //ViewBag.Email = user.Email;
            //Show how to get roles from claims principal
            var prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var roles = prinicpal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            var email = prinicpal.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
            ViewBag.Roles = roles;
            ViewBag.Email = email;
            return View();
        }

        [SimpleAuthorize(AppResources.Foo, Operations.Read) ]
        public ActionResult ReadFoo()
        {
            ViewBag.Message = "Read Foo";

            return View();
        }

        [SimpleAuthorize(AppResources.Foo, Operations.Create)]
        public ActionResult WriteFoo()
        {
            ViewBag.Message = "Write Foo";

            return View();
        }

        //Demonstrates how to use ClaimsAuthorizationManager to manage 
        //authorization down in the class libraries
        public ActionResult UseClaimsAuthorization()
        {
            try
            {
                FakeService.MyMethod();
            }
            catch (SecurityException ex)
            {
                Response.Redirect(loginUrl(Url.Action("UseClaimsAuthorization", "Home")));
            }
            ViewBag.Message = "Test Claims Authorization";
            return View();
        }

        public string loginUrl(string returnUrl)
        {
             return "../Account/Login?ReturnUrl=" + HttpUtility.UrlEncode(returnUrl);
        }

    }
}
