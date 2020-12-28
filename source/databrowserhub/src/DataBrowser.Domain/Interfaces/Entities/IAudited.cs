using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Interfaces.Entities
{
    public interface IAudited : ICreationAudited, IModificationAudited
    {
    }
}
