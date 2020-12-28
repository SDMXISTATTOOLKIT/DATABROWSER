using DataBrowser.Domain.Entities.Users;
using DataBrowser.Domain.Interfaces.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataBrowser.DB.EFCore.Configuration
{
    public class UserAuditConfig : IEntityTypeConfiguration<UserAudit>
    {
        public void Configure(EntityTypeBuilder<UserAudit> entity)
        {
            entity.HasKey(x => x.UserAuditId);

            entity.Property(x => x.UserId).IsRequired();
            entity.Property(x => x.Timestamp).IsRequired();
            entity.Property(x => x.AuditEvent).IsRequired();

            entity.ToTable(TableNames.UsersAudit);
        }
    }
}