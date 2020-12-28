using DataBrowser.Domain.Entities.TransatableItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBrowser.Entities.SQLite.Configuration
{
    public class TransatableItemConfig : IEntityTypeConfiguration<TransatableItem>
    {
        public void Configure(EntityTypeBuilder<TransatableItem> entity)
        {
            entity.HasKey(p => p.TransatableItemId);

            entity.Ignore(b => b.DomainEvents);
        }
    }
}