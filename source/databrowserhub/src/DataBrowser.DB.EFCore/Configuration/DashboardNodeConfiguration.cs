using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Interfaces.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBrowser.DB.EFCore.Configuration
{
    public class DashboardNodeConfiguration : IEntityTypeConfiguration<DashboardNode>
    {
        public void Configure(EntityTypeBuilder<DashboardNode> entity)
        {
            entity.ToTable(TableNames.DashboardNodes);

            entity.HasKey(dvt => new {dvt.DashboardId, dvt.NodeId});

            entity.Property(e => e.Weight).HasDefaultValue(0);

            entity.HasOne(x => x.Dashboard)
                .WithMany(x => x.Nodes)
                .HasForeignKey(x => x.DashboardId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}