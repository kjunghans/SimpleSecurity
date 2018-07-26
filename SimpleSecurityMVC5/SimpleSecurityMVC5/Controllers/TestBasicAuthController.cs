using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleSecurityMVC5.Controllers
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
