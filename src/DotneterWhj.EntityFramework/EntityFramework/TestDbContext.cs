namespace DotneterWhj.EntityFramework.EntityFramework
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using DotneterWhj.EntityModel;
    using System.Diagnostics;
    using DotneterWhj.ToolKits;
    using Microsoft.Extensions.Logging;

    public partial class TestDbContext : DbContext
    {
        public TestDbContext(ICustomLogger<TestDbContext> logger)
            : base("name=connection")
        {
            this.Database.Log = sql =>
            {
                Debug.WriteLine(sql);
                logger.Info(sql);
            };
        }

        public virtual DbSet<AppInfo> AppInfos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
        }


    }
}
