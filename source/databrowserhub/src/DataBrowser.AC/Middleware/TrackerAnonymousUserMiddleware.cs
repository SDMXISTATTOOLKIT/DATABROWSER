using System;
using System.Linq;
using System.Threading.Tasks;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace DataBrowser.AC.Middleware
{
    public class TrackerAnonymousUserMiddleware
    {
        private readonly ILogger<TrackerAnonymousUserMiddleware> _logger;
        private readonly RequestDelegate _next;

        public TrackerAnonymousUserMiddleware(RequestDelegate next, ILogger<TrackerAnonymousUserMiddleware> logger)
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context, IRequestContext requestContext)
        {
            if (context.Response.Headers != null)
                if (!context.Response.Headers.ContainsKey(HeaderConstants.Header_OperationGuid))
                    context.Response.Headers.Add(HeaderConstants.Header_OperationGuid,
                        requestContext.UserOperationGuid);

            if (context?.User?.Identity == null || !context.User.Identity.IsAuthenticated)
            {
                var strUserGuid = "";
                if (context.Request.Headers != null)
                {
                    context.Request.Headers.TryGetValue(HeaderConstants.Header_UserGuid, out var userGuid);

                    if (StringValues.IsNullOrEmpty(userGuid))
                    {
                        strUserGuid = Guid.NewGuid().ToString();
                        _logger.LogDebug($"Generate new guid {strUserGuid}");
                        context.Request.Headers.Add(HeaderConstants.Header_UserGuid, strUserGuid);
                    }
                    else
                    {
                        strUserGuid = userGuid.First();
                        _logger.LogDebug($"Read guid from headers{strUserGuid}");
                    }
                }

                if (context.Response.Headers != null)
                    if (!context.Response.Headers.ContainsKey(HeaderConstants.Header_UserGuid))
                        context.Response.Headers.Add(HeaderConstants.Header_UserGuid, strUserGuid);
            }

            await _next.Invoke(context);
        }
    }
}