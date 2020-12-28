using DataBrowser.Domain.Entities.SeedWork;

namespace DataBrowser.Domain.Events
{
    public class ImageChangePublicEvent : PublicEventBase
    {
        public string EntityName { get; }
        public string OldFilePath { get; }
        public string NewFilePath { get; }

        public ImageChangePublicEvent(string entityName, string oldFilePath, string newFilePath)
        {
            EntityName = entityName;
            OldFilePath = oldFilePath;
            NewFilePath = newFilePath;
        }

    }
}
