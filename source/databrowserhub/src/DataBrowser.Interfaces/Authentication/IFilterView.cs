using System.Security.Claims;
using DataBrowser.Domain.Dtos;

namespace DataBrowser.Interfaces.Authentication
{
    public interface IFilterView
    {
        bool CheckPermission(ViewTemplateDto view, ClaimsPrincipal specificUser = null);
    }
}