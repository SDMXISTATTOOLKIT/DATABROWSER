using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Extensions.Logging;

namespace WSHUB
{
    public static class Program
    {
        private static readonly LoggerProviderCollection Providers = new LoggerProviderCollection();

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile(
                $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                true)
            .AddJsonFile("config/logconfig.json", false, true)
            .AddJsonFile(
                $"config/logconfig.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                true)
            .AddJsonFile("config/dashboarddatacachegenerator.json", true, true)
            .AddJsonFile(
                $"config/dashboarddatacachegenerator.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                true)
            .AddJsonFile("config/dataflowdatacachegenerator.json", true, true)
            .AddJsonFile(
                $"config/dataflowdatacachegenerator.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                true)
            .AddEnvironmentVariables()
            .Build();

        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            try
            {
                Log.Information("Getting the WSHUB running...");

                var host = CreateHostBuilder(args).Build();

                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureWebHostDefaults(webBuilder => { 
                    webBuilder.UseStartup<Startup>(); 
                })
                .UseSerilog(providers: Providers);
        }
    }
}