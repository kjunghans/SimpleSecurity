using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using SimpleSecurity.Entities;

namespace SimpleSecurity.Repositories.ModelConfiguration
{
    public class ResourceConfiguration : EntityTypeConfiguration<Resource>
    {
        internal ResourceConfiguration()
        {
            this.HasKey(e => e.Id);
            this.Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(e => e.Name).IsVariableLength().HasMaxLength(100).IsRequired();
        }
    }
}
