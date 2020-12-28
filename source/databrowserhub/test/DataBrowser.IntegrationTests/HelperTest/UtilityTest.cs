using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataBrowser.DB.EFCore.Context;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Entities.Users;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Entities.SQLite;
using DataBrowser.Interfaces.Constants;
using DataBrowser.Interfaces.Dto.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DataBrowser.IntegrationTests.HelperTest
{
    public class UtilityTest
    {
        public static string DataflowDataCacheFileName => "DB/DataflowDataCache_Node_-1.sqlite";

        public static void RemoveDataflowDataCacheIfExist()
        {
            if (File.Exists(DataflowDataCacheFileName))
            {
                var maxCount = 10;
                var fileInfo = new FileInfo(DataflowDataCacheFileName);
                while (IsFileLocked(fileInfo) && maxCount > 0)
                {
                    Thread.Sleep(100);
                    maxCount--;
                }

                File.Delete(DataflowDataCacheFileName);
            }
        }

        public static bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }

        public static async Task<UserAuthenticatedResult> GetAdminToken(HttpClient httpClient)
        {
            var jsonData = @"{
    ""Email"": ""admin@admindatabworser.com"",
    ""Password"": ""RGF0YUJyb3dzZXIxIQ==""
}";
            var response = await httpClient.PostAsync("/Auth/Token",
                new StringContent(jsonData, Encoding.UTF8, "application/json"));

            var strResult = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserAuthenticatedResult>(strResult);
        }

        public static async Task<UserAuthenticatedResult> GetUserToken(HttpClient httpClient)
        {
            var jsonData = @"{
    ""Email"": ""User1@User1.com"",
    ""Password"": ""RGF0YUJyb3dzZXIxITE=""
}";
            var response = await httpClient.PostAsync("/Auth/Token",
                new StringContent(jsonData, Encoding.UTF8, "application/json"));

            var strResult = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserAuthenticatedResult>(strResult);
        }

        public static async Task<UserAuthenticatedResult> GetSeconAdminToken(HttpClient httpClient)
        {
            var jsonData = @"{
    ""Email"": ""User3@User3.com"",
    ""Password"": ""RGF0YUJyb3dzZXIxITM=""
}";
            var response = await httpClient.PostAsync("/Auth/Token",
                new StringContent(jsonData, Encoding.UTF8, "application/json"));

            var strResult = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserAuthenticatedResult>(strResult);
        }

        public static async Task SeedDatabase(IServiceProvider _serviceProvider)
        {
            ////START Restore Original DB
            ////var dbFileWorking = "DB/DataBrowserIntegrationTestDB.sqlite";
            ////var backUpFile = "DB/DataBrowserIntegrationTestDB.StartPoint.sqlite";
            ////if (File.Exists(dbFileWorking))
            ////{
            ////    var maxCount = 10;
            ////    var fileInfo = new FileInfo(dbFileWorking);
            ////    while (IsFileLocked(fileInfo) && maxCount > 0)
            ////    {
            ////        Thread.Sleep(100);
            ////        maxCount--;
            ////    }
            ////}

            ////File.Copy(backUpFile, dbFileWorking, true);
            ////END Restore Original DB
            ///
            using (var scope = _serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                await createDefaultUsersAsync(userManager);
                await createDefaultNodesAsync(scope.ServiceProvider.GetRequiredService<IRepository<Node>>());
                await createDefaultViews(scope.ServiceProvider.GetRequiredService<IRepository<ViewTemplate>>());
                await createDefaultTemplates(scope.ServiceProvider.GetRequiredService<IRepository<ViewTemplate>>());
                await createDefaultDashboard(scope.ServiceProvider.GetRequiredService<IRepository<Dashboard>>());
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private static async Task createDefaultUsersAsync(UserManager<ApplicationUser> userManager)
        {
            var defaultUser = new ApplicationUser
            {
                UserName = "User1", Email = "User1@User1.com", EmailConfirmed = true, PhoneNumberConfirmed = true,
                IsSuperAdmin = true
            };
            await userManager.CreateAsync(defaultUser, UserAndGroup.SuperAdminPassword + "1");
            await userManager.AddToRoleAsync(defaultUser, UserAndGroup.Roles.User.ToString());

            defaultUser = new ApplicationUser
            {
                UserName = "User2", Email = "User2@User2.com", EmailConfirmed = true, PhoneNumberConfirmed = true,
                IsSuperAdmin = true
            };
            await userManager.CreateAsync(defaultUser, UserAndGroup.SuperAdminPassword + "2");
            await userManager.AddToRoleAsync(defaultUser, UserAndGroup.Roles.User.ToString());

            defaultUser = new ApplicationUser
            {
                UserName = "User3", Email = "User3@User3.com", EmailConfirmed = true, PhoneNumberConfirmed = true,
                IsSuperAdmin = true
            };
            await userManager.CreateAsync(defaultUser, UserAndGroup.SuperAdminPassword + "3");
            await userManager.AddToRoleAsync(defaultUser, UserAndGroup.Roles.Administrator.ToString());
        }

        private static async Task createDefaultNodesAsync(IRepository<Node> repository)
        {
            var nodeDto = new NodeDto
            {
                Type = "SDMX-REST",
                Code = "TestFede3",
                Active = true,
                Logo = "Logoooo",
                EndPoint = "http://vmcorsow701/ISTAT_TOOLKIT/nsiws_7.9.0_core/rest/",
                Order = 1,
                EnableHttpAuth = true,
                AuthHttpUsername = "admin",
                AuthHttpPassword = "admin",
                EnableProxy = false,
                UseProxySystem = false,
                ProxyAddress = null,
                ProxyPort = 0,
                ProxyUsername = null,
                ProxyPassword = null,
                BackgroundMediaURL = "./images/istat/istat-background.jpg",
                EmptyCellDefaultValue = "",
                DefaultView = "",
                ShowDataflowUncategorized = false,
                CriteriaSelectionMode = "",
                LabelDimensionTerritorials = null,
                CategorySchemaExcludes = null,
                EndPointFormatSupported = null,
                DecimalNumber = null,
                CatalogNavigationMode = null,
                AuthHttpDomain = null,
                TtlCatalog = null,
                TtlDataflow = null,
                ShowDataflowNotInProduction = false,
                Title = new Dictionary<string, string> {{"IT", "Titolo"}, {"EN", "Title"}},
                Slogan = new Dictionary<string, string> {{"IT", "Slogan italiano"}},
                Description = new Dictionary<string, string>
                    {{"IT", "description in itaaaaaaaaaa"}, {"EN", "description englishhhhh"}}
            };
            var entity = Node.CreateNode(nodeDto);
            repository.Add(entity);
            await repository.UnitOfWork.SaveChangesAsync();

            nodeDto = new NodeDto
            {
                Type = "SDMX-REST",
                Code = "NodeTest",
                Active = false,
                Logo = "logo1",
                EndPoint = "http://vmcorsow701/ISTAT_TOOLKIT/nsiws_7.9.0_core/SdmxRegistryService",
                Order = 3,
                EnableHttpAuth = false,
                AuthHttpUsername = null,
                AuthHttpPassword = null,
                EnableProxy = false,
                UseProxySystem = false,
                ProxyAddress = null,
                ProxyPort = 0,
                ProxyUsername = null,
                ProxyPassword = null,
                BackgroundMediaURL = "./images/inps/inps-background.jpg",
                EmptyCellDefaultValue = "",
                DefaultView = "",
                ShowDataflowUncategorized = false,
                CriteriaSelectionMode = "",
                LabelDimensionTerritorials = null,
                CategorySchemaExcludes = null,
                EndPointFormatSupported = null,
                DecimalNumber = null,
                CatalogNavigationMode = null,
                AuthHttpDomain = null,
                TtlCatalog = null,
                TtlDataflow = null,
                ShowDataflowNotInProduction = false,
                Title = new Dictionary<string, string>
                    {{"IT", "Titolo in italiano"}, {"EN", "English title"}, {"ES", "Esp title"}}
            };
            entity = Node.CreateNode(nodeDto);
            repository.Add(entity);
            await repository.UnitOfWork.SaveChangesAsync();

            nodeDto = new NodeDto
            {
                Type = "SDMX-SOAPv21",
                Code = "Other",
                Active = true,
                Logo = "logo prova",
                EndPoint = "http://vmcorsow701/ISTAT_TOOLKIT/nsiws_7.9.0_core/rest/",
                Order = 2,
                EnableHttpAuth = true,
                AuthHttpUsername = "httpadmin",
                AuthHttpPassword = "httpuser",
                EnableProxy = true,
                UseProxySystem = false,
                ProxyAddress = "proxyUrl",
                ProxyPort = 8080,
                ProxyUsername = "userproxy",
                ProxyPassword = "passpworxy",
                BackgroundMediaURL = "./images/sdg/sdg-background.png",
                EmptyCellDefaultValue = "",
                DefaultView = "",
                ShowDataflowUncategorized = false,
                CriteriaSelectionMode = "",
                LabelDimensionTerritorials = null,
                CategorySchemaExcludes = null,
                EndPointFormatSupported = null,
                DecimalNumber = null,
                CatalogNavigationMode = null,
                AuthHttpDomain = null,
                TtlCatalog = null,
                TtlDataflow = null,
                ShowDataflowNotInProduction = false,
                Title = new Dictionary<string, string>
                    {{"IT", "Titolo in italiano"}, {"EN", "English title"}, {"ES", "Esp title"}},
                Slogan = new Dictionary<string, string> {{"EN", "Slogan ENG2"}},
                Description = new Dictionary<string, string> {{"EN", "description englishhhhh222"}}
            };
            entity = Node.CreateNode(nodeDto);
            repository.Add(entity);
            await repository.UnitOfWork.SaveChangesAsync();

            nodeDto = new NodeDto
            {
                Type = "SDMX-REST",
                Code = "Node4",
                Active = false,
                Logo = "testlogo",
                EndPoint = "http://vmcorsow701/ISTAT_TOOLKIT/nsiws_7.9.0_core/rest/",
                Order = 4,
                EnableHttpAuth = true,
                AuthHttpUsername = "",
                AuthHttpPassword = "",
                EnableProxy = true,
                UseProxySystem = true,
                ProxyAddress = "proxyUrl",
                ProxyPort = 8080,
                ProxyUsername = "",
                ProxyPassword = "",
                BackgroundMediaURL = "./images/sdg/sdg-background.png",
                EmptyCellDefaultValue = "",
                DefaultView = "",
                ShowDataflowUncategorized = false,
                CriteriaSelectionMode = "",
                LabelDimensionTerritorials = null,
                CategorySchemaExcludes = null,
                EndPointFormatSupported = null,
                DecimalNumber = null,
                CatalogNavigationMode = null,
                AuthHttpDomain = null,
                TtlCatalog = null,
                TtlDataflow = null,
                ShowDataflowNotInProduction = false,
                Title = new Dictionary<string, string> {{"IT", "Titolo in italiano2"}}
            };
            entity = Node.CreateNode(nodeDto);
            repository.Add(entity);
            await repository.UnitOfWork.SaveChangesAsync();

            nodeDto = new NodeDto
            {
                Type = "SDMX-SOAPv21",
                Code = "NodeFull",
                Active = true,
                Logo = "Logoooo",
                EndPoint = "http://vmcorsow701/ISTAT_TOOLKIT/nsiws_7.9.0_core/SdmxRegistryService",
                Order = 5,
                EnableHttpAuth = true,
                AuthHttpUsername = null,
                AuthHttpPassword = null,
                EnableProxy = true,
                UseProxySystem = true,
                ProxyAddress = null,
                ProxyPort = 0,
                ProxyUsername = null,
                ProxyPassword = null,
                BackgroundMediaURL = "./images/sdg/sdg-background.png",
                EmptyCellDefaultValue = "",
                DefaultView = "",
                ShowDataflowUncategorized = false,
                CriteriaSelectionMode = "",
                LabelDimensionTerritorials = null,
                CategorySchemaExcludes = null,
                EndPointFormatSupported = null,
                DecimalNumber = null,
                CatalogNavigationMode = null,
                AuthHttpDomain = null,
                TtlCatalog = null,
                TtlDataflow = null,
                ShowDataflowNotInProduction = false,
                Slogan = new Dictionary<string, string> {{"EN", "Slogan ENG3"}, {"ES", "Slogan ESSSSS3"}},
                Description = new Dictionary<string, string>
                    {{"EN", "description englishhhhh node 5"}, {"IT", "description IT 5node"}}
            };
            entity = Node.CreateNode(nodeDto);
            repository.Add(entity);

            await repository.UnitOfWork.SaveChangesAsync();
        }

        private static async Task createDefaultViews(IRepository<ViewTemplate> repository)
        {
            var viewTemplateDto = new ViewTemplateDto
            {
                Title = new Dictionary<string, string> {{"IT", "Titolo"}, {"EN", "Title"}},
                DatasetId = "datasetTestId",
                Type = ViewTemplateType.View,
                DefaultView = "ViewDefault",
                Criteria = null,
                Layouts = "layoutsssss",
                HiddenDimensions = "HiddenDimensionsssss",
                BlockAxes = true,
                EnableCriteria = false,
                EnableVariation = true,
                DecimalNumber = 2,
                DecimalSeparator = new Dictionary<string, string> {{"IT", "."}, {"EN", ","}},
                NodeId = 1,
                UserId = 1
            };
            var entity = await ViewTemplate.CreateViewTemplateAsync(viewTemplateDto, null);
            repository.Add(entity.ValidateObject);
            await repository.UnitOfWork.SaveChangesAsync();


            viewTemplateDto = new ViewTemplateDto
            {
                Title = new Dictionary<string, string> {{"IT", "Titolo2"}, {"EN", "Title2"}},
                DatasetId = "datasetTestId2",
                Type = ViewTemplateType.View,
                DefaultView = "ViewDefault2",
                Criteria = null,
                Layouts = "layoutsssss2",
                HiddenDimensions = "HiddenDimensionsssss2",
                BlockAxes = true,
                EnableCriteria = true,
                EnableVariation = true,
                DecimalNumber = 4,
                DecimalSeparator = new Dictionary<string, string> {{"IT", ","}, {"EN", "."}},
                NodeId = 1,
                UserId = 1
            };
            entity = await ViewTemplate.CreateViewTemplateAsync(viewTemplateDto, null);
            repository.Add(entity.ValidateObject);
            await repository.UnitOfWork.SaveChangesAsync();

            viewTemplateDto = new ViewTemplateDto
            {
                Title = new Dictionary<string, string> {{"IT", "Titolo3"}},
                DatasetId = "datasetTestId3",
                Type = ViewTemplateType.View,
                DefaultView = "ViewDefault3",
                Criteria = null,
                Layouts = "layoutsssss3",
                HiddenDimensions = "HiddenDimensionsssss3",
                BlockAxes = true,
                EnableCriteria = true,
                EnableVariation = true,
                DecimalNumber = 4,
                DecimalSeparator = new Dictionary<string, string> {{"IT", ","}, {"EN", "."}},
                NodeId = 3,
                UserId = 1
            };
            entity = await ViewTemplate.CreateViewTemplateAsync(viewTemplateDto, null);
            repository.Add(entity.ValidateObject);
            await repository.UnitOfWork.SaveChangesAsync();

            viewTemplateDto = new ViewTemplateDto
            {
                Title = new Dictionary<string, string> {{"EN", "Titolo4"}},
                DatasetId = "datasetTestId4",
                Type = ViewTemplateType.View,
                DefaultView = "ViewDefault4",
                Criteria = null,
                Layouts = "layoutsssss4",
                HiddenDimensions = "HiddenDimensionsssss4",
                BlockAxes = false,
                EnableCriteria = true,
                EnableVariation = true,
                DecimalNumber = 4,
                DecimalSeparator = new Dictionary<string, string> {{"IT", ","}, {"EN", "."}},
                NodeId = 2,
                UserId = 1
            };
            entity = await ViewTemplate.CreateViewTemplateAsync(viewTemplateDto, null);
            repository.Add(entity.ValidateObject);
            await repository.UnitOfWork.SaveChangesAsync();

            viewTemplateDto = new ViewTemplateDto
            {
                Title = new Dictionary<string, string> {{"IT", "Titolo5"}},
                DatasetId = "datasetTestId5",
                Type = ViewTemplateType.View,
                DefaultView = "ViewDefault5",
                Criteria = null,
                Layouts = "layoutsssss5",
                HiddenDimensions = "HiddenDimensionsssss5",
                BlockAxes = false,
                EnableCriteria = false,
                EnableVariation = true,
                DecimalNumber = 1,
                DecimalSeparator = new Dictionary<string, string> {{"IT", "-"}, {"EN", "_"}},
                NodeId = 2,
                UserId = 2
            };
            entity = await ViewTemplate.CreateViewTemplateAsync(viewTemplateDto, null);
            repository.Add(entity.ValidateObject);
            await repository.UnitOfWork.SaveChangesAsync();

            viewTemplateDto = new ViewTemplateDto
            {
                Title = new Dictionary<string, string> {{"IT", "Titolo6"}, {"EN", "Titolo6"}},
                DatasetId = "datasetTestId6",
                Type = ViewTemplateType.View,
                DefaultView = "ViewDefault6",
                Criteria = null,
                Layouts = "layoutsssss6",
                HiddenDimensions = "HiddenDimensionsssss6",
                BlockAxes = false,
                EnableCriteria = false,
                EnableVariation = true,
                DecimalNumber = 2,
                DecimalSeparator = new Dictionary<string, string> {{"IT", "."}, {"EN", ","}},
                NodeId = 5,
                UserId = 3
            };
            entity = await ViewTemplate.CreateViewTemplateAsync(viewTemplateDto, null);
            repository.Add(entity.ValidateObject);
            await repository.UnitOfWork.SaveChangesAsync();

            viewTemplateDto = new ViewTemplateDto
            {
                Title = new Dictionary<string, string> {{"IT", "Titolo7"}, {"EN", "Titolo7"}},
                DatasetId = "datasetTestId7",
                Type = ViewTemplateType.View,
                DefaultView = "ViewDefault7",
                Criteria = null,
                Layouts = "layoutsssss7",
                HiddenDimensions = "HiddenDimensionsssss7",
                BlockAxes = false,
                EnableCriteria = false,
                EnableVariation = true,
                DecimalNumber = 1,
                DecimalSeparator = new Dictionary<string, string> {{"IT", "."}, {"EN", ","}},
                NodeId = 1,
                UserId = 2
            };
            entity = await ViewTemplate.CreateViewTemplateAsync(viewTemplateDto, null);
            repository.Add(entity.ValidateObject);
            await repository.UnitOfWork.SaveChangesAsync();

            viewTemplateDto = new ViewTemplateDto
            {
                Title = new Dictionary<string, string> {{"IT", "Titolo8"}, {"FR", "Titolo8"}},
                DatasetId = "datasetTestId8",
                Type = ViewTemplateType.View,
                DefaultView = "ViewDefault8",
                Criteria = null,
                Layouts = "layoutsssss8",
                HiddenDimensions = "HiddenDimensionsssss8",
                BlockAxes = false,
                EnableCriteria = false,
                EnableVariation = true,
                DecimalNumber = 2,
                DecimalSeparator = new Dictionary<string, string> {{"IT", "."}, {"EN", ","}},
                NodeId = 1,
                UserId = 4
            };
            entity = await ViewTemplate.CreateViewTemplateAsync(viewTemplateDto, null);
            repository.Add(entity.ValidateObject);
            await repository.UnitOfWork.SaveChangesAsync();
        }

        private static async Task createDefaultTemplates(IRepository<ViewTemplate> repository)
        {
            var viewTemplateDto = new ViewTemplateDto
            {
                Title = new Dictionary<string, string> {{"IT", "Titolo"}, {"EN", "Title"}},
                DatasetId = "datasetTestId",
                Type = ViewTemplateType.Template,
                DefaultView = "ViewDefault",
                Criteria = null,
                Layouts = "layoutsssss",
                HiddenDimensions = "HiddenDimensionsssss",
                BlockAxes = true,
                EnableCriteria = false,
                EnableVariation = true,
                DecimalNumber = 2,
                DecimalSeparator = new Dictionary<string, string> {{"IT", "."}, {"EN", ","}},
                NodeId = 1,
                UserId = null
            };
            var entity = await ViewTemplate.CreateViewTemplateAsync(viewTemplateDto, null);
            repository.Add(entity.ValidateObject);
            await repository.UnitOfWork.SaveChangesAsync();


            viewTemplateDto = new ViewTemplateDto
            {
                Title = new Dictionary<string, string> {{"IT", "Titolo2"}, {"EN", "Title2"}},
                DatasetId = "datasetTestId2",
                Type = ViewTemplateType.Template,
                DefaultView = "ViewDefault2",
                Criteria = null,
                Layouts = "layoutsssss2",
                HiddenDimensions = "HiddenDimensionsssss2",
                BlockAxes = true,
                EnableCriteria = true,
                EnableVariation = true,
                DecimalNumber = 4,
                DecimalSeparator = new Dictionary<string, string> {{"IT", ","}, {"EN", "."}},
                NodeId = 1,
                UserId = null
            };
            entity = await ViewTemplate.CreateViewTemplateAsync(viewTemplateDto, null);
            repository.Add(entity.ValidateObject);
            await repository.UnitOfWork.SaveChangesAsync();

            viewTemplateDto = new ViewTemplateDto
            {
                Title = new Dictionary<string, string> {{"IT", "Titolo3"}},
                DatasetId = "datasetTestId3",
                Type = ViewTemplateType.Template,
                DefaultView = "ViewDefault3",
                Criteria = null,
                Layouts = "layoutsssss3",
                HiddenDimensions = "HiddenDimensionsssss3",
                BlockAxes = true,
                EnableCriteria = true,
                EnableVariation = true,
                DecimalNumber = 4,
                DecimalSeparator = new Dictionary<string, string> {{"IT", ","}, {"EN", "."}},
                NodeId = 3,
                UserId = null
            };
            entity = await ViewTemplate.CreateViewTemplateAsync(viewTemplateDto, null);
            repository.Add(entity.ValidateObject);
            await repository.UnitOfWork.SaveChangesAsync();

            viewTemplateDto = new ViewTemplateDto
            {
                Title = new Dictionary<string, string> {{"EN", "Titolo4"}},
                DatasetId = "datasetTestId4",
                Type = ViewTemplateType.Template,
                DefaultView = "ViewDefault4",
                Criteria = null,
                Layouts = "layoutsssss4",
                HiddenDimensions = "HiddenDimensionsssss4",
                BlockAxes = false,
                EnableCriteria = true,
                EnableVariation = true,
                DecimalNumber = 4,
                DecimalSeparator = new Dictionary<string, string> {{"IT", ","}, {"EN", "."}},
                NodeId = 2,
                UserId = null
            };
            entity = await ViewTemplate.CreateViewTemplateAsync(viewTemplateDto, null);
            repository.Add(entity.ValidateObject);
            await repository.UnitOfWork.SaveChangesAsync();
        }

        private static async Task createDefaultDashboard(IRepository<Dashboard> repository)
        {
            var dashboardDto = new DashboardDto
            {
                Title = new Dictionary<string, string> {{"IT", "Titolo"}, {"EN", "Title"}},
                DashboardConfig = new DashboardDto.DashboardConfigItem[1][],
                ViewIds = new List<int> {1, 2},
                UserId = 1,
                Weight = 1
            };
            dashboardDto.DashboardConfig[0] = new[] {new DashboardDto.DashboardConfigItem {Value = 1}};
            var entity = await Dashboard.CreateDashboardAsync(dashboardDto, null);
            repository.Add(entity.ValidateObject);
            await repository.UnitOfWork.SaveChangesAsync();

            dashboardDto = new DashboardDto
            {
                Title = new Dictionary<string, string> {{"IT", "Titolo2"}, {"EN", "Title2"}},
                DashboardConfig = new DashboardDto.DashboardConfigItem[1][],
                ViewIds = new List<int> {2, 3},
                HubId = 1,
                UserId = 1,
                Weight = 2
            };
            dashboardDto.DashboardConfig[0] = new[] {new DashboardDto.DashboardConfigItem {Value = 2}};
            entity = await Dashboard.CreateDashboardAsync(dashboardDto, null);
            repository.Add(entity.ValidateObject);
            await repository.UnitOfWork.SaveChangesAsync();

            dashboardDto = new DashboardDto
            {
                Title = new Dictionary<string, string> {{"EN", "Title3"}},
                DashboardConfig = new DashboardDto.DashboardConfigItem[1][],
                ViewIds = new List<int> {8},
                HubId = 1,
                UserId = 4,
                Weight = 3
            };
            dashboardDto.DashboardConfig[0] = new[] {new DashboardDto.DashboardConfigItem {Value = 3}};
            entity = await Dashboard.CreateDashboardAsync(dashboardDto, null);
            repository.Add(entity.ValidateObject);
            await repository.UnitOfWork.SaveChangesAsync();

            dashboardDto = new DashboardDto
            {
                Title = new Dictionary<string, string> {{"IT", "Titolo4"}, {"EN", "Title4"}},
                DashboardConfig = new DashboardDto.DashboardConfigItem[1][],
                ViewIds = new List<int> {1},
                NodeIds = new List<int> {1, 2, 3},
                UserId = 1,
                Weight = 5
            };
            dashboardDto.DashboardConfig[0] = new[] {new DashboardDto.DashboardConfigItem {Value = 4}};
            entity = await Dashboard.CreateDashboardAsync(dashboardDto, null);
            repository.Add(entity.ValidateObject);
            await repository.UnitOfWork.SaveChangesAsync();

            dashboardDto = new DashboardDto
            {
                Title = new Dictionary<string, string> {{"IT", "Titolo5"}, {"EN", "Title5"}},
                DashboardConfig = new DashboardDto.DashboardConfigItem[1][],
                ViewIds = new List<int> {1},
                NodeIds = new List<int> {1, 2, 3},
                HubId = 1,
                UserId = 1,
                Weight = 4
            };
            dashboardDto.DashboardConfig[0] = new[] {new DashboardDto.DashboardConfigItem {Value = 5}};
            entity = await Dashboard.CreateDashboardAsync(dashboardDto, null);
            repository.Add(entity.ValidateObject);
            await repository.UnitOfWork.SaveChangesAsync();
        }
    }
}