using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Specifications.Query;
using System.Collections.Generic;
using System.Linq;

namespace DataBrowser.Specifications.ViewTemplates
{
    public class ViewTemplateByMultiIdsSpecification : BaseSpecification<ViewTemplate>
    {
        public ViewTemplateByMultiIdsSpecification(List<int> viewTemplateIds)
            : base(b => viewTemplateIds.Any(k => k == b.ViewTemplateId))
        {
            AddInclude("Title.TransatableItemValues");
        }
    }
}
