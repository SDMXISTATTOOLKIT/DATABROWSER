using System;
using System.Globalization;
using System.Reflection;
using System.Xml;
using EndPointConnector.Interfaces.Sdmx.Models;
using EndPointConnector.ParserSdmx;
using Microsoft.Extensions.Logging;
using Org.Sdmxsource.Sdmx.Api.Constants;

namespace Sister.EndPointConnector.Sdmx.Nsi.Soap.Get
{
    public class NsiGetArtefactSoap
    {
        private const string namespaceQuery = "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/query";

        private readonly EndPointSdmxConfig _endPointSDMXNodeConfig;
        private readonly ILogger<NsiGetArtefactSoap> _logger;

        public NsiGetArtefactSoap(EndPointSdmxConfig endPointSDMXNodeConfig, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<NsiGetArtefactSoap>();
            _endPointSDMXNodeConfig = endPointSDMXNodeConfig;
        }

        public XmlDocument GetArtefact(SdmxStructureEnumType type, string id, string agency, string version,
            StructureReferenceDetailEnumType refDetail = StructureReferenceDetailEnumType.None, string respDetail = "")
        {
            _logger.LogDebug($"START {MethodBase.GetCurrentMethod().Name}");


            var soapAct = SDMXUtils.GetQuerySoapAction(type);
            var action = string.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}{2}",
                _endPointSDMXNodeConfig.Namespace,
                _endPointSDMXNodeConfig.Namespace.EndsWith("/", StringComparison.Ordinal) ? string.Empty : "/",
                soapAct);

