using DataBrowser.Domain.Entities.Users;
using DataBrowser.Domain.Interfaces.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBrowser.DB.EFCore.Configuration
{
    public class ApplicationUserClaimConfig : IEntityTypeConfiguration<ApplicationUserClaim>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserClaim> builder)
        {
            builder.ToTable(TableNames.UserClaims);

            builder.Property(p => p.UserId)
                .HasColumnName(nameof(ApplicationUserClaim.UserId))
                .HasColumnType("int");

            builder.HasOne(p => p.User)
                .WithMany(u => u.UserClaims)
                .HasForeignKey(p => p.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}