using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using EndPointConnector.Interfaces.Sdmx.Models;
using EndPointConnector.Interfaces.Sdmx.Nsi;
using EndPointConnector.Interfaces.Sdmx.Nsi.Get;
using EndPointConnector.ParserSdmx;
using Estat.Sri.CustomRequests.Manager;
using Estat.Sri.CustomRequests.Model;
using Microsoft.Extensions.Logging;
using Org.Sdmxsource.Sdmx.Api.Model.Format;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;

namespace Sister.EndPointConnector.Sdmx.Nsi.Soap.Get
{
    public class NsiGetV20Soap : INsiGetV20
    {
        private readonly INsiEndPointHttpRequest _endPointHttpRequest;
        private readonly ILogger<NsiGetV20Soap> _logger;
        private readonly SdmxParser _sdmxParser;

        public NsiGetV20Soap(INsiEndPointHttpRequest endPointHttpRequest, ILoggerFactory loggerFactory)
        {
            _endPointHttpRequest = endPointHttpRequest;
            _sdmxParser = new SdmxParser(loggerFactory);
            _logger = loggerFactory.CreateLogger<NsiGetV20Soap>();
        }

        /// <summary>
        ///     Sends the specified <paramref name="references" /> to the Web Service defined by <see cref="_config" />
        /// </summary>
        /// <param name="references">The <see cref="IStructureReference" /></param>
        /// <param name="resolveReferences">
        ///     The resolve references
        /// </param>
        /// <returns>The QueryStructureResponse returned by the Web Service</returns>
        public async Task<ISdmxObjects> SendQueryStructureRequestV20Async(
            IEnumerable<IStructureReference> references,
            bool resolveReferences)
        {
            _logger.LogDebug("START SendQueryStructureRequestV20Async");
            var queryStructureRequestBuilderManager = new QueryStructureRequestBuilderManager();

            IStructureQueryFormat<XDocument> queryFormat = new QueryStructureRequestFormat();
            _logger.LogDebug("BuildStructureQuery");
            var wdoc = queryStructureRequestBuilderManager.BuildStructureQuery(references, queryFormat,
                resolveReferences);

            var doc = new XmlDocument();
            _logger.LogDebug("LoadXml");
            doc.LoadXml(wdoc.ToString());

            var httpRequest =
                _endPointHttpRequest.CreateRequest(doc, SdmxEndPointCostant.SDMXWSFunction.QueryStructure, true);
            var response = await _endPointHttpRequest.SendRequestAsync(httpRequest);

            if (response.XmlResponse.InnerText.Contains("No Results Found"))
            {
                _logger.LogDebug("END Artefact not found");
                return new SdmxObjectsImpl();
            }

            var parseReuslt = _sdmxParser.GetSdmxObjectsFromNsiResponse(response);
            _logger.LogDebug("END SendQueryStructureRequestV20Async");
            return parseReuslt;
        }
    }
}