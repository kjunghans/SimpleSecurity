using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Security;
using SeedSimple.Models;
using SimpleSecurity;
using SimpleSecurity.Entities;
using SimpleSecurity.Repositories;

namespace SeedSimple
{
    //public class InitSecurityDb : DropCreateDatabaseIfModelChanges<SecurityContext>
    public class DropCreateSecurityDb : DropCreateDatabaseAlways<SecurityContext>
    {
        protected override void Seed(SecurityContext context)
        {

            WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection("SimpleSecurityConnection",
               "UserProfile", "UserId", "UserName", autoCreateTables: true);
            var roles = (WebMatrix.WebData.SimpleRoleProvider)Roles.Provider;
            var membership = (WebMatrix.WebData.SimpleMembershipProvider)Membership.Provider;

            if (!roles.RoleExists("Admin"))
            {
                roles.CreateRole("Admin");
            }
            if (!roles.RoleExists("User"))
            {
                roles.CreateRole("User");
            }
            if (!roles.RoleExists("SuperUser"))
            {
                roles.CreateRole("SuperUser");
            }
            if (!WebSecurity.FoundUser("test"))
            {
                WebSecurity.CreateUserAndAccount("test", "password", "test@gmail.com");
            }
            if (!roles.GetRolesForUser("test").Contains("Admin"))
            {
                roles.AddUsersToRoles(new[] { "test" }, new[] { "admin" });
            }
            if (!WebSecurity.FoundUser("joe"))
            {
                WebSecurity.CreateUserAndAccount("joe", "password", "joe@gmail.com");
            }
            if (!roles.GetRolesForUser("joe").Contains("User"))
            {
                roles.AddUsersToRoles(new[] { "joe" }, new[] { "User" });
            }

            //Build the operations available for this resource
            Operations operations = Operations.Read;
            operations |= Operations.Create;
            //Add the resource
            ResourceService.AddResource(AppResources.Foo, "Foo", operations);
            //Map the resource/operation to the roles
            ResourceService.MapOperationToRole(AppResources.Foo, Operations.Read, "User");
            ResourceService.MapOperationToRole(AppResources.Foo, Operations.Create, "Admin");
  
        }
    }
}