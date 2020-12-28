using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WSHUB.HealthChecks
{
    public class ApiHealthCheck : IHealthCheck
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;

        public ApiHealthCheck(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        {
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            return HealthCheckResult.Unhealthy("The check indicates an unhealthy result.");

            //var request = _httpContextAccessor.HttpContext.Request;

            //var apiLink = _linkGenerator.GetPathByAction("Get", "Artefact");
            //var myUrl = request.Scheme + "://" + request.Host + apiLink;
            //string pageContents;
            //using (var client = new HttpClient())
            //{
            //    using (var response = await client.GetAsync(myUrl))
            //    {
            //        pageContents = await response.Content.ReadAsStringAsync();
            //    }
            //}

            //if (pageContents.Contains(".NET Bot Black Sweatshirt"))
            //    return HealthCheckResult.Healthy("The check indicates a healthy result.");

            //return HealthCheckResult.Unhealthy("The check indicates an unhealthy result.");
        }
    }
}