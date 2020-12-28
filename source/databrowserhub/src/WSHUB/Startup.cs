using AutoMapper;
using DataBrowser.AC;
using DataBrowser.AC.Middleware;
using DataBrowser.AC.Modules;
using DataBrowser.AuthenticationAuthorization;
using DataBrowser.AuthenticationAuthorization.Filters;
using DataBrowser.AuthenticationAuthorization.Handler;
using DataBrowser.AuthenticationAuthorization.Policy;
using DataBrowser.Domain.Entities.Users;
using DataBrowser.Domain.Serialization;
using DataBrowser.Entities.SQLite;
using DataBrowser.INF.Modules;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Constants;
using EndPointConnector.Interfaces.Sdmx.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WSHUB.Filters;
using WSHUB.HostedService.Workers;
using WSHUB.Middleware;
using WSHUB.Models;

namespace WSHUB
{
    public class Startup
    {
        public Startup(IHostEnvironment hostEnvironment)
        {
            Configuration = Program.Configuration;
            HostEnvironment = hostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostEnvironment HostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Compression
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });
            services.AddResponseCompression(options =>
            {
                IEnumerable<string> MimeTypes = new[]
                {
                    "application/zip",
                    "image/png",
                    "image/jpeg",
                    "video/mp4"
                };
                options.EnableForHttps = true;
                options.ExcludedMimeTypes = MimeTypes;
                options.Providers.Add<GzipCompressionProvider>();
            });
            //END Compression

            services.AddMemoryCache();

            services.AddSingleton<IConfiguration>(Configuration);

            //Read appsettings.json
            services.Configure<EndPointAppConfig>(Configuration.GetSection("EndPointConnector"));
            services.Configure<EndPointAppConfig.Noderepository>(
                Configuration.GetSection("EndPointConnector:NodeRepository"));
            services.Configure<SdmxCacheConfig>(Configuration.GetSection("SDMXCache"));
            services.Configure<MemoryCacheConfig>(Configuration.GetSection("MemoryCache"));
            services.Configure<DataflowDataCacheConfig>(Configuration.GetSection("DataflowDataCache"));
            services.Configure<AuthenticationConfig>(Configuration.GetSection("Authentication"));
            services.Configure<DatabaseConfig>(Configuration.GetSection("Database"));
            services.Configure<GeneralConfig>(Configuration.GetSection("General"));
            services.Configure<DataflowDataCacheGeneratorWorkerConfig>(Configuration.GetSection("DataflowDataCacheGenerator"));
            services.Configure<DashboardDataCacheGeneratorWorkerConfig>(Configuration.GetSection("DashboardDataCacheGenerator"));
            services.Configure<MailConfig>(Configuration.GetSection("Mail"));
            services.Configure<SchedulerConfig>(Configuration.GetSection("Scheduler"));
            //END Read appsettings.json

            services.AddHostedService<MigratorDBHostedService>();
            services.AddHostedService<SchedulerHostedService>();

            //Database
            Infrastructure.ConfigureDatabase(services, Configuration, HostEnvironment.ContentRootPath);

            //Authorization Authentication
            var authenticationConfig = Configuration.GetSection("Authentication").Get<AuthenticationConfig>();
            ApplicationCore.ConfigureAuthorizationHandler(services, authenticationConfig);
            Infrastructure.ConfigureIdentity(services, authenticationConfig);
            ApplicationCore.ConfigureAuthorizationAuthentication(services, authenticationConfig);

            //Validation Rules
            var validationRulesConfig = Configuration.GetSection("General:ValidationRules").Get<ValidationRulesConfig>();
            ApplicationCore.ConfigureValidationRules(services, validationRulesConfig);

            //CORS
            ApplicationCore.ConfigureCORS(services, Configuration);

            services.AddTransient<FileResultFilter>();
            services.AddControllers(options =>
            {
                options.Filters.AddService<FileResultFilter>();
                //options.ModelBinderProviders.Insert(0, new HeaderNodeModelBinderProvider()); //EXAMPLE FOR REGISTER BINDER
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = DataBrowserJsonSerializer.Settings.ContractResolver;
                options.SerializerSettings.NullValueHandling = DataBrowserJsonSerializer.Settings.NullValueHandling;
                options.SerializerSettings.Formatting = Formatting.None;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });


            services.AddHttpClient("default", c =>
            {
                c.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                c.Timeout = TimeSpan.FromMinutes(120);
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                Proxy = WebRequest.DefaultWebProxy,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            });


            services.AddHttpContextAccessor();
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" }));
            services.AddSwaggerGenNewtonsoftSupport();

            //Register Automapper
            services.AddAutoMapper(typeof(APIMapperConfiguration), typeof(MappingProfileConfiguration));

            ApplicationCore.ConfigureOthersApplicationCore(services);
            Infrastructure.ConfigureOthersInfrastructure(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            app.UseSerilogRequestLogging(
            //    options =>
            //{
            //    // Customize the message template
            //    options.MessageTemplate = "Handled {RequestPath}";

            //    // Emit debug-level events instead of the defaults
            //    //options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;

            //    // Attach additional properties to the request completion event
            //    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            //    {
            //        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            //        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            //    };
            //}
            );

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseResponseCompression();
            app.UseRouting();


            var corsConfig = Configuration.GetSection("General:CORS").Get<CORSConfig>();
            if (corsConfig.Enable) app.UseCors("AllowAll");


            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware(typeof(TrackerAnonymousUserMiddleware)); //Must be register after UseAuthorization


            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("v1/swagger.json", "My API V1"); });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

    }
}