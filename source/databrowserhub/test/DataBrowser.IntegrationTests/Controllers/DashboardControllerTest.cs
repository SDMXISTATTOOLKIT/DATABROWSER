using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DataBrowser.IntegrationTests.HelperTest;
using DataBrowser.Query.Dashboards.ModelView;
using Newtonsoft.Json;
using WSHUB;
using WSHUB.Models.Request;
using Xunit;

namespace DataBrowser.IntegrationTests.Controllers
{
    public class DashboardControllerTest : BaseControllerTest
    {
        public DashboardControllerTest(CustomWebApplicationFactory<Startup> factory)
            : base(factory)
        {
            var token = UtilityTest.GetAdminToken(httpClient).Result.Token;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        [Fact]
        public async Task Create_Dashboard_AdminUser_Ok()
        {
            // Act
            var dashboardDto = new DashboardCreateModelView
            {
                Title = new Dictionary<string, string> {{"IT", "Titolo11Edit"}, {"ES", "Espan Ti2222tle"}},
                DashboardConfig =
                    @"[[{""type"":""text"",""value"":{""gb"":""<p>text text text</p>"",""it"":""<p>testo testo testo</p>"",""fr"":""<p>texte&nbsp;texte&nbsp;texte</p>""},""widthPercentage"":100,""showTitle"":true,""enableFilters"":true}]]",
                ViewIds = new List<int> {2, 3, 8},
                Weight = 22
            };

            // Assert
            var response = await httpClient.PostAsync("/Dashboards",
                new StringContent(JsonConvert.SerializeObject(dashboardDto), Encoding.UTF8, "application/json"));
            Assert.True(response.StatusCode == HttpStatusCode.Created);
            Assert.Equal("application/text", response.Content.Headers.ContentType.ToString());
            var id = Convert.ToInt32(await response.Content.ReadAsStringAsync());

            response = await httpClient.GetAsync($"/Dashboards/{id}");
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());

            var strResult = await response.Content.ReadAsStringAsync();
            var dashboard = JsonConvert.DeserializeObject<DashboardViewModel>(strResult);


            Assert.Equal(dashboardDto.DashboardConfig, dashboard.DashboardConfig);
            Assert.Equal(id, dashboard.DashboardId);
            Assert.Equal(2, dashboardDto.Title.Count);
            Assert.Contains(dashboard.Title, i => i.Key.Equals("it", StringComparison.InvariantCultureIgnoreCase));
            Assert.Contains(dashboard.Title, i => i.Key.Equals("es", StringComparison.InvariantCultureIgnoreCase));
            Assert.Contains(dashboard.Title,
                i => i.Value.Equals("Titolo11Edit", StringComparison.InvariantCultureIgnoreCase));
            Assert.Contains(dashboard.Title,
                i => i.Value.Equals("Espan Ti2222tle", StringComparison.InvariantCultureIgnoreCase));
            Assert.Equal(2, dashboard.ViewIds.Count);
            Assert.Contains(dashboard.ViewIds, i => i == 2);
            Assert.Contains(dashboard.ViewIds, i => i == 3);
            //Assert.Contains(dashboard.ViewIds, i => i == 8);DISCARD BEACUSE view 8 don't assign to current user
        }

        [Fact]
        public async Task Get_Dashboards_AdminUser_Ok()
        {
            // Act
            var response = await httpClient.GetAsync("/Dashboards");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK);
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());

            var strResult = await response.Content.ReadAsStringAsync();
            var dashboards = JsonConvert.DeserializeObject<List<DashboardViewModel>>(strResult);

            Assert.Equal(File.ReadAllText("GoldenMaster/Get_Dashboards_AdminUser_Ok.txt"), strResult);

