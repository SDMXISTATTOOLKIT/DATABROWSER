using DataBrowser.Domain.Entities.Versioning;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DataBrowser.DB.EFCore.Context
{
    public class DataBrowserUpdaterContext : DbContext
    {
        public DataBrowserUpdaterContext(DbContextOptions<DataBrowserUpdaterContext> options)
             : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<DataBrowserVersion> DataBrowserVersion { get; set; }
        public DbSet<DataBrowserVersionActionUpgrader> DataBrowserVersionActionUpgraders { get; set; }
    }
}
