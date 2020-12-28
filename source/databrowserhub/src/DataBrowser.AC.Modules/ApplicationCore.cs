using DataBrowser.AC.Adapter;
using DataBrowser.AC.Caches;
using DataBrowser.AC.EndPointConnector;
using DataBrowser.AC.Workers;
using DataBrowser.AuthenticationAuthorization;
using DataBrowser.AuthenticationAuthorization.Filters;
using DataBrowser.AuthenticationAuthorization.Handler;
using DataBrowser.AuthenticationAuthorization.Policy;
using DataBrowser.Command.Nodes;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Specifications.Rules;
using DataBrowser.DomainServices;
using DataBrowser.DomainServices.Interfaces;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Cache;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Constants;
using DataBrowser.Interfaces.Workers;
using DataBrowser.Query.Nodes;
using DataBrowser.Services;
using DataBrowser.Services.Interfaces;
using DataBrowser.Specifications.ViewTemplates.Rules;
using DataBrowser.Subcribers;
using DataBrowser.UseCase;
using EndPointConnector.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataBrowser.AC.Modules
{
    public class ApplicationCore
    {
        public static void ConfigureValidationRules(IServiceCollection services, ValidationRulesConfig validationRulesConfig)
        {
            if (validationRulesConfig == null)
            {
                return;
            }

            if (validationRulesConfig.View == null || //Default Rule
                    validationRulesConfig.View.Any(i => i.Equals("UniqueNamaForUser", StringComparison.InvariantCultureIgnoreCase)))
            {
                services.AddScoped<IRuleSpecification<ViewTemplateDto>, NameRuleSpecification>();
            }
        }

        public static void ConfigureCORS(IServiceCollection services, IConfiguration configuration)
        {
            var corsConfig = configuration.GetSection("General:CORS").Get<CORSConfig>();
            if (corsConfig != null && corsConfig.Enable)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll",
                        builder =>
                        {
                            builder.SetIsOriginAllowed(i => true)
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials()
                                .WithExposedHeaders("OperationGuid")
                                .WithExposedHeaders("UserGuid")
                                .WithExposedHeaders("BackEndTimers")
                                .WithExposedHeaders("Token-Expired")
                                .WithExposedHeaders("ItemsCount");
                        });
                });
            }
        }

        public static void ConfigureAuthorizationHandler(IServiceCollection services, AuthenticationConfig authenticationConfig)
        {
            if (!authenticationConfig.IsActive)
            {
                services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();
            }

            services.AddTransient<IAuthorizationHandler, AllowManageCacheHandler>();
            services.AddTransient<IAuthorizationHandler, AllowManageConfigHandler>();
            services.AddTransient<IAuthorizationHandler, AllowManageTemplateHandler>();
            services.AddTransient<IAuthorizationHandler, AllowManageViewHandler>();
            services.AddTransient<IAuthorizationHandler, AllowUploadFileHandler>();
            services.AddTransient<IAuthorizationHandler, ServiceUserPolicyHandler>();
            services.AddTransient<IAuthorizationHandler, AuthenticatedUserHandler>();
        }

        public static void ConfigureAuthorizationAuthentication(IServiceCollection services, AuthenticationConfig authenticationConfig)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidAudience = authenticationConfig.Audience,
                        ValidIssuer = authenticationConfig.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationConfig.Key)),
                        ClockSkew = TimeSpan.Zero
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }

                            return Task.CompletedTask;
                        },
                        OnTokenValidated = validationContext =>
                        {
                            validationContext.Success();
                            return Task.CompletedTask;
                        }
                    };
                });


            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyName.AuthenticatedUser,
                    policy => { policy.AddRequirements(new AuthenticatedUserPolicy()); });
                options.AddPolicy(PolicyName.ServiceUser,
                    policy => { policy.AddRequirements(new ServiceUserPolicy()); });
                options.AddPolicy(PolicyName.ManageCache,
                    policy => { policy.AddRequirements(new AllowManageCachePolicy()); });
                options.AddPolicy(PolicyName.ManageConfig,
                    policy => { policy.AddRequirements(new AllowManageConfigPolicy()); });
                options.AddPolicy(PolicyName.ManageTemplate,
                    policy => { policy.AddRequirements(new AllowManageTemplatePolicy()); });
                options.AddPolicy(PolicyName.ManageView,
                    policy => { policy.AddRequirements(new AllowManageViewPolicy()); });
                options.AddPolicy(PolicyName.UploadFile,
                    policy => { policy.AddRequirements(new AllowUploadFilePolicy()); });
            });

            services.AddScoped<IFilterNode, FilterNode>();
            services.AddScoped<IFilterTemplate, FilterTemplate>();
            services.AddScoped<IFilterView, FilterView>();
            services.AddScoped<IFilterDashboard, FilterDashboard>();
            services.AddScoped<IUserService, UserService>();
        }

        public static void ConfigureOthersApplicationCore(IServiceCollection services)
        {
            //CQRS
            List<Assembly> assemblies = new List<Assembly>
            {
                typeof(NodeEndPointReferenceChangedPublicEvent).GetTypeInfo().Assembly, //DataBrowser.Domain  (DomainEvent)
                typeof(NodeByIdQuery).GetTypeInfo().Assembly, //DataBrowser.Query
                typeof(CountObservationsFromDataflowUseCase).GetTypeInfo().Assembly, //DataBrowser.UseCase
                typeof(CreateNodeCommand).GetTypeInfo().Assembly, //DataBrowser.Command
                typeof(NodeEndPointReferenceChangedHandler).GetTypeInfo().Assembly //DataBrowser.Subcribers
            };
            services.AddMediatR(assemblies.Distinct().ToArray());

            services.AddScoped<IDatasetService, DatasetService>();
            services.AddScoped<INodeConfigService, NodeConfigService>();
            services.AddScoped<IMediatorService, MediatorService>();
            services.AddScoped<IEndPointConnectorFactory, EndPointConnectorFactory>();
            services.AddScoped<IRequestContext, SpecificContextAdapter>();
            services.AddScoped<IApplicationCleaner, ApplicationCleaner>();

            //WorkerServices
            services.AddScoped<IDataflowDataCacheGenerator, DataflowDataCacheGenerator>();
            services.AddScoped<IDashboardDataCacheGenerator, DashboardDataCacheGenerator>();

            //Cache
            services.AddScoped<IDataBrowserCachesService, DataBrowserCachesService>();
        }
    }
}
