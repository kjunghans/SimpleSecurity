using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SeedSimple.Models;
using SimpleSecurity.Filters;
using SimpleSecurity.Entities;

namespace SeedSimple.Controllers
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
