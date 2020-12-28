using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Specifications.Query;

namespace DataBrowser.Specifications.ViewTemplates
{
    public class ViewTemplateByType_Dataset_NodeIdSpecification : BaseSpecification<ViewTemplate>
    {
        public ViewTemplateByType_Dataset_NodeIdSpecification(int nodeId, string datasetId, ViewTemplateType viewTemplateType = ViewTemplateType.View)
            : base(b => (nodeId == -1 || nodeId == b.NodeFK) && b.DatasetId == datasetId && b.Type == viewTemplateType.ToString())
        {
        }
    }
}
