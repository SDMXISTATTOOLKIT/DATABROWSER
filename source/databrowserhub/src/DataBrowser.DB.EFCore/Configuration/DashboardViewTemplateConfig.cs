using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Interfaces.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBrowser.DB.EFCore.Configuration
{
    public class DashboardViewTemplateConfig : IEntityTypeConfiguration<DashboardViewTemplate>
    {
        public void Configure(EntityTypeBuilder<DashboardViewTemplate> entity)
        {
            entity.ToTable(TableNames.DashboardViewTemplates);

            entity.HasKey(dvt => new {dvt.DashboardId, dvt.ViewTemplateId});

            entity.Property(e => e.Weight).HasDefaultValue(0);

            entity.HasOne(x => x.Dashboard)
                .WithMany(x => x.Views)
                .HasForeignKey(x => x.DashboardId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}