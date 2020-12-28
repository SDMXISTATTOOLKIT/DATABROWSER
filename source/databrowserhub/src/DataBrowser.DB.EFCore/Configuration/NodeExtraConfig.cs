using DataBrowser.AC.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Entities.SQLite.Configuration
{
    public class NodeExtraConfig : IEntityTypeConfiguration<NodeExtra>
    {

        public void Configure(EntityTypeBuilder<NodeExtra> entity)
        {
            entity.HasKey(p => new { p.ExtraFk, p.NodeFk });

            //Node
            entity.HasOne(ne => ne.Node)
            .WithMany(n => n.NodeExtras)
            .HasForeignKey(ne => ne.NodeFk);

            //Extra
            entity.HasOne(ne => ne.Extra)
            .WithMany(e => e.NodeExtras)
            .HasForeignKey(ne => ne.ExtraFk);
        }
    }
}
