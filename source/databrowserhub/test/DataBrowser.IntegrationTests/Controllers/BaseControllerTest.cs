using System.Net.Http;
using DataBrowser.DB.EFCore.Context;
using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.Domain.Entities.Users;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Entities.SQLite;
using DataBrowser.IntegrationTests.HelperTest;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WSHUB;
using Xunit;

namespace DataBrowser.IntegrationTests.Controllers
{
    public class BaseControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        protected readonly CustomWebApplicationFactory<Startup> _factory;
        protected readonly HttpClient httpClient;

        public BaseControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            httpClient = _factory.CreateClient();

            using (var scope = _factory.Services.CreateScope())
            {
                var dataBrowserUpdaterContext = scope.ServiceProvider.GetRequiredService<DataBrowserUpdaterContext>();
                dataBrowserUpdaterContext.Database.EnsureDeleted();
                dataBrowserUpdaterContext.Database.EnsureCreated();

                var myDbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                myDbContext.Database.EnsureDeleted();
                myDbContext.Database.EnsureCreated();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                var hubRepository = scope.ServiceProvider.GetRequiredService<IRepository<Hub>>();
                ApplicationDbContextSeed.SeedEssentialsAsync(userManager, roleManager, hubRepository).Wait();

                UtilityTest.SeedDatabase(_factory.Services).Wait();
            }
        }
    }
}