using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Threading;
using SimpleSecurity.AspNetIdentity.Entities;

namespace SimpleSecurity.AspNetIdentity
{
    public class SimpleAuthorizationManager : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            string resourceStr = context.Resource.First().Value;
            string actionStr = context.Action.First().Value;
            int resourceId;
            Operations operationId;
            try { resourceId = Int32.Parse(resourceStr); }
            catch { throw new Exception("Invalid resource. Must be a string representation of an integer value."); }
            try { operationId = (Operations)Enum.Parse(typeof(Operations),actionStr); }
            catch { throw new Exception("Invalid action/operation. Must be a string representation of an integer value."); }

            //Get the current claims principal
            var prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            //Make sure they are authenticated
            if (!prinicpal.Identity.IsAuthenticated)
                return false;
            //Get the roles from the claims
            var roles = prinicpal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            //Check if they are authorized
            return ResourceService.Authorize(resourceId, operationId, roles);
        }

    }
}
