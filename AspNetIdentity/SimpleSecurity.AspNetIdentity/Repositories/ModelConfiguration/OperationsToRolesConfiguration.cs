using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using SimpleSecurity.AspNetIdentity.Entities;

namespace SimpleSecurity.AspNetIdentity.Repositories.ModelConfiguration
{
    public class OperationsToRolesConfiguration : EntityTypeConfiguration<OperationsToRoles>
    {
        internal OperationsToRolesConfiguration()
        {
            this.HasKey(e => new { e.ResourceId, e.RoleName });
            this.Property(e => e.RoleName).IsVariableLength().HasMaxLength(256).IsRequired();
         }
    }
}
