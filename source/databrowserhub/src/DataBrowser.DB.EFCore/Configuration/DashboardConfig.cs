using DataBrowser.Domain.Entities.DBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBrowser.DB.EFCore.Configuration
{
    public class DashboardConfig : IEntityTypeConfiguration<Dashboard>
    {
        public void Configure(EntityTypeBuilder<Dashboard> entity)
        {
            entity.HasKey(i => i.DashboardId);

            entity.HasMany(b => b.Views)
                .WithOne(u => u.Dashboard);

            entity.HasMany(b => b.Nodes)
                .WithOne(u => u.Dashboard);

            entity.Property(e => e.Weight).HasDefaultValue(0);

            entity.HasOne(b => b.Title).WithOne().HasForeignKey<Dashboard>(c => c.TitleFK)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}