using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Specifications.Query;

namespace DataBrowser.Specifications.ViewTemplates
{
    public class ViewTemplateByTypeSpecification : BaseSpecification<ViewTemplate>
    {
        public ViewTemplateByTypeSpecification(int nodeId, ViewTemplateType viewTemplateType = ViewTemplateType.View)
            : base(b => (nodeId == -1 || nodeId == b.NodeFK) && b.Type == viewTemplateType.ToString())
        {
        }
    }
}