            Assert.Equal(4, dashboards.Count);
        }

        [Fact]
        public async Task Get_Dashboards_AdminUserTwo_Ok()
        {
            // Act
            var token = UtilityTest.GetSeconAdminToken(httpClient).Result.Token;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.GetAsync("/Dashboards");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK);
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());

            var strResult = await response.Content.ReadAsStringAsync();
            var dashboards = JsonConvert.DeserializeObject<List<DashboardViewModel>>(strResult);

            Assert.Equal(1, dashboards.Count);
            Assert.Equal(3, dashboards.First().DashboardId);
        }

        [Fact]
        public async Task Get_Dashboards_NormalUser_Ok()
        {
            // Act
            var token = UtilityTest.GetUserToken(httpClient).Result.Token;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.GetAsync("/Dashboards");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK);
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());

            var strResult = await response.Content.ReadAsStringAsync();
            var dashboards = JsonConvert.DeserializeObject<List<DashboardViewModel>>(strResult);

            Assert.Equal(0, dashboards.Count);
        }

        [Fact]
        public async Task Get_Dashboards_Anonymous_Ok()
        {
            // Act
            httpClient.DefaultRequestHeaders.Authorization = null;
            var response = await httpClient.GetAsync("/Dashboards");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK);
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());

            var strResult = await response.Content.ReadAsStringAsync();
            var dashboards = JsonConvert.DeserializeObject<List<DashboardViewModel>>(strResult);

            Assert.Equal(0, dashboards.Count);
        }

        [Fact]
        public async Task Get_NodeDashboards_AdminUser_Ok()
        {
            // Act
            var response = await httpClient.GetAsync("/Dashboards/Nodes/2");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK);
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());

            var strResult = await response.Content.ReadAsStringAsync();
            var dashboards = JsonConvert.DeserializeObject<List<DashboardViewModel>>(strResult);

            Assert.Equal(2, dashboards.Count);

            //Assert.Equal(File.ReadAllText("GoldenMaster/Get_DashboardsNodes2_AdminUser_Ok.txt"), strResult);

            Assert.Contains(dashboards, i => i.DashboardId == 4);
            Assert.Contains(dashboards, i => i.DashboardId == 5);
            Assert.True(dashboards.First(i => i.DashboardId == 4).Views.Count == 1);
            Assert.True(dashboards.First(i => i.DashboardId == 5).Views.Count == 1);
            Assert.NotNull(dashboards.First(i => i.DashboardId == 4).Views.First(i => i.Key == 1));
            Assert.NotNull(dashboards.First(i => i.DashboardId == 5).Views.First(i => i.Key == 1));
        }

        [Fact]
        public async Task Get_NodeDashboards_NormalUser_Ok()
        {
            // Act
            var token = UtilityTest.GetUserToken(httpClient).Result.Token;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.GetAsync("/Dashboards/Nodes/2");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK);
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());

            var strResult = await response.Content.ReadAsStringAsync();
            var dashboards = JsonConvert.DeserializeObject<List<DashboardViewModel>>(strResult);

            Assert.Equal(2, dashboards.Count);
            Assert.Contains(dashboards, i => i.DashboardId == 4);
            Assert.Contains(dashboards, i => i.DashboardId == 5);
            Assert.True(dashboards.First(i => i.DashboardId == 4).Views.Count == 1);
            Assert.True(dashboards.First(i => i.DashboardId == 5).Views.Count == 1);
            Assert.NotNull(dashboards.First(i => i.DashboardId == 4).Views.First(i => i.Key == 1));
            Assert.NotNull(dashboards.First(i => i.DashboardId == 5).Views.First(i => i.Key == 1));
        }

        [Fact]
        public async Task Get_NodeDashboards_Anonymous_Ok()
        {
            // Act
            httpClient.DefaultRequestHeaders.Authorization = null;
            var response = await httpClient.GetAsync("/Dashboards/Nodes/2");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK);
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());

            var strResult = await response.Content.ReadAsStringAsync();
            var dashboards = JsonConvert.DeserializeObject<List<DashboardViewModel>>(strResult);

            Assert.Equal(2, dashboards.Count);
            Assert.Contains(dashboards, i => i.DashboardId == 4);
            Assert.Contains(dashboards, i => i.DashboardId == 5);
            Assert.True(dashboards.First(i => i.DashboardId == 4).Views.Count == 1);
            Assert.True(dashboards.First(i => i.DashboardId == 5).Views.Count == 1);
            Assert.NotNull(dashboards.First(i => i.DashboardId == 4).Views.First(i => i.Key == 1));
            Assert.NotNull(dashboards.First(i => i.DashboardId == 5).Views.First(i => i.Key == 1));
        }

        [Fact]
        public async Task Get_HubDashboards_AdminUser_Ok()
        {
            // Act
            var response = await httpClient.GetAsync("/Dashboards/Hub");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK);
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());

            var strResult = await response.Content.ReadAsStringAsync();
            var dashboards = JsonConvert.DeserializeObject<List<DashboardViewModel>>(strResult);

            Assert.Equal(3, dashboards.Count);

            //Assert.Equal(File.ReadAllText("GoldenMaster/Get_DashboardsHub_AdminUser_Ok.txt"), strResult);

            Assert.Contains(dashboards, i => i.DashboardId == 2);
            Assert.Contains(dashboards, i => i.DashboardId == 3);
            Assert.Contains(dashboards, i => i.DashboardId == 5);
            Assert.True(dashboards.First(i => i.DashboardId == 2).Views.Count == 2);
            Assert.True(dashboards.First(i => i.DashboardId == 3).Views.Count == 1);
            Assert.True(dashboards.First(i => i.DashboardId == 5).Views.Count == 1);
            Assert.NotNull(dashboards.First(i => i.DashboardId == 2).Views.First(i => i.Key == 2));
            Assert.NotNull(dashboards.First(i => i.DashboardId == 2).Views.First(i => i.Key == 3));
            Assert.NotNull(dashboards.First(i => i.DashboardId == 3).Views.First(i => i.Key == 8));
            Assert.NotNull(dashboards.First(i => i.DashboardId == 5).Views.First(i => i.Key == 1));
        }

        [Fact]
        public async Task Get_HubDashboards_NormalUser_Ok()
        {
            // Act
            var token = UtilityTest.GetUserToken(httpClient).Result.Token;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.GetAsync("/Dashboards/Hub");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK);
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());

            var strResult = await response.Content.ReadAsStringAsync();
            var dashboards = JsonConvert.DeserializeObject<List<DashboardViewModel>>(strResult);

            Assert.Equal(3, dashboards.Count);

            Assert.Contains(dashboards, i => i.DashboardId == 2);
            Assert.Contains(dashboards, i => i.DashboardId == 3);
            Assert.Contains(dashboards, i => i.DashboardId == 5);
            Assert.True(dashboards.First(i => i.DashboardId == 2).Views.Count == 2);
            Assert.True(dashboards.First(i => i.DashboardId == 3).Views.Count == 1);
            Assert.True(dashboards.First(i => i.DashboardId == 5).Views.Count == 1);
            Assert.NotNull(dashboards.First(i => i.DashboardId == 2).Views.First(i => i.Key == 2));
            Assert.NotNull(dashboards.First(i => i.DashboardId == 2).Views.First(i => i.Key == 3));
            Assert.NotNull(dashboards.First(i => i.DashboardId == 3).Views.First(i => i.Key == 8));
            Assert.NotNull(dashboards.First(i => i.DashboardId == 5).Views.First(i => i.Key == 1));
        }

        [Fact]
        public async Task Get_HubDashboards_Anonymous_Ok()
        {
            // Act
            httpClient.DefaultRequestHeaders.Authorization = null;
            var response = await httpClient.GetAsync("/Dashboards/Hub");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK);
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());

            var strResult = await response.Content.ReadAsStringAsync();
            var dashboards = JsonConvert.DeserializeObject<List<DashboardViewModel>>(strResult);

            Assert.Equal(3, dashboards.Count);

            Assert.Contains(dashboards, i => i.DashboardId == 2);
            Assert.Contains(dashboards, i => i.DashboardId == 3);
            Assert.Contains(dashboards, i => i.DashboardId == 5);
            Assert.True(dashboards.First(i => i.DashboardId == 2).Views.Count == 2);
            Assert.True(dashboards.First(i => i.DashboardId == 3).Views.Count == 1);
            Assert.True(dashboards.First(i => i.DashboardId == 5).Views.Count == 1);
            Assert.NotNull(dashboards.First(i => i.DashboardId == 2).Views.First(i => i.Key == 2));
            Assert.NotNull(dashboards.First(i => i.DashboardId == 2).Views.First(i => i.Key == 3));
            Assert.NotNull(dashboards.First(i => i.DashboardId == 3).Views.First(i => i.Key == 8));
            Assert.NotNull(dashboards.First(i => i.DashboardId == 5).Views.First(i => i.Key == 1));
        }

        [Fact]
        public async Task Get_Dashboard_AdminUser_Ok()
        {
            // Act
            var response = await httpClient.GetAsync("/Dashboards/5");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK);
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());

            var strResult = await response.Content.ReadAsStringAsync();
            var dashboard = JsonConvert.DeserializeObject<DashboardViewModel>(strResult);


            //Assert.Equal(File.ReadAllText("GoldenMaster/Get_Dashboard_AdminUser_Ok.txt"), strResult);

            Assert.Equal(5, dashboard.DashboardId);
            Assert.Equal(1, dashboard.ViewIds.Count);
            Assert.Equal(1, dashboard.HubId);
            Assert.Contains(dashboard.NodeIds, i => i == 1);
            Assert.Contains(dashboard.NodeIds, i => i == 2);
            Assert.Contains(dashboard.NodeIds, i => i == 3);
        }

        [Fact]
        public async Task Get_Dashboard_NormalUser_Ok()
        {
            // Act
            var token = UtilityTest.GetUserToken(httpClient).Result.Token;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.GetAsync("/Dashboards/5");

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK);
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());

            var strResult = await response.Content.ReadAsStringAsync();
            var dashboard = JsonConvert.DeserializeObject<DashboardViewModel>(strResult);


            //Assert.Equal(File.ReadAllText("GoldenMaster/Get_Dashboard_AdminUser_Ok.txt"), strResult);

            Assert.Equal(5, dashboard.DashboardId);
            Assert.Equal(1, dashboard.ViewIds.Count);
            Assert.Equal(1, dashboard.HubId);
            Assert.Contains(dashboard.NodeIds, i => i == 1);
            Assert.Contains(dashboard.NodeIds, i => i == 2);
            Assert.Contains(dashboard.NodeIds, i => i == 3);
        }

        [Fact]
        public async Task Delete_DashboardAssignToNodeHub_AdminUser_Ok()
        {
            var response = await httpClient.DeleteAsync("/Dashboards/5");
            Assert.True(response.StatusCode == HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task Delete_Dashboard_AdminUser_Ok()
        {
            await httpClient.DeleteAsync("/Dashboards/5/Hub");
            await httpClient.DeleteAsync("/Dashboards/5/Nodes/1");
            await httpClient.DeleteAsync("/Dashboards/5/Nodes/2");
            await httpClient.DeleteAsync("/Dashboards/5/Nodes/3");

            var response = await httpClient.DeleteAsync("/Dashboards/5");
            Assert.True(response.StatusCode == HttpStatusCode.NoContent);

            response = await httpClient.GetAsync("/Dashboards/5");
            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_DashboardViews_AdminUser_Ok()
        {
            var response = await httpClient.DeleteAsync("/Dashboards/5/Views");
            Assert.True(response.StatusCode == HttpStatusCode.NoContent);

            response = await httpClient.GetAsync("/Dashboards/5");
            var strResult = await response.Content.ReadAsStringAsync();
            var dashboard = JsonConvert.DeserializeObject<DashboardViewModel>(strResult);
            Assert.Equal(0, dashboard.ViewIds.Count);
        }

        [Fact]
        public async Task UnAssign_DashboardNode_AdminUser_Ok()
        {
            var response = await httpClient.DeleteAsync("/Dashboards/5/Nodes/2");
            Assert.True(response.StatusCode == HttpStatusCode.NoContent);

            response = await httpClient.GetAsync("/Dashboards/5");
            var strResult = await response.Content.ReadAsStringAsync();
            var dashboard = JsonConvert.DeserializeObject<DashboardViewModel>(strResult);
            Assert.Equal(2, dashboard.NodeIds.Count);
            Assert.Contains(dashboard.NodeIds, i => i == 1);
            Assert.Contains(dashboard.NodeIds, i => i == 3);
        }

        [Fact]
        public async Task Assign_DashboardNode_AdminUser_Ok()
        {
            var response = await httpClient.PostAsync("/Dashboards/5/Nodes/4", null);
            Assert.True(response.StatusCode == HttpStatusCode.NoContent);

            response = await httpClient.GetAsync("/Dashboards/5");
            var strResult = await response.Content.ReadAsStringAsync();
            var dashboard = JsonConvert.DeserializeObject<DashboardViewModel>(strResult);
            Assert.Equal(4, dashboard.NodeIds.Count);
            Assert.Contains(dashboard.NodeIds, i => i == 1);
            Assert.Contains(dashboard.NodeIds, i => i == 2);
            Assert.Contains(dashboard.NodeIds, i => i == 3);
            Assert.Contains(dashboard.NodeIds, i => i == 4);
        }

        [Fact]
        public async Task Assign_DashboardHub_AdminUser_Ok()
        {
            var response = await httpClient.PostAsync("/Dashboards/1/Hub", null);
            Assert.True(response.StatusCode == HttpStatusCode.NoContent);

            response = await httpClient.GetAsync("/Dashboards/1");
            var strResult = await response.Content.ReadAsStringAsync();
            var dashboard = JsonConvert.DeserializeObject<DashboardViewModel>(strResult);
            Assert.Equal(1, dashboard.HubId);
        }

        [Fact]
        public async Task UnAssign_DashboardHub_AdminUser_Ok()
        {
            var response = await httpClient.DeleteAsync("/Dashboards/2/Hub");
            Assert.True(response.StatusCode == HttpStatusCode.NoContent);

            response = await httpClient.GetAsync("/Dashboards/2");
            var strResult = await response.Content.ReadAsStringAsync();
            var dashboard = JsonConvert.DeserializeObject<DashboardViewModel>(strResult);
            Assert.Null(dashboard.HubId);
        }

        [Fact]
        public async Task Update_Dashboard_AdminUser_Ok()
        {
            // Act
            var dashboardDto = new DashboardCreateModelView
            {
                DashboardId = 5,
                Title = new Dictionary<string, string> {{"IT", "Titolo5Edit"}, {"ES", "Espan Title"}},
                DashboardConfig =
                    @"[[{""type"":""text"",""value"":{""gb"":""<p>text text text</p>"",""it"":""<p>testo testo testo</p>"",""fr"":""<p>texte&nbsp;texte&nbsp;texte</p>""},""widthPercentage"":100,""showTitle"":true,""enableFilters"":true}]]",
                ViewIds = new List<int> {2, 3, 8},
                Weight = 2
            };

            // Assert
            var response = await httpClient.PutAsync("/Dashboards/5",
                new StringContent(JsonConvert.SerializeObject(dashboardDto), Encoding.UTF8, "application/json"));
            Assert.True(response.StatusCode == HttpStatusCode.NoContent);


            response = await httpClient.GetAsync("/Dashboards/5");
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());

            var strResult = await response.Content.ReadAsStringAsync();
            var dashboard = JsonConvert.DeserializeObject<DashboardViewModel>(strResult);


            Assert.Equal(dashboardDto.DashboardConfig, dashboard.DashboardConfig);
            Assert.Equal(dashboardDto.DashboardId, dashboard.DashboardId);
            Assert.Equal(4, dashboard.Weight); //because PUT don't edit Weight
            Assert.Equal(2, dashboardDto.Title.Count);
            Assert.Contains(dashboard.Title, i => i.Key.Equals("it", StringComparison.InvariantCultureIgnoreCase));
            Assert.Contains(dashboard.Title, i => i.Key.Equals("es", StringComparison.InvariantCultureIgnoreCase));
            Assert.Contains(dashboard.Title,
                i => i.Value.Equals("Titolo5Edit", StringComparison.InvariantCultureIgnoreCase));
            Assert.Contains(dashboard.Title,
                i => i.Value.Equals("Espan Title", StringComparison.InvariantCultureIgnoreCase));
            Assert.Equal(2, dashboard.ViewIds.Count);
            Assert.Contains(dashboard.ViewIds, i => i == 2);
            Assert.Contains(dashboard.ViewIds, i => i == 3);
            //Assert.Contains(dashboard.ViewIds, i => i == 8);DISCARD BEACUSE view 8 don't assign to current user
        }

        [Fact]
        public async Task Update_Dashboard_NormalUser_Deny()
        {
            // Act
            var dashboardDto = new DashboardCreateModelView
            {
                DashboardId = 5,
                Title = new Dictionary<string, string> {{"IT", "Titolo5Edit"}, {"ES", "Espan Title"}},
                DashboardConfig =
                    @"[[{""type"":""text"",""value"":{""gb"":""<p>text text text</p>"",""it"":""<p>testo testo testo</p>"",""fr"":""<p>texte&nbsp;texte&nbsp;texte</p>""},""widthPercentage"":100,""showTitle"":true,""enableFilters"":true,""filterDimension"":null}]]",
                ViewIds = new List<int> {2, 3},
                Weight = 2
            };

            // Assert
            var token = UtilityTest.GetUserToken(httpClient).Result.Token;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.PutAsync("/Dashboards/5",
                new StringContent(JsonConvert.SerializeObject(dashboardDto), Encoding.UTF8, "application/json"));
            Assert.True(response.StatusCode == HttpStatusCode.Forbidden);
        }
    }
}