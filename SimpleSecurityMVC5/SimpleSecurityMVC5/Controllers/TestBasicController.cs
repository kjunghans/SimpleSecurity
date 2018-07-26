using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SimpleSecurityMVC5.Models;
using SimpleSecurity.Filters;
using SimpleSecurity.Entities;

namespace SimpleSecurityMVC5.Controllers
{
    public class TestBasicController : ApiController
    {
        [BasicAuthorize(AppResources.Foo, Operations.Read) ]
        public string Get()
        {
            return "Authorized & Authenticated to use API.";
        }
    }
}
