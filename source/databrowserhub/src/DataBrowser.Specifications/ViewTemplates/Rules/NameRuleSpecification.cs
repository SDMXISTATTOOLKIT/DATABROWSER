using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Entities.ViewTemplates.Validators;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Domain.Serialization;
using DataBrowser.Domain.Specifications.Rules;
using DataBrowser.Domain.Validators;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBrowser.Specifications.ViewTemplates.Rules
{
    public class NameRuleSpecification : IRuleSpecification<ViewTemplateDto>
    {
        private readonly IRepository<ViewTemplate> _viewTemplate;

        public NameRuleSpecification(IRepository<ViewTemplate> viewTemplate)
        {
            _viewTemplate = viewTemplate;
        }

        public async Task<ValidatorResult> IsSatisfiedAsync(ViewTemplateDto viewTemplate)
        {
            var result = new ValidatorResult { IsSatisfied = true };
            if (_viewTemplate == null || !viewTemplate.UserId.HasValue)
            {
                return result;
            }

            var nameToCheck = viewTemplate.Title.ToDictionary(k => k.Key, i => i.Value);
            if (nameToCheck == null ||
                !nameToCheck.Any())
            {
                return result;
            }

            var allViews = await _viewTemplate.FindAsync(new ViewByUserIdSpecification(viewTemplate.UserId.Value));
            var otherViewNames = allViews?.Where(i=>i.ViewTemplateId != viewTemplate.ViewTemplateId)?.SelectMany(i => i.Title?.TransatableItemValues?.ToDictionary(k => k.Language, k => k.Value));
            if (otherViewNames == null ||
                !otherViewNames.Any())
            {
                return result;
            }

            var allTitleForLang = new Dictionary<string, List<string>>();
            foreach (var item in otherViewNames)
            {
                if (!allTitleForLang.ContainsKey(item.Key))
                {
                    allTitleForLang.Add(item.Key, new List<string>());
                }
                allTitleForLang[item.Key].Add(item.Value);
            }


            var nameConflicts = nameToCheck?.Where(i => allTitleForLang.ContainsKey(i.Key) && allTitleForLang[i.Key].Any(k => k.Equals(i.Value, StringComparison.InvariantCultureIgnoreCase)))?.ToDictionary(i => i.Key, i => i.Value);
            if (nameConflicts != null &&
                nameConflicts.Any())
            {
                result.IsSatisfied = false;
                result.Errors = new List<ValidatorError> {
                    new ValidatorError {
                        Code = RulesConstant.ErrorCode.DuplicateName,
                        Detail = new ValidatorErrorDetail { JsonData = DataBrowserJsonSerializer.SerializeObject(nameConflicts) },
                        Type = ValidatorType.Rules,
                        GeneratorClass = this.GetType().FullName
                    }
                };
            }

            return result;
        }
    }

}