            _logger.LogDebug("composeSoapQuery");
            var requestQueryMessage = composeSoapQuery(type, id, agency, version,
                refDetail, respDetail, action,
                _endPointSDMXNodeConfig.Prefix, _endPointSDMXNodeConfig.Namespace);
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("compose Soap Query:");
                _logger.LogTrace(requestQueryMessage.OuterXml);
            }

            _logger.LogDebug($"END {MethodBase.GetCurrentMethod().Name}");
            return requestQueryMessage;
        }

        /// <summary>
        ///     Get XmlQueryMessage for getting SDMX artefacts.
        /// </summary>
        /// <param name="artefact">Artefact type.</param>
        /// <param name="id">Artefact id.</param>
        /// <param name="agencyID">Artefact agency.</param>
        /// <param name="version">Artefact version.</param>
        /// <param name="operation">Operation to be performed through the query.</param>
        /// <param name="refDetail">Reference detail: eg. None, Children, Parents (default = None).</param>
        /// <returns></returns>
        private XmlDocument composeSoapQuery(SdmxStructureEnumType artefactType, string id, string agencyID,
            string version, StructureReferenceDetailEnumType refDetail, string responseDetail, string operation,
            string prefix, string nameSpace)
        {
            _logger.LogDebug($"START {MethodBase.GetCurrentMethod().Name}");

            var originalReturnDetail = responseDetail;
            var xDom = new XmlDocument();

            // Carico il template
            xDom.Load(getTemplate(artefactType));

            //setto id, agency e version o rimuovo il filtro
            SetKey(ref xDom, id, agencyID, version);

            //setto il livello di dettaglio
            //WARNING!!!!!!!!!!  Artefacts with cross references cannot be queried with Stub parameter
            if (refDetail != StructureReferenceDetailEnumType.None)
                responseDetail = "Full";
            else
                responseDetail = artefactType == SdmxStructureEnumType.CodeList ||
                                 artefactType == SdmxStructureEnumType.Dsd ||
                                 artefactType == SdmxStructureEnumType.ConceptScheme ||
                                 artefactType == SdmxStructureEnumType.CategoryScheme ||
                                 artefactType == SdmxStructureEnumType.AgencyScheme
                    ? "CompleteStub"
                    : "Stub";

            var isSingleArtefactRequest = id != null && agencyID != null && version != null;
            responseDetail = isSingleArtefactRequest || artefactType == SdmxStructureEnumType.Dataflow ||
                             artefactType == SdmxStructureEnumType.Categorisation ||
                             artefactType == SdmxStructureEnumType.Agency ||
                             artefactType == SdmxStructureEnumType.MetadataFlow
                ? "Full"
                : responseDetail;

            if (!string.IsNullOrWhiteSpace(originalReturnDetail))
            {
                _logger.LogDebug($"originalReturnDetail: {originalReturnDetail} \t returnDetail: {responseDetail}");
                responseDetail = originalReturnDetail;
            }

            _logger.LogDebug($"returnDetail: {responseDetail}");
            SetReturnDetail(ref xDom, responseDetail);

            //setto il reference detail
            SetReferenceDetail(ref xDom,
                refDetail == StructureReferenceDetailEnumType.Null ? StructureReferenceDetailEnumType.None : refDetail);

            _logger.LogDebug($"END {MethodBase.GetCurrentMethod().Name}");
            return xDom;
        }

        /// <summary>
        ///     Get XmlTemplate for the artefact type given.
        /// </summary>
        /// <param name="artefactType"></param>
        /// <returns></returns>
        private string getTemplate(SdmxStructureEnumType artefactType)
        {
            var fileName = ".\\SdmxQueryTemplate\\2.1\\" + artefactType + ".xml";
            return fileName;
        }

        /// <summary>
        ///     Set id, agency and version for the query or replace the corrispondent xml node.
        /// </summary>
        /// <param name="xDom">XML document containing the query.</param>
        /// <param name="id">Artefact id.</param>
        /// <param name="agency">Artefact agency.</param>
        /// <param name="version">Artefact version.</param>
        private void SetKey(ref XmlDocument xDom, string id, string agency, string version)
        {
            _logger.LogDebug($"START {MethodBase.GetCurrentMethod().Name}");
            var xManag = GetNamespaceManager(xDom);

            var xNodeID = xDom.SelectSingleNode("//query:ID", xManag);
            var xNodeAgency = xDom.SelectSingleNode("//query:AgencyID", xManag);
            var xNodeVersion = xDom.SelectSingleNode("//query:Version", xManag);

            if (id != null)
                xNodeID.InnerText = id;
            else
                xNodeID.ParentNode.RemoveChild(xNodeID);

            if (agency != null)
                xNodeAgency.InnerText = agency;
            else
                xNodeAgency.ParentNode.RemoveChild(xNodeAgency);

            if (version != null)
                xNodeVersion.InnerText = version;
            else
                xNodeVersion.ParentNode.RemoveChild(xNodeVersion);

            _logger.LogDebug($"END {MethodBase.GetCurrentMethod().Name}");
        }

        /// <summary>
        ///     Get NameSpaceManager for the document.
        /// </summary>
        /// <returns></returns>
        private XmlNamespaceManager GetNamespaceManager(XmlDocument xDom)
        {
            var xManag = new XmlNamespaceManager(xDom.NameTable);
            xManag.AddNamespace("query", namespaceQuery);

            return xManag;
        }

        /// <summary>
        ///     Set the return detail for the query, i.e. Full or Stub.
        /// </summary>
        /// <param name="xDom">XML document containing the query.</param>
        /// <param name="value">Return deatil (Full or Stub).</param>
        private void SetReturnDetail(ref XmlDocument xDom, string value)
        {
            var xNodeID = xDom.SelectSingleNode("//query:ReturnDetails", GetNamespaceManager(xDom));
            xNodeID.Attributes["detail"].Value = value;
        }

        /// <summary>
        ///     Set the reference detail for the query.
        /// </summary>
        /// <param name="xDom">XML document containing the query.</param>
        /// <param name="refDetail">Reference deatil. Eg. None, Children, Parent, etc.</param>
        private void SetReferenceDetail(ref XmlDocument xDom, StructureReferenceDetailEnumType refDetail)
        {
            var xManag = GetNamespaceManager(xDom);

            var xNodeID = xDom.SelectSingleNode("//query:References", xManag);
            var refDet = xDom.CreateElement("query", refDetail.ToString(), xManag.LookupNamespace("query"));


            xNodeID.AppendChild(refDet);
        }
    }
}