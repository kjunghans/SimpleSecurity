namespace SimpleSecurity.AspNetIdentity.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using SimpleSecurity.AspNetIdentity.Entities;

    internal sealed class Configuration : DbMigrationsConfiguration<SimpleSecurity.AspNetIdentity.Repositories.SecurityContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "SimpleSecurity.AspNetIdentity.Repositories.SecurityContext";
        }

        protected override void Seed(SimpleSecurity.AspNetIdentity.Repositories.SecurityContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            var webSecurity = new WebSecurity();
            if (!webSecurity.FoundUser("admin"))
                webSecurity.CreateUserAndAccount("admin", "password", "admin@gmail.com");
            if (!webSecurity.FoundUser("user"))
                webSecurity.CreateUserAndAccount("user", "password", "user@gmail.com");
            if (!RoleService.RoleExists("admin"))
            {
                RoleService.AddRole("admin");
                webSecurity.MapUserToRole(webSecurity.GetUserId("admin"), "admin");
            }
            if (!RoleService.RoleExists("user"))
            {
                RoleService.AddRole("user");
                webSecurity.MapUserToRole(webSecurity.GetUserId("user"), "user");
            }

            int AppResourcesFoo = 1;

            if (!ResourceService.FoundResource(AppResourcesFoo))
            {
                //Build the operations available for this resource
                Operations operations = Operations.Read;
                operations |= Operations.Create;
                //Add the resource
                ResourceService.AddResource(AppResourcesFoo, "Foo", operations);
                //Map the resource/operation to the roles
                ResourceService.MapOperationToRole(AppResourcesFoo, Operations.Read, "user");
                ResourceService.MapOperationToRole(AppResourcesFoo, Operations.Create, "admin");
            }

        }
    }
}
