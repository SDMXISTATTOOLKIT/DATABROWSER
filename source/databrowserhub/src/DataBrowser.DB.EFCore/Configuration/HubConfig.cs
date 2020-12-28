using DataBrowser.Domain.Entities.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBrowser.Entities.SQLite.Configuration
{
    public class HubConfig : IEntityTypeConfiguration<Hub>
    {
        public void Configure(EntityTypeBuilder<Hub> entity)
        {
            entity.HasKey(p => p.HubId);

            entity.HasOne(b => b.Title).WithOne().HasForeignKey<Hub>(c => c.TitleFk).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(b => b.Slogan).WithOne().HasForeignKey<Hub>(c => c.SloganFk).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(b => b.Description).WithOne().HasForeignKey<Hub>(c => c.DescriptionFk)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(b => b.Disclaimer).WithOne().HasForeignKey<Hub>(c => c.DisclaimerFk)
                .OnDelete(DeleteBehavior.SetNull);

            entity.Ignore(b => b.DomainEvents);
        }
    }
}