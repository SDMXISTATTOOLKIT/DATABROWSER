using System;
using EndPointConnector.Interfaces.Sdmx.Models;
using EndPointConnector.ParserSdmx;
using Microsoft.Extensions.Logging;
using Org.Sdmxsource.Sdmx.Api.Constants;

namespace Sister.EndPointConnector.Sdmx.Nsi.Rest.Get
{
    public class NsiGetArtefactRest
    {
        private readonly EndPointSdmxConfig _endPointSDMXNodeConfig;
        private readonly ILogger<NsiGetArtefactRest> _logger;

        public NsiGetArtefactRest(ILoggerFactory loggerFactory, EndPointSdmxConfig endPointSDMXNodeConfig)
        {
            _logger = loggerFactory.CreateLogger<NsiGetArtefactRest>();
            _endPointSDMXNodeConfig = endPointSDMXNodeConfig;
        }

        public string GetArtefact(SdmxStructureEnumType type, string id, string agency, string version,
            StructureReferenceDetailEnumType refDetail = StructureReferenceDetailEnumType.None, string respDetail = "")
        {
            var parameters = composeRestQuery(type, id, agency, version, refDetail, respDetail);
            _logger.LogDebug($"parameters generated: {parameters}");
            return parameters;
        }

        private string composeRestQuery(SdmxStructureEnumType type, string id, string agency, string version,
            StructureReferenceDetailEnumType refDetail, string responseDetail)
        {
            if (!string.IsNullOrWhiteSpace(responseDetail) &&
                responseDetail.Equals("stub", StringComparison.InvariantCultureIgnoreCase))
                //Mapping name from SOAP to REST
                responseDetail = _endPointSDMXNodeConfig.SupportAllCompleteStubs ? "allcompletestubs" : "allstubs";
            var originalReturnDetail = responseDetail;
            var queryRest = "";

            queryRest += SDMXUtils.GetArtefactString(type);

            queryRest += "/" + (!string.IsNullOrWhiteSpace(agency) ? agency : "all"); //AGENCY
            queryRest += "/" + (!string.IsNullOrWhiteSpace(id) ? id : "all"); //ID
            if (!string.IsNullOrWhiteSpace(version)) //VERSION
                queryRest += $"/{version}";
            else
                queryRest += "/all";

            responseDetail = _endPointSDMXNodeConfig.SupportAllCompleteStubs ? "allcompletestubs" : "allstubs";
            var isSingleArtefactRequest = id != null && agency != null && version != null;
            responseDetail = isSingleArtefactRequest || refDetail != StructureReferenceDetailEnumType.None ||
                             type == SdmxStructureEnumType.Dsd || type == SdmxStructureEnumType.Dataflow ||
                             type == SdmxStructureEnumType.Categorisation || type == SdmxStructureEnumType.Agency ||
                             type == SdmxStructureEnumType.AgencyScheme ||
                             type == SdmxStructureEnumType.ContentConstraint ||
                             type == SdmxStructureEnumType.MetadataFlow
                ? "Full"
                : responseDetail;


            if (!string.IsNullOrWhiteSpace(originalReturnDetail)) responseDetail = originalReturnDetail;
            queryRest += $"/?detail={responseDetail}&references={refDetail}";

            return queryRest;
        }

        /*
         * 
         * 
         * EXAMPLE CREATE QUERY STRING
         * 
         * var dataflowRefBean = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dataflow));
         *
         *  IRestStructureQuery structureQuery = new RESTStructureQueryCore(StructureQueryDetail.GetFromEnum(StructureQueryDetailEnumType.Full), StructureReferenceDetail.GetFromEnum(StructureReferenceDetailEnumType.Specific), SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dsd), dataflowRefBean, true);
         *  IStructureQueryFactory factory = new RestStructureQueryFactory();
         *  IStructureQueryBuilderManager structureQueryBuilderManager = new StructureQueryBuilderManager(factory);
         *  IStructureQueryFormat<string> structureQueryFormat = new RestQueryFormat();
         *  string request = structureQueryBuilderManager.BuildStructureQuery(structureQuery, structureQueryFormat);
         *
         *  throw new NotImplementedException();
         * 
         * 
         */
    }
}