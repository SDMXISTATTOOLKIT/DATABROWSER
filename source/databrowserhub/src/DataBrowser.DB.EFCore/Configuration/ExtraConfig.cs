using DataBrowser.Domain.Entities.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBrowser.Entities.SQLite.Configuration
{
    public class ExtraConfig : IEntityTypeConfiguration<Extra>
    {
        public void Configure(EntityTypeBuilder<Extra> entity)
        {
            entity.HasKey(p => p.ExtraId);

            entity.Ignore(b => b.DomainEvents);

            entity.HasOne(b => b.TransatableItem).WithOne().HasForeignKey<Extra>(c => c.TransatableItemFk)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}