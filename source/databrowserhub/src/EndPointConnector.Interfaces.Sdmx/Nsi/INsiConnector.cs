using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using DataBrowser.Interfaces.EndPoint;
using EndPointConnector.Interfaces.Sdmx.Models;
using EndPointConnector.Models;
using EndPointConnector.Models.Dto;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Engine;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;

namespace EndPointConnector.Interfaces.Sdmx.Nsi
{
    public interface INsiConnector
    {
        SdmxEndPointCostant.ConnectorType EndPointType { get; }

        EndPointCustomAnnotationConfig EndPointCustomAnnotationConfig { get; }
        EndPointSdmxConfig EndPointConfig { get; }

        Task<ISdmxObjects> GetArtefactAsync(SdmxStructureEnumType type, string id, string agency, string version,
            StructureReferenceDetailEnumType refDetail = StructureReferenceDetailEnumType.None, string respDetail = "",
            bool useCache = false, bool includeCrossReference = true, bool orderItems = false);

        Task<NodeCatalogDto> GetNodeCatalogAsync(string lang, bool useCache = false);

        Task<ISdmxObjects> GetCategorySchemesAndCategorisationsAsync(bool useCache = false);

        Task<ISdmxObjects> GetDataflowsAsync(bool useCache = false);
        Task<ISdmxObjects> GetOnlyDataflowsValidForCatalogWithDsdAsync(bool useCache = false);


        Task<ISdmxObjects> SendQueryStructureRequestAsync(IEnumerable<IStructureReference> references,
            bool resolveReferences);

        Task<ISdmxObjects> GetCodeListCostraintAsync(IDataflowObject dataflow, IDataStructureObject dsd,
            string component, bool useCache = false, bool orderItems = false);

        Task<ISdmxObjects> GetCodeListCostraintFilterAsync(IDataflowObject dataflow, IDataStructureObject dsd,
            string criteriaId, List<FilterCriteria> filterComponents, bool useCache = false, bool orderItems = false);

        Task<GenericResponseData<IDataReaderEngine>> GetDataflowDataAsync(IDataflowObject df, IDataStructureObject kf,
            List<FilterCriteria> filterCriteria, bool useAttr, bool useCache = false);

        Task<GenericResponseData<XmlDocument>> GetDataflowXmlCompactDataAsync(IDataflowObject df,
            IDataStructureObject kf, List<FilterCriteria> filterCriteria, bool useCache = false);

        Task<GenericResponseData<string>> GetDataflowJsonSdmxDataAsync(IDataflowObject df, IDataStructureObject kf,
            List<FilterCriteria> filterCriteria, bool useCache = false);

        Task<ISdmxObjects> GetDataflowWithUsedData(IDataflowObject df);

        Task<GenericResponseData<string>> GetDataflowJsonStatDataAsync(IDataflowObject df, IDataStructureObject kf,
            List<FilterCriteria> filterCriteria, ISdmxObjects extraDataflowData, bool useCache = false);

        Task<long> GetDataflowObservationCountAsync(IDataflowObject df, IDataStructureObject kf,
            List<FilterCriteria> filterCriteria, bool useCache = false);

        Task<GenericResponseData<string>> DownloadDataflowsAsync(IDataflowObject df, IDataStructureObject kf,
            List<FilterCriteria> filterCriteria, string downloadFormat, int? maxObservations = null);
    }
}