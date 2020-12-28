using DataBrowser.Domain.Entities.TransatableItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBrowser.Entities.SQLite.Configuration
{
    public class TransatableItemValueConfig : IEntityTypeConfiguration<TransatableItemValue>
    {
        public void Configure(EntityTypeBuilder<TransatableItemValue> entity)
        {
            entity.HasKey(p => new {p.TransatableItemFk, p.Language});

            entity.Ignore(b => b.DomainEvents);

            //TransatableItemValue
            entity.HasOne(a => a.TransatableItem).WithMany(b => b.TransatableItemValues)
                .HasForeignKey(c => c.TransatableItemFk).OnDelete(DeleteBehavior.Cascade);
        }
    }
}