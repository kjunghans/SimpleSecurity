using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleSecurity.Entities;
using SimpleSecurity.Repositories.ModelConfiguration;

namespace SimpleSecurity.Repositories
{
    public class SecurityContext : DbContext
    {
        public SecurityContext()
            : base("SimpleSecurityConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<OperationsToRoles> OperationsToRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ResourceConfiguration());
            modelBuilder.Configurations.Add(new OperationsToRolesConfiguration());
        }
    }

}
