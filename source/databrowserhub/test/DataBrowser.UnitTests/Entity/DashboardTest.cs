using System.Collections.Generic;
using System.Threading.Tasks;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.UnitTests.HelperTest;
using Xunit;

namespace DataBrowser.UnitTests.Entity
{
    public class DashboardTest
    {
        [Fact]
        public async Task CreateVariusDashboard_WithCorrectData_Ok()
        {
            var dashboardDto = EntityDashboardData.CreateStandardDashboardDto();
            var dashboard = await Dashboard.CreateDashboardAsync(dashboardDto, null);
            EntityDashboardData.CheckDashboardDtoFromDashboardEntity(dashboardDto, dashboard.ValidateObject, false);

            dashboardDto = new DashboardDto
            {
                DashboardConfig = new DashboardDto.DashboardConfigItem[1][],
                Title = new Dictionary<string, string> {{"en", "titlesss en"}},
                UserId = 1,
                Weight = 100,
                ViewIds = new List<int> {1, 5, 7, 10, 203404}
            };
            dashboardDto.DashboardConfig[0] = new[] {new DashboardDto.DashboardConfigItem {Value = 1}};

            dashboard = await Dashboard.CreateDashboardAsync(dashboardDto, null);
            EntityDashboardData.CheckDashboardDtoFromDashboardEntity(dashboardDto, dashboard.ValidateObject, false);

            dashboardDto = new DashboardDto
            {
                DashboardConfig = new DashboardDto.DashboardConfigItem[1][],
                Title = new Dictionary<string, string> {{"it", "titolo it2"}, {"en", "titlesss en2"}},
                UserId = 1,
                Weight = 100,
                NodeIds = new List<int> {1, 4, 80, 102}
            };
            dashboardDto.DashboardConfig[0] = new[] {new DashboardDto.DashboardConfigItem {Value = 2}};

            dashboard = await Dashboard.CreateDashboardAsync(dashboardDto, null);
            EntityDashboardData.CheckDashboardDtoFromDashboardEntity(dashboardDto, dashboard.ValidateObject, false);

            dashboardDto = new DashboardDto
            {
                DashboardConfig = new DashboardDto.DashboardConfigItem[1][],
                Title = new Dictionary<string, string> {{"it", "titolo it3"}},
                UserId = 1,
                Weight = 100,
                ViewIds = new List<int> {3, 9, 86, 15, 2391},
                NodeIds = new List<int> {1, 4, 80, 102}
            };
            dashboardDto.DashboardConfig[0] = new[] {new DashboardDto.DashboardConfigItem {Value = 3}};

            dashboard = await Dashboard.CreateDashboardAsync(dashboardDto, null);
            EntityDashboardData.CheckDashboardDtoFromDashboardEntity(dashboardDto, dashboard.ValidateObject, false);
        }

        [Fact]
        public async Task EditVariusDashboard_WithCorrectData_Ok()
        {
            var dashboardValidator = await Dashboard.CreateDashboardAsync(EntityDashboardData.CreateStandardDashboardDto(), null);
            var dashboard = dashboardValidator.ValidateObject;

            var dashboardDto = new DashboardDto
            {
                DashboardConfig = new DashboardDto.DashboardConfigItem[1][],
                Title = new Dictionary<string, string> {{"en", "titlesss en"}},
                UserId = 1,
                Weight = 100,
                ViewIds = new List<int> {1, 5, 7, 10, 203404}
            };
            dashboardDto.DashboardConfig[0] = new[] {new DashboardDto.DashboardConfigItem {Value = 1}};
            await dashboard.EditAsync(dashboardDto, null);
            EntityDashboardData.CheckTitleFromDto(dashboardDto, dashboard);
            Assert.Equal(3, dashboard.Views.Count);
            Assert.Equal(2, dashboard.Nodes.Count);
            Assert.Equal(1, dashboard.Weight);

            dashboardDto = new DashboardDto
            {
                DashboardConfig = new DashboardDto.DashboardConfigItem[1][],
                Title = new Dictionary<string, string> {{"it", "titolo it2"}, {"en", "titlesss en2"}},
                UserId = 1,
                Weight = 423,
                NodeIds = new List<int> {1, 4, 80, 102}
            };
            dashboardDto.DashboardConfig[0] = new[] {new DashboardDto.DashboardConfigItem {Value = 2}};
            await dashboard.EditAsync(dashboardDto, null);
            EntityDashboardData.CheckTitleFromDto(dashboardDto, dashboard);
            Assert.Equal(3, dashboard.Views.Count);
            Assert.Equal(2, dashboard.Nodes.Count);
            Assert.Equal(1, dashboard.Weight);

            dashboardDto = new DashboardDto
            {
                DashboardConfig = new DashboardDto.DashboardConfigItem[1][],
                Title = new Dictionary<string, string> {{"it", "titolo it3"}},
                UserId = 1,
                Weight = 754,
                ViewIds = new List<int> {3, 9, 86, 15, 2391},
                NodeIds = new List<int> {1, 4, 80, 102}
            };
            dashboardDto.DashboardConfig[0] = new[] {new DashboardDto.DashboardConfigItem {Value = 3}};
            await dashboard.EditAsync(dashboardDto, null);
            EntityDashboardData.CheckTitleFromDto(dashboardDto, dashboard);
            Assert.Equal(3, dashboard.Views.Count);
            Assert.Equal(2, dashboard.Nodes.Count);
            Assert.Equal(1, dashboard.Weight);
        }

        [Fact]
        public async Task EditTitle_Ok()
        {
            var dashboard = await EntityDashboardData.CreateStandardDashboardAsync();

            var titleNew = new Dictionary<string, string> {{"IT", "Titolo"}, {"FR", "TitleFR"}, {"DE", "Title DE"}};
            dashboard.SetTitleTranslation(titleNew);

            EntityDashboardData.CheckTitleFromDto(new DashboardDto {Title = titleNew}, dashboard);
        }

        [Fact]
        public async Task RemoveTitle_Ok()
        {
            var dashboard = await EntityDashboardData.CreateStandardDashboardAsync();

            dashboard.SetTitleTranslation(null);
            Assert.Null(dashboard.Title);
        }

        [Fact]
        public async Task AssignNode_Ok()
        {
            var dashboard = await EntityDashboardData.CreateStandardDashboardAsync();

            dashboard.AssignNode(89);
            Assert.Equal(3, dashboard.Nodes.Count);
            Assert.Contains(dashboard.Nodes, i => i.NodeId == 4);
            Assert.Contains(dashboard.Nodes, i => i.NodeId == 5);
            Assert.Contains(dashboard.Nodes, i => i.NodeId == 89);

            dashboard.AssignNode(89);
            Assert.Equal(3, dashboard.Nodes.Count);
            Assert.Contains(dashboard.Nodes, i => i.NodeId == 4);
            Assert.Contains(dashboard.Nodes, i => i.NodeId == 5);
            Assert.Contains(dashboard.Nodes, i => i.NodeId == 89);
        }

        [Fact]
        public async Task UnAssignNode_Ok()
        {
            var dashboard = await EntityDashboardData.CreateStandardDashboardAsync();

            dashboard.UnAssignNode(4);
            Assert.Equal(1, dashboard.Nodes.Count);
            Assert.Contains(dashboard.Nodes, i => i.NodeId == 5);

            dashboard.UnAssignNode(4);
            Assert.Equal(1, dashboard.Nodes.Count);
            Assert.Contains(dashboard.Nodes, i => i.NodeId == 5);
        }

        [Fact]
        public async Task SetNode_Ok()
        {
            var dashboard = await EntityDashboardData.CreateStandardDashboardAsync();

            dashboard.SetNode(new List<int> {4, 10, 102});
            Assert.Equal(3, dashboard.Nodes.Count);
            Assert.Contains(dashboard.Nodes, i => i.NodeId == 4);
            Assert.Contains(dashboard.Nodes, i => i.NodeId == 10);
            Assert.Contains(dashboard.Nodes, i => i.NodeId == 102);
        }

        [Fact]
        public async Task AssignView_Ok()
        {
            var dashboard = await EntityDashboardData.CreateStandardDashboardAsync();

            dashboard.AssignView(103);
            Assert.Equal(4, dashboard.Views.Count);
            Assert.Contains(dashboard.Views, i => i.ViewTemplateId == 1);
            Assert.Contains(dashboard.Views, i => i.ViewTemplateId == 2);
            Assert.Contains(dashboard.Views, i => i.ViewTemplateId == 3);
            Assert.Contains(dashboard.Views, i => i.ViewTemplateId == 103);

            dashboard.AssignView(103);
            Assert.Equal(4, dashboard.Views.Count);
            Assert.Contains(dashboard.Views, i => i.ViewTemplateId == 1);
            Assert.Contains(dashboard.Views, i => i.ViewTemplateId == 2);
            Assert.Contains(dashboard.Views, i => i.ViewTemplateId == 3);
            Assert.Contains(dashboard.Views, i => i.ViewTemplateId == 103);
        }

        [Fact]
        public async Task UnAssignView_Ok()
        {
            var dashboard = await EntityDashboardData.CreateStandardDashboardAsync();

            dashboard.UnAssignView(1);
            Assert.Equal(2, dashboard.Views.Count);
            Assert.Contains(dashboard.Views, i => i.ViewTemplateId == 2);
            Assert.Contains(dashboard.Views, i => i.ViewTemplateId == 3);

            dashboard.UnAssignView(1);
            Assert.Equal(2, dashboard.Views.Count);
            Assert.Contains(dashboard.Views, i => i.ViewTemplateId == 2);
            Assert.Contains(dashboard.Views, i => i.ViewTemplateId == 3);
        }

        [Fact]
        public async Task SetView_Ok()
        {
            var dashboard = await EntityDashboardData.CreateStandardDashboardAsync();

            dashboard.SetView(new List<int> {1, 54, 46});
            Assert.Equal(3, dashboard.Views.Count);
            Assert.Contains(dashboard.Views, i => i.ViewTemplateId == 1);
            Assert.Contains(dashboard.Views, i => i.ViewTemplateId == 54);
            Assert.Contains(dashboard.Views, i => i.ViewTemplateId == 46);
        }
    }
}