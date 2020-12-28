using DataBrowser.Domain.Entities.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBrowser.Entities.SQLite.Configuration
{
    public class NodeConfig : IEntityTypeConfiguration<Node>
    {
        public void Configure(EntityTypeBuilder<Node> entity)
        {
            entity.HasKey(p => p.NodeId);
            entity.HasIndex(p => new {p.Code}).IsUnique();

            entity.HasMany(ne => ne.Extras).WithOne().OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(b => b.Title).WithOne().HasForeignKey<Node>(c => c.TitleFk).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(b => b.Slogan).WithOne().HasForeignKey<Node>(c => c.SloganFk)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(b => b.Description).WithOne().HasForeignKey<Node>(c => c.DescriptionFk)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(b => b.DecimalSeparator).WithOne().HasForeignKey<Node>(c => c.DecimalSeparatorFk)
                .OnDelete(DeleteBehavior.SetNull);

            entity.Ignore(b => b.DomainEvents);
        }
    }
}