using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Specifications.Query;

namespace DataBrowser.Specifications.ViewTemplates
{
    public class ViewByUserIdSpecification : BaseSpecification<ViewTemplate>
    {
        public ViewByUserIdSpecification(int userId)
            : base(b => b.UserFK == userId && b.Type == ViewTemplateType.View.ToString())
        {
        }
    }
}
