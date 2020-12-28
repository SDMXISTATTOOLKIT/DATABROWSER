using DataBrowser.Domain.Entities.ViewTemplates;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataBrowser.DomainServices.Interfaces
{
    public interface IViewTemplateFactory
    {
        Task<ViewTemplate> CreateViewAsync(IViewTemplateValidator viewTemplateValidator);
        ViewTemplate CreateTemplateAsync();
        Task<ViewTemplate> ChangeViewNameAsync(IViewTemplateValidator viewTemplateValidator);
        Task<ViewTemplate> ChangeTemplateNameAsync(IViewTemplateValidator viewTemplateValidator);
    }
}
