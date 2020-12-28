using System.Threading.Tasks;
using DataBrowser.Interfaces.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TracertLOg;

namespace WSHUB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TracingController : ControllerBase
    {
        /// <summary>
        ///     Get al tracing for specific operation
        /// </summary>
        /// <param name="nodeId">Node id</param>
        /// <response code="200">Node found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpGet("{operationId}")]
        [HttpGet("{operationId}/{simpleMode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<ActionResult> GetTracing(string operationId, bool? simpleMode)
        {
            var nodeResult = await Tracing.ReadTracingAsync(operationId, simpleMode.HasValue && simpleMode.Value);

            var serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = nodeResult != null
                ? JsonConvert.SerializeObject(nodeResult, serializerSettings)
                : "Trace not found";
            result.StatusCode = nodeResult != null ? 200 : 404;
            return result;
        }

        [HttpGet("{operationId}/FilterBy/{operationName}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<ActionResult> GetTracingFilter(string operationId, string operationName)
        {
            var nodeResult = await Tracing.ReadTracingAsync(operationId, false, operationName);


            var serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = nodeResult != null
                ? JsonConvert.SerializeObject(nodeResult, serializerSettings)
                : "Trace not found";
            result.StatusCode = nodeResult != null ? 200 : 404;
            return result;
        }

        /// <summary>
        ///     Get al tracing for specific operation
        /// </summary>
        /// <param name="nodeId">Node id</param>
        /// <response code="200">Node found.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Node not found.</response>
        /// <response code="500">Error.</response>
        /// <returns>JsonSdmx</returns>
        [HttpGet("Limit/{lastOperations}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<ActionResult> GetTracingLast(int lastOperations)
        {
            var nodeResult = await Tracing.ReadTracingAsync(lastNsiOperation: lastOperations, simpleMode: true);

            var serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = nodeResult != null
                ? JsonConvert.SerializeObject(nodeResult, serializerSettings)
                : "Trace not found";
            result.StatusCode = nodeResult != null ? 200 : 404;
            return result;
        }
    }
}