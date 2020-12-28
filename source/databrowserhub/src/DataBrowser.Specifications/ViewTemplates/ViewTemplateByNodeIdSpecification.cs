using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Specifications.Query;

namespace DataBrowser.Specifications.ViewTemplates
{
    public class ViewTemplateByNodeIdSpecification : BaseSpecification<ViewTemplate>
    {
        public ViewTemplateByNodeIdSpecification(int nodeId = -1)
            : base(b => nodeId == -1 || b.NodeFK == nodeId)
        {
        }
    }
}
