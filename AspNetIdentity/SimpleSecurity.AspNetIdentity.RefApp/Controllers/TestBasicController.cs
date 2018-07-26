using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SimpleSecurity.AspNetIdentity.RefApp.Models;
using SimpleSecurity.AspNetIdentity.Filters;
using SimpleSecurity.AspNetIdentity.Entities;

namespace SimpleSecurity.AspNetIdentity.RefApp.Controllers
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
