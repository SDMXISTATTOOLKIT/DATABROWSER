using DataBrowser.Domain.Entities.Users;
using DataBrowser.Domain.Interfaces.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBrowser.DB.EFCore.Configuration
{
    public class ApplicationRoleClaimConfig : IEntityTypeConfiguration<ApplicationRoleClaim>
    {
        public void Configure(EntityTypeBuilder<ApplicationRoleClaim> entity)
        {
            // still use default table name
            entity.ToTable(TableNames.RoleClaims);

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            entity.Property(p => p.RoleId)
                .HasColumnName(nameof(ApplicationRoleClaim.RoleId))
                .HasColumnType("int");

            entity.HasOne(p => p.Role)
                .WithMany(r => r.RoleClaims)
                .HasForeignKey(p => p.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}