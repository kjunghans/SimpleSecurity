using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SimpleSecurity.Entities;

namespace SimpleSecurity.Filters
{
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true,
    AllowMultiple = true)]
    public class SimpleAuthorize : AuthorizeAttribute
    {
        public SimpleAuthorize(int ResourceId, Operations operation)
        {
            _resourceId = ResourceId;
            _operation = operation;
        }

        private int _resourceId;
        private Operations _operation;

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //Get the current claims principal
            var prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            //Make sure they are authenticated
            if (!prinicpal.Identity.IsAuthenticated)
                return false;
            //Get the roles from the claims
            var roles = prinicpal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            //Check if they are authorized
            return ResourceService.Authorize(_resourceId, _operation, roles);
        }


    }
}
