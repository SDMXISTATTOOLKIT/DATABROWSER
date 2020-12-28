using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Entities.Versioning
{
    public class DataBrowserVersionActionUpgrader
    {
        public int DataBrowserVersionActionUpgraderId { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string Name { get; set; }
        public bool Success { get; set; }
        public string Errors { get; set; }
        public int DataBrowserVersionId { get; set; }
    }
}
