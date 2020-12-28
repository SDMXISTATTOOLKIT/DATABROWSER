using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Interfaces.Entities
{
    public interface IModificationAudited : IHasModificationTime
    {
        long? LastModifierUserId { get; set; }
    }
}
