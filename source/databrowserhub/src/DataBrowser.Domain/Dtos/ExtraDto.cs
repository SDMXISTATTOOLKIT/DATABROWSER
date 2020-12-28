using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Dtos
{
    public class ExtraDto
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
        public bool IsPublic { get; set; }
        public Dictionary<string, string> Transaltes { get; set; }
    }
}
