using DataBrowser.Domain.Specifications.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBrowser.Domain.Validators
{
    public class Validator<T, TResult> : IValidator<T, TResult>
    {
        private readonly IEnumerable<IRuleSpecification<T>> _rules;
        private List<ValidatorResult> _ruleResults;
        private bool _executeRun;
        private TResult _validateObject;

        public Validator(IEnumerable<IRuleSpecification<T>> rules)
        {
            _rules = rules;
        }

        public async Task ExecuteCheckAsync(T dtoEntity, TResult viewTemplate)
        {
            _validateObject = viewTemplate;
            _executeRun = true;
            _ruleResults = new List<ValidatorResult>();
            if (_rules == null)
            {
                return;
            }

            foreach (var item in _rules)
            {
                var isSatisfied = await item.IsSatisfiedAsync(dtoEntity);
                if (!isSatisfied.IsSatisfied)
                {
                    _ruleResults.Add(isSatisfied);
                }
            }
        }

        public bool IsValid
        {
            get
            {
                if (!_executeRun)
                {
                    throw new Exception("ExecuteCheckAsync() before to call IsValid()");
                }

                if (_ruleResults == null ||
                    !_ruleResults.Any())
                {
                    return true;
                }

                return false;
            }
        }

        public IEnumerable<ValidatorResult> BrokenRules
        {
            get
            {
                if (!_executeRun)
                {
                    throw new Exception("ExecuteCheckAsync() before to call IsValid()");
                }

                return _ruleResults;
            }
        }

        public TResult ValidateObject => IsValid ? _validateObject : default(TResult);
    }
}
