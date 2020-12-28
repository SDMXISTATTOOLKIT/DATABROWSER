using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Validators
{
    public class ValidatorResult
    {
        public bool IsSatisfied { get; set; }
        public IEnumerable<ValidatorError> Errors { get; set; }
    }
    
}
