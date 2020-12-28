using System.Collections.Generic;
using DataBrowser.Domain.Dtos;

namespace DataBrowser.UnitTests.HelperTest
{
    public class DashboardUtility
    {
        public static DashboardDto CreatePrivateDashboard(int id, int userId)
        {
            var dashboardDto = new DashboardDto
            {
                DashboardId = id,
                DashboardConfig = new DashboardDto.DashboardConfigItem[1][],
                Title = new Dictionary<string, string> {{"IT", "TESTTITOLO"}, {"EN", "TESTTITLE"}},
                UserId = userId
            };
            dashboardDto.DashboardConfig[0] = new[] {new DashboardDto.DashboardConfigItem {Value = 1}};
            return dashboardDto;
        }

        public static DashboardDto CreatePublicDashboard(int id, int userId, int hubId)
        {
            var dashboardDto = new DashboardDto
            {
                DashboardId = id,
                DashboardConfig = new DashboardDto.DashboardConfigItem[1][],
                Title = new Dictionary<string, string> {{"IT", "TESTTITOLO"}, {"EN", "TESTTITLE"}},
                UserId = userId,
                HubId = hubId
            };
            dashboardDto.DashboardConfig[0] = new[] {new DashboardDto.DashboardConfigItem {Value = 1}};
            return dashboardDto;
        }

        public static DashboardDto CreatePublicDashboard(int id, int userId, List<int> nodesId)
        {
            var dashboardDto = new DashboardDto
            {
                DashboardId = id,
                DashboardConfig = new DashboardDto.DashboardConfigItem[1][],
                Title = new Dictionary<string, string> {{"IT", "TESTTITOLO"}, {"EN", "TESTTITLE"}},
                UserId = userId,
                NodeIds = nodesId
            };
            dashboardDto.DashboardConfig[0] = new[] {new DashboardDto.DashboardConfigItem {Value = 1}};
            return dashboardDto;
        }
    }
}