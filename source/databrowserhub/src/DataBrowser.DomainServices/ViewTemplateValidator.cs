using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.DomainServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataBrowser.DomainServices
{
    public class ViewTemplateValidator : IViewTemplateValidator
    {
        public async Task<bool> NameValidatorAsync(IRepository<ViewTemplate> repository)
        {
            await repository.GetByIdAsync(1);

            return true;
        }
    }
}
