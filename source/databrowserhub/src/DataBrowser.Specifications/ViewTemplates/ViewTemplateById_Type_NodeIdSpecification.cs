using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Specifications.Query;

namespace DataBrowser.Specifications.ViewTemplates
{
    public class ViewTemplateById_Type_NodeIdSpecification : BaseSpecification<ViewTemplate>
    {
        public ViewTemplateById_Type_NodeIdSpecification(int viewTemplate, int nodeId, ViewTemplateType viewTemplateType = ViewTemplateType.View)
            : base(b => b.ViewTemplateId == viewTemplate && (nodeId == -1 || nodeId == b.NodeFK) && b.Type == viewTemplateType.ToString())
        {
        }
    }
}
