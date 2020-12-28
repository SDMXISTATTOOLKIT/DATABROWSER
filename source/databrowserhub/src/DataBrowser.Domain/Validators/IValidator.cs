using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataBrowser.Domain.Validators
{
    public interface IValidator<T, TResult>
    {
        Task ExecuteCheckAsync(T dtoEntity, TResult validateObject);
        bool IsValid { get; }
        IEnumerable<ValidatorResult> BrokenRules { get; }
        TResult ValidateObject { get; }
    }
}
