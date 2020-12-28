using System.Collections.Generic;
using System.Threading.Tasks;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Domain.Serialization;
using DataBrowser.Interfaces;
using DataBrowser.Query.Geometries;
using DataBrowser.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WSHUB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeometryController : ApiBaseController
    {
        private readonly ILogger<GeometryController> _logger;
        private readonly IGeometryRepository _repository;
        private readonly IRequestContext _requestContext;


        public GeometryController(ILogger<GeometryController> logger,
            IRequestContext requestContext,
            IMediatorService mediatorService,
            IGeometryRepository repository)
            : base(mediatorService)
        {
            _logger = logger;
            _requestContext = requestContext;
            _repository = repository;
        }


        [HttpGet("count")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CountGeometries()
        {
            _logger.LogDebug("START CountGeometries");

            var counter = await QueryAsync(new GetCountItemsQuery());

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = DataBrowserJsonSerializer.SerializeObject(counter);
            result.StatusCode = 200;
            return result;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetAllGeometries()
        {
            _logger.LogDebug("START GetAllGeometries");

            var geometryList = await QueryAsync(new GetGeometriesQuery());

            var validResponse = geometryList != null && geometryList.Count > 0;
            var result = new ContentResult();
            result.ContentType = validResponse ? "application/json" : "application/text";
            result.Content = validResponse
                ? DataBrowserJsonSerializer.SerializeObject(geometryList)
                : "No geometry found";
            result.StatusCode = validResponse ? 200 : 404;
            return result;
        }


        [HttpGet("{geometryId}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetSingleGeometryById(string geometryId)
        {
            _logger.LogDebug("START GetSingleGeometryById");

            var geometryList = await QueryAsync(new GetGeometriesQuery(new List<string> {geometryId}));

            var validResponse = geometryList != null && geometryList.Count > 0;
            var result = new ContentResult();
            result.ContentType = validResponse ? "application/json" : "application/text";
            result.Content = validResponse
                ? DataBrowserJsonSerializer.SerializeObject(geometryList)
                : "No geometry found";
            result.StatusCode = validResponse ? 200 : 404;
            return result;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetGeometriesById([FromBody] List<string> geometryIdList)
        {
            _logger.LogDebug("START GetGeometriesById");

            var geometryList = await QueryAsync(new GetGeometriesQuery(geometryIdList));

            var validResponse = geometryList != null && geometryList.Count > 0;

            // delete me please. Generate always a valid response
            /*if (!validResponse)
            {
                var allgeometryList = await QueryAsync(new GetGeometriesQuery());
                geometryList = new List<GeometryDto>();
                for (var i = 0; i < geometryIdList.Count; i++)
                {
                    geometryList.Add(allgeometryList[i]);
                    geometryList[i].Id = geometryIdList[i];
                }
                validResponse = true;
            }*/

            var result = new ContentResult();
            result.ContentType = validResponse ? "application/json" : "application/text";
            result.Content = validResponse
                ? DataBrowserJsonSerializer.SerializeObject(geometryList)
                : "No geometry found";
            result.StatusCode = validResponse ? 200 : 404;
            return result;
        }
    }
}