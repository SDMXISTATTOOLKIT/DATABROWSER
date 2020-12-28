using DataBrowser.Domain.Entities.Users;
using DataBrowser.Domain.Interfaces.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBrowser.Entities.SQLite.Configuration
{
    public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> entity)
        {
            entity.ToTable(TableNames.Users);

            entity.HasKey(x => x.Id);

            entity.HasMany(ne => ne.RefreshTokens).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}