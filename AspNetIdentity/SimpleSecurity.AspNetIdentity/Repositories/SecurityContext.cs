using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using SimpleSecurity.AspNetIdentity.Entities;
using SimpleSecurity.AspNetIdentity.Repositories.ModelConfiguration;

namespace SimpleSecurity.AspNetIdentity.Repositories
{
    public class SecurityContext : IdentityDbContext<ApplicationUser>
    {
        public SecurityContext()
            : base("SimpleSecurityConnection", throwIfV1Schema: false)
        {
        }

        //public DbSet<ApplicationUser> ApplicationUser { get { return this.Set<ApplicationUser>(); } }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<OperationsToRoles> OperationsToRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new ResourceConfiguration());
            modelBuilder.Configurations.Add(new OperationsToRolesConfiguration());
        }
    }

}
