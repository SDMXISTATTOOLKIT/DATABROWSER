using System.Security.Claims;
using DataBrowser.Domain.Dtos;

namespace DataBrowser.Interfaces.Authentication
{
    public interface IFilterTemplate
    {
        bool CheckPermission(ViewTemplateDto template, ClaimsPrincipal specificUser = null);
    }
}