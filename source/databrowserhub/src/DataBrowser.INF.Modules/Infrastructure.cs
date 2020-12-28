using DataBrowser.AC.Utility;
using DataBrowser.AuthenticationAuthorization;
using DataBrowser.DB.Dapper;
using DataBrowser.DB.EFCore.Context;
using DataBrowser.DB.EFCore.Utility;
using DataBrowser.Domain.Entities.Users;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Entities.SQLite;
using DataBrowser.INF.DataflowDataCache;
using DataBrowser.INF.Mail;
using DataBrowser.INF.MemoryCache;
using DataBrowser.INF.SdmxCache;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Cache;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Database;
using DataBrowser.Interfaces.Mail;
using DataBrowser.Interfaces.Updater;
using DataBrowser.Interfaces.Workers;
using DataBrowser.Updater;
using EndPointConnector.Interfaces;
using EndPointConnector.Interfaces.JsonStat;
using EndPointConnector.Interfaces.Sdmx;
using EndPointConnector.Interfaces.Spod;
using EndPointConnector.JsonStatParser.Factories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sister.EndPointConnector.Sdmx;
using Sister.EndPointConnector.Spod;
using System;
using System.IO;
using WSHUB.HostedService.Workers;

namespace DataBrowser.INF.Modules
{
    public class Infrastructure
    {
        public static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration, string contentRootPath)
        {
            var databaseConfig = configuration.GetSection("Database").Get<DatabaseConfig>();

            services.AddTransient(sp =>
                ConnectionFactory.CreateDbConnection(databaseConfig));
            services.AddTransient(sp =>
                ConnectionFactory.CreateGeometryDbConnection(configuration.GetSection("GeometryDatabase")
                    .Get<GeometryDatabaseConfig>()));

            if (databaseConfig.DbType != null &&
                databaseConfig.DbType.Equals("SQLite", StringComparison.InvariantCultureIgnoreCase))
            {
                var stringBuilder = new SqliteConnectionStringBuilder
                {
                    ConnectionString = databaseConfig.ConnectionString
                };

                stringBuilder.DataSource = Path.Combine(contentRootPath, stringBuilder.DataSource);

                services.AddDbContext<DatabaseContext>(option =>
                    option.UseLazyLoadingProxies()
                        .UseSqlite(stringBuilder.ConnectionString));
                services.AddDbContext<DataBrowserUpdaterContext>(option =>
                    option.UseSqlite(stringBuilder.ConnectionString));
                services.AddScoped<IDatabaseBackup, SQLiteDatabaseBackup>();
            }
            else if (databaseConfig.DbType != null &&
                    databaseConfig.DbType.Equals("SqlServer", StringComparison.InvariantCultureIgnoreCase))
            {
                services.AddDbContext<DatabaseContext>(option =>
                    option.UseLazyLoadingProxies()
                        .UseSqlServer(databaseConfig.ConnectionString));
                services.AddDbContext<DataBrowserUpdaterContext>(option =>
                    option.UseSqlServer(databaseConfig.ConnectionString));
            }

        }

        public static void ConfigureIdentity(IServiceCollection services, AuthenticationConfig authenticationConfig)
        {
            services.AddTransient<IPasswordValidator<ApplicationUser>, CustomPasswordPolicy>();
            services.AddTransient<IUserValidator<ApplicationUser>, CustomUsernameEmailPolicy>();

            services.AddIdentity<ApplicationUser, ApplicationRole>(
                opts =>
                {
                    opts.User.RequireUniqueEmail = true;
                    opts.Password.RequiredLength = authenticationConfig.UserPolicy.PasswordRequiredLength;
                    opts.Password.RequireNonAlphanumeric =
                        authenticationConfig.UserPolicy.PasswordRequireNonAlphanumeric;
                    opts.Password.RequireLowercase = authenticationConfig.UserPolicy.PasswordRequireLowercase;
                    opts.Password.RequireUppercase = authenticationConfig.UserPolicy.PasswordRequireUppercase;
                    opts.Password.RequireDigit = authenticationConfig.UserPolicy.PasswordRequireDigit;
                })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<DatabaseContext>();
        }

        public static void ConfigureOthersInfrastructure(IServiceCollection services)
        {
            //Cache
            services.AddScoped<IDataBrowserMemoryCache, DataBrowserMemoryCache>();
            services.AddScoped<IDataflowDataCache, SqliteDataCache>();


            //Repository
            services.AddTransient(typeof(IRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEndPointCache, SdmxCacheEngine>();
            services.AddScoped<ISdmxCache, SdmxCacheEngine>();
            services.AddScoped<ISpodEndPointFactory, SpodEndPointManager>();
            services.AddScoped<ISdmxEndPointFactory, SdmxEndPointManager>();
            services.AddScoped<ISdmxSpecificEndPointFactory, SdmxEndPointManager>();

            //JsonStat Convert
            services.AddScoped<IFromSdmxJsonToJsonStatConverterFactory, FromSdmxJsonToJsonStatConverterFactory>();
            services.AddScoped<IFromSdmxXmlToJsonStatConverterFactory, FromSdmxXmlToJsonStatConverterFactory>();
            services.AddScoped<IFromJsonStatToJsonStatConverterFactory, FromJsonStatToJsonStatConverterFactory>();

            //Generic Database
            services.AddScoped<IGeometryRepository, GeometryRepository>();
            services.AddScoped<IClearAuthDb, ClearAuthDb>();

            //Worker
            services.AddSingleton<IDashboardCacheGeneratorWorker, DashboardDataCacheGeneratorWorker>();
            services.AddSingleton<IDataflowCacheGeneratorWorker, DataflowDataCacheGeneratorWorker>();

            //Updater
            services.AddScoped<IUpdater, DataBrowserUpdater>();
        }
    }
}
