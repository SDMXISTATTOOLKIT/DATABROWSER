using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Specifications.Query;
using System.Collections.Generic;

namespace DataBrowser.Specifications.ViewTemplates
{
    public class ViewListByIdsSpecification : BaseSpecification<ViewTemplate>
    {
        public ViewListByIdsSpecification(List<int> viewIds)
            : base(b => b.Type == ViewTemplateType.View.ToString() && viewIds != null && viewIds.Contains(b.ViewTemplateId) )
        {
        }
    }
}
