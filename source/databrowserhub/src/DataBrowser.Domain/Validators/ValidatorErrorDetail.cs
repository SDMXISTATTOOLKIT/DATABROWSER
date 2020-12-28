using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Validators
{
    public class ValidatorErrorDetail
    {
        public IEnumerable<string> Messages { get; set; }
        public object CustomData { get; set; }
        public string JsonData { get; set; }
    }
}
