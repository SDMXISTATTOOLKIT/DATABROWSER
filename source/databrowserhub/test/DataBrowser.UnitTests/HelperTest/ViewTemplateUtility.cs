using DataBrowser.Domain.Dtos;

namespace DataBrowser.UnitTests.HelperTest
{
    public class ViewTemplateUtility
    {
        public static ViewTemplateDto CreateTemplate(int nodeId)
        {
            return new ViewTemplateDto
            {
                NodeId = nodeId,
                ViewTemplateId = 1,
                Type = ViewTemplateType.Template,
                DatasetId = "ab+cd+1.1",
                UserId = null
            };
        }

        public static ViewTemplateDto CreateView(int nodeId, int userId)
        {
            return new ViewTemplateDto
            {
                NodeId = nodeId,
                ViewTemplateId = 2,
                Type = ViewTemplateType.View,
                DatasetId = "zt+ys+2.4",
                UserId = userId
            };
        }
    }
}