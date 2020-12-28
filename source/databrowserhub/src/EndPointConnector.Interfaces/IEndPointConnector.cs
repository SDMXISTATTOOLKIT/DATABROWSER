using System.Collections.Generic;
using System.Threading.Tasks;
using DataBrowser.Interfaces.EndPoint;
using EndPointConnector.Models;
using EndPointConnector.Models.Dto;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;

namespace EndPointConnector.Interfaces
{
    public interface IEndPointConnector
    {
        bool TryUseCache { get; set; }


        /// <summary>
        ///     Get endpoint type
        /// </summary>
        EndPointWS.WsType EndPointType { get; }

        /// <summary>
        ///     Retrieves the specific artefact.
        /// </summary>
        Task<ArtefactContainer> GetArtefactAsync(ArtefactType.ArtefactEnumType type, string id,
            ArtefactType.ReferenceDetailEnumType refDetail = ArtefactType.ReferenceDetailEnumType.None,
            ArtefactType.ResponseDetailEnumType respDetail = ArtefactType.ResponseDetailEnumType.Null,
            bool includeCrossReference = true, bool orderItems = false);

        /// <summary>
        ///     Get categorisations tree with dataflow.
        /// </summary>
        /// <param name="langs">order language</param>
        Task<NodeCatalogDto> GetNodeCatalogAsync(string lang);

        Task<ArtefactContainer> GetOnlyDataflowsValidForCatalogWithDsdCodelistAsync();

        Task<ISdmxObjects> GetDataflowWithAllUsedDataAsync(Dataflow dataflow);

        /// <summary>
        ///     Gets a bean with data about the codelist for specified dataflow and component.
        ///     The dataflow can be retrieved from <see cref="RetrieveTree" /> and the component from <see cref="GetStructure" />
        /// </summary>
        Task<ArtefactContainer> GetCodeListCostraintAsync(Dataflow dataflow, Dsd dsd, string component,
            bool orderItems = false);

        /// <summary>
        ///     Retrieves all available categorisations and category schemes.
        /// </summary>
        Task<ArtefactContainer> GetCategorySchemesAndCategorisationsAsync();

        /// <summary>
        ///     Retrieves all available dataflows.
        /// </summary>
        Task<Dataflow> GetDataflowsAsync();

        /// <summary>
        ///     Download dataflow.
        /// </summary>
        Task<GenericResponseData<string>> DownloadDataflowsAsync(Dataflow df, Dsd kf,
            List<FilterCriteria> filterCriteria, string downloadFormat, int? maxObservations = null);

        Task<ArtefactContainer> GetCodeListCostraintFilterAsync(Dataflow dataflow, Dsd dsd, string criteriaId,
            List<FilterCriteria> filterCriteria, bool orderItems = false);

        Task<GenericResponseData<string>>
            GetDataflowDataAsync(Dataflow df, Dsd kf, List<FilterCriteria> filterCriteria, ISdmxObjects extraDataflowData);

        Task<long> GetDataflowObservationCountAsync(Dataflow df, Dsd kf, List<FilterCriteria> filterCriteria);
    }
}