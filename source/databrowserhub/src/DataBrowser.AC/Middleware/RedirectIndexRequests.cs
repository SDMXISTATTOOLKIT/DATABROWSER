using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;

namespace DataBrowser.AC.Middleware
{
    public class RedirectIndexRequests : IRule
    {
        private readonly string _urlToRedirect;

        public RedirectIndexRequests(string urlToRedirect)
        {
            _urlToRedirect = urlToRedirect;
        }

        public void ApplyRule(RewriteContext context)
        {
            var request = context.HttpContext.Request;

            if (request.Path.Value.Equals(request.PathBase.Value + "/", StringComparison.OrdinalIgnoreCase))
            {
                var response = context.HttpContext.Response;
                response.StatusCode = StatusCodes.Status307TemporaryRedirect;
                context.Result = RuleResult.EndResponse;
                response.Headers[HeaderNames.Location] =
                    $"{request.Scheme}://{request.Host}{request.PathBase.Value.TrimEnd('/')}/{_urlToRedirect}";
            }
        }
    }
}