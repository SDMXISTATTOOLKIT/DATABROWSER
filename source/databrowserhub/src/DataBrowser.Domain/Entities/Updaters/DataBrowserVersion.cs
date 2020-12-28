using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Entities.Versioning
{
    public class DataBrowserVersion
    {
        public int DataBrowserVersionId { get; set; }
        public DateTime From { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
        public int Revision { get; set; }
        public bool IsCurrentVersion { get; set; }
    }
}
