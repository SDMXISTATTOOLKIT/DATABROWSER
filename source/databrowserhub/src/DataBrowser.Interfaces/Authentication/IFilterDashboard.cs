using System.Security.Claims;
using DataBrowser.Domain.Dtos;

namespace DataBrowser.Interfaces.Authentication
{
    public interface IFilterDashboard
    {
        bool CheckReadPermission(DashboardDto dashboard, ClaimsPrincipal specificUser = null);
        bool CheckWritePermission(DashboardDto dashboard, ClaimsPrincipal specificUser = null);
    }
}