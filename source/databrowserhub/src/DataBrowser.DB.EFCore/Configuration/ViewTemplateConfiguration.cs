using DataBrowser.Domain.Entities.ViewTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBrowser.DB.EFCore.Configuration
{
    public class ViewTemplateConfiguration : IEntityTypeConfiguration<ViewTemplate>
    {
        public void Configure(EntityTypeBuilder<ViewTemplate> entity)
        {
            entity.HasKey(p => p.ViewTemplateId);

            entity.HasOne(b => b.Title).WithOne().HasForeignKey<ViewTemplate>(c => c.TitleFK)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(b => b.DecimalSeparator).WithOne().HasForeignKey<ViewTemplate>(c => c.DecimalSeparatorFk)
                .OnDelete(DeleteBehavior.SetNull);

            //entity.HasOne(b => b.User).WithOne().HasForeignKey<ViewTemplate>(i=> i.UserFK).OnDelete(DeleteBehavior.Cascade);

            //entity.HasMany(ne => ne.DashboardViewTemplates).WithOne();

            entity.Ignore(b => b.DomainEvents);
        }
    }
}