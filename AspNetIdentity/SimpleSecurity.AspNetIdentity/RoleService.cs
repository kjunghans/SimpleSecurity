using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SimpleSecurity.AspNetIdentity.Repositories;

namespace SimpleSecurity.AspNetIdentity
{
    public class RoleService
    {
        private static RoleManager<IdentityRole> newRoleManager
        {
            get { return new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new SecurityContext())); }
        }
        public static void AddRole(string roleName)
        {
            using (var roleManager = newRoleManager)
            {
                var roleresult = roleManager.Create(new IdentityRole(roleName));
            }
        }

        public static bool RoleExists(string roleName)
        {
            using (var roleManager = newRoleManager)
            {
                return roleManager.RoleExists<IdentityRole, string>(roleName);
            }
        }

 
    }
}
