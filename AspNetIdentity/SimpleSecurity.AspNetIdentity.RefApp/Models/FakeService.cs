using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel.Services;
using SimpleSecurity.AspNetIdentity.Entities;


namespace SimpleSecurity.AspNetIdentity.RefApp.Models
{
    public class FakeService
    {
        public static void MyMethod()
        {
            //Here is where we would check access to perform operations in this method.
            ClaimsPrincipalPermission.CheckAccess(AppResources.Foo.ToString(), Operations.Read.ToString());
            //If we made it this far we were authorized and we would perform the actual operations here.
        }
    }
}