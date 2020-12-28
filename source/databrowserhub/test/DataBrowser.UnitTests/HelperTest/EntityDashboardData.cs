using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.DBoard;
using Xunit;

namespace DataBrowser.UnitTests.HelperTest
{
    public static class EntityDashboardData
    {
        public static DashboardDto CreateStandardDashboardDto()
        {
            var dashboardDto = new DashboardDto
            {
                DashboardConfig = new DashboardDto.DashboardConfigItem[1][],
                Title = new Dictionary<string, string> {{"en", "titlesss en"}, {"it", "titolo it"}},
                UserId = 1,
                Weight = 1,
                ViewIds = new List<int> {1, 2, 3},
                NodeIds = new List<int> {4, 5}
            };
            dashboardDto.DashboardConfig[0] = new[] {new DashboardDto.DashboardConfigItem {Value = 1}};
            return dashboardDto;
        }

        public static async Task<Dashboard> CreateStandardDashboardAsync()
        {
            var validator = await Dashboard.CreateDashboardAsync(CreateStandardDashboardDto(), null);
            return validator.ValidateObject;
        }

        public static void CheckDashbordEntityFromDashboardDto(DashboardDto dashboardDto, Dashboard dashboard)
        {
            Assert.Equal(dashboardDto.ConvertoDashboardConfigToText(), dashboard.DashboardConfig);
            Assert.Equal(dashboardDto.Weight, dashboard.Weight);
            Assert.Equal(dashboardDto.UserId, dashboard.UserFk);
            Assert.Equal(dashboardDto.HubId, dashboard.HubFk ?? -1);
            if (dashboardDto.NodeIds == null || dashboardDto.NodeIds.Count == 0)
            {
                Assert.Equal(0, dashboard.Nodes.Count);
            }
            else
            {
                Assert.Equal(dashboardDto.NodeIds.Count, dashboard.Nodes.Count);
                Assert.True(dashboardDto.NodeIds.All(nodeId => dashboard.Nodes.Any(isItem => isItem.NodeId == nodeId)));
            }

            if (dashboardDto.ViewIds == null || dashboardDto.ViewIds.Count == 0)
            {
                Assert.Equal(0, dashboard.Views.Count);
            }
            else
            {
                Assert.Equal(dashboardDto.ViewIds.Count, dashboard.Views.Count);
                Assert.True(dashboardDto.ViewIds.All(nodeId =>
                    dashboard.Views.Any(isItem => isItem.ViewTemplateId == nodeId)));
            }

            if (dashboardDto.Title == null || dashboardDto.Title.Count == 0)
            {
                Assert.Null(dashboard.Title);
            }
            else
            {
                Assert.Equal(dashboardDto.Title.Count, dashboard.Title.TransatableItemValues.Count);
                foreach (var item in dashboard.Title.TransatableItemValues)
                {
                    Assert.True(dashboardDto.Title.ContainsKey(item.Language));
                    Assert.Equal(dashboardDto.Title[item.Language], item.Value);
                }
            }
        }

        public static void CheckDashboardDtoFromDashboardEntity(DashboardDto dashboardDto, Dashboard dashboard,
            bool onlyEditData)
        {
            Assert.Equal(dashboard.DashboardConfig, dashboardDto.ConvertoDashboardConfigToText());

            if (dashboard.Title == null)
            {
                Assert.Empty(dashboardDto.Title);
            }
            else
            {
                Assert.Equal(dashboard.Title.TransatableItemValues.Count, dashboardDto.Title.Count);
                foreach (var item in dashboardDto.Title)
                {
                    Assert.Contains(dashboard.Title.TransatableItemValues, i => i.Language.Equals(item.Key));
                    var entityTransalte = dashboard.Title.TransatableItemValues.First(i => i.Language.Equals(item.Key));
                    Assert.Equal(entityTransalte.Value, item.Value);
                }
            }

            if (onlyEditData) return;
            Assert.Equal(dashboard.Weight, dashboardDto.Weight);
            Assert.Equal(dashboard.UserFk, dashboardDto.UserId);
            Assert.Equal(dashboard.HubFk ?? -1, dashboardDto.HubId ?? -1);
            if (dashboardDto.NodeIds == null || dashboardDto.NodeIds.Count == 0)
            {
                Assert.Equal(0, dashboard.Nodes.Count);
            }
            else
            {
                Assert.Equal(dashboardDto.NodeIds.Count, dashboard.Nodes.Count);
                Assert.True(dashboardDto.NodeIds.All(nodeId => dashboard.Nodes.Any(isItem => isItem.NodeId == nodeId)));
            }

            if (dashboard.Views == null || dashboard.Views.Count == 0)
            {
                Assert.Equal(0, dashboardDto?.ViewIds?.Count ?? 0);
            }
            else
            {
                Assert.Equal(dashboard.Views.Count, dashboardDto.ViewIds.Count);
                Assert.True(dashboardDto.ViewIds.All(nodeId =>
                    dashboard.Views.Any(isItem => isItem.ViewTemplateId == nodeId)));
            }
        }


        public static void CheckTitleFromDto(DashboardDto dashboardDto, Dashboard dashboard)
        {
            Assert.Equal(dashboardDto.Title.Count, dashboard.Title.TransatableItemValues.Count);
            if (dashboardDto.Title == null) return;
            foreach (var item in dashboardDto.Title)
            {
                Assert.Contains(dashboard.Title.TransatableItemValues, i => i.Language.Equals(item.Key));
                var entityTransalte = dashboard.Title.TransatableItemValues.First(i => i.Language.Equals(item.Key));
                Assert.Equal(entityTransalte.Value, item.Value);
            }
        }

        public static void CheckNodesFromDto(DashboardDto dashboardDto, Dashboard dashboard)
        {
            Assert.Equal(dashboardDto.Title.Count, dashboard.Title.TransatableItemValues.Count);
            if (dashboardDto.Title == null) return;
            foreach (var item in dashboardDto.Title)
            {
                Assert.Contains(dashboard.Title.TransatableItemValues, i => i.Language.Equals(item.Key));
                var entityTransalte = dashboard.Title.TransatableItemValues.First(i => i.Language.Equals(item.Key));
                Assert.Equal(entityTransalte.Value, item.Value);
            }
        }
    }
}