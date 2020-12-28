using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBrowser.DB.EFCore.Context;
using DataBrowser.Entities.SQLite;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace DataBrowser.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.json");

            builder.ConfigureAppConfiguration((context, conf) => { conf.AddJsonFile(configPath); });
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<DatabaseContext>));

                services.Remove(descriptor);

                descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<DataBrowserUpdaterContext>));

                services.Remove(descriptor);

                var dbName = "InMemoryDbForTesting" + Guid.NewGuid().ToString().Replace("-", "");
                services.AddDbContext<DatabaseContext>(options =>
                {
                    options.UseLazyLoadingProxies().UseInMemoryDatabase(dbName);
                });
                dbName = "InMemoryVersionDbForTesting" + Guid.NewGuid().ToString().Replace("-", "");
                services.AddDbContext<DataBrowserUpdaterContext>(options =>
                {
                    options.UseLazyLoadingProxies().UseInMemoryDatabase(dbName);
                });
            });
        }
    }
}