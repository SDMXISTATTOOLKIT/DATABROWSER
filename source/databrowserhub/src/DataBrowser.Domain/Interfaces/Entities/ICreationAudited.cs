using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Interfaces.Entities
{
    public interface ICreationAudited : IHasCreationTime
    {
        long? CreatorUserId { get; set; }
    }
}
