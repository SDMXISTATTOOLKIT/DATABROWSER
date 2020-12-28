using System.Collections.Generic;

namespace DataBrowser.Domain.Validators
{
    public class ValidatorError
    {
        public string Code { get; set; }
        public ValidatorErrorDetail Detail { get; set; }
        public ValidatorType Type { get; set; }
        public string GeneratorClass { get; set; }
    }
    public enum ValidatorType { Rules, DomainEntity, Business, Other }
}
