using DotneterWhj.EntityModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.EntityFramework.EntityConfigs
{
    public class AppInfoConfig : EntityTypeConfiguration<AppInfo>
    {

        public AppInfoConfig()
        {
            this.ToTable("AppInfo")
                   .HasKey(e => e.Id);

            this.Property(p => p.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(e => e.AppId).IsUnicode().IsRequired().HasMaxLength(32);

            this.Property(e => e.AppSecret).IsUnicode().IsRequired().HasMaxLength(32);

        }

    }
}
