using DataBrowser.Domain.Validators;
using System.Threading.Tasks;

namespace DataBrowser.Domain.Specifications.Rules
{
    public interface IRuleSpecification<T>
    {
        Task<ValidatorResult> IsSatisfiedAsync(T subject);
    }
}
