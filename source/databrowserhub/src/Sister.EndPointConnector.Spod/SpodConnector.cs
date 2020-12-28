using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataBrowser.Interfaces.EndPoint;
using EndPointConnector.Interfaces;
using EndPointConnector.Models;
using EndPointConnector.Models.Dto;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;

namespace Sister.EndPointConnector.Spod
{
    public class SpodConnector : IEndPointConnector
    {
        public bool TryUseCache
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public EndPointWS.WsType EndPointType => throw new NotImplementedException();

        public Task<GenericResponseData<string>> DownloadDataflowsAsync(Dataflow df, Dsd kf,
            List<FilterCriteria> filterCriteria, string downloadFormat, int? maxObservations = null)
        {
            throw new NotImplementedException();
        }

        public Task<ArtefactContainer> GetArtefactAsync(ArtefactType.ArtefactEnumType type, string id,
            ArtefactType.ReferenceDetailEnumType refDetail = ArtefactType.ReferenceDetailEnumType.None,
            ArtefactType.ResponseDetailEnumType respDetail = ArtefactType.ResponseDetailEnumType.Null,
            bool includeCrossReference = true, bool orderItems = false)
        {
            throw new NotImplementedException();
        }

        public Task<ArtefactContainer> GetCategorySchemesAndCategorisationsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ArtefactContainer> GetCodeListCostraintAsync(Dataflow dataflow, Dsd dsd, string component,
            bool orderItems = false)
        {
            throw new NotImplementedException();
        }

        public Task<ArtefactContainer> GetCodeListCostraintFilterAsync(Dataflow dataflow, Dsd dsd, string criteriaId,
            List<FilterCriteria> filterCriteria, bool orderItems = false)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponseData<string>> GetDataflowDataAsync(Dataflow df, Dsd kf,
            List<FilterCriteria> filterCriteria)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponseData<string>> GetDataflowDataAsync(Dataflow df, Dsd kf, List<FilterCriteria> filterCriteria, ArtefactContainer extraDataflowData)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponseData<string>> GetDataflowDataAsync(Dataflow df, Dsd kf, List<FilterCriteria> filterCriteria, ISdmxObjects extraDataflowData)
        {
            throw new NotImplementedException();
        }

        public Task<long> GetDataflowObservationCountAsync(Dataflow df, Dsd kf, List<FilterCriteria> filterCriteria)
        {
            throw new NotImplementedException();
        }

        public Task<Dataflow> GetDataflowsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ArtefactContainer> GetDataflowWithAllUsedDataAsync(Dataflow dataflow)
        {
            throw new NotImplementedException();
        }

        public Task<NodeCatalogDto> GetNodeCatalogAsync(string lang)
        {
            throw new NotImplementedException();
        }

        public Task<ArtefactContainer> GetOnlyDataflowsValidForCatalogWithDsdCodelistAsync()
        {
            throw new NotImplementedException();
        }

        Task<ISdmxObjects> IEndPointConnector.GetDataflowWithAllUsedDataAsync(Dataflow dataflow)
        {
            throw new NotImplementedException();
        }
    }
}