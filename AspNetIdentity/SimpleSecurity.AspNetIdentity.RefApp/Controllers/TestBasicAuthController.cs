using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleSecurity.AspNetIdentity.RefApp.Controllers
{
    public class TestBasicAuthController : Controller
    {
        //
        // GET: /TestBasicAuth/

        public ActionResult Index()
        {
            return View();
        }

    }
}
