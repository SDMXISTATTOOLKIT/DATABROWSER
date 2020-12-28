using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataBrowser.DomainServices.Interfaces
{
    public interface IViewTemplateValidator
    {
        Task<bool> NameValidatorAsync(IRepository<ViewTemplate> repository);
    }
}
