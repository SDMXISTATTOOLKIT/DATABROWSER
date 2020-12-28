using DataBrowser.Interfaces.Cache;
using DataBrowser.Interfaces.Cache.Key;
using DataBrowser.Interfaces.EndPoint;
using EndPointConnector.Interfaces;
using EndPointConnector.Interfaces.Sdmx.Nsi;
using EndPointConnector.Models;
using EndPointConnector.Models.Dto;
using EndPointConnector.ParserSdmx;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sister.EndPointConnector.Sdmx
{
    public class SdmxConnector : IEndPointConnector
    {
        private readonly INsiConnector _nsiConnector;

        public SdmxConnector(INsiConnector nsiConnector)
        {
            _nsiConnector = nsiConnector;
        }

        public EndPointWS.WsType EndPointType => EndPointWS.WsType.Sdmx;

        public bool TryUseCache { get; set; }

        public async Task<ArtefactContainer> GetArtefactAsync(ArtefactType.ArtefactEnumType type, string id,
            ArtefactType.ReferenceDetailEnumType refDetail = ArtefactType.ReferenceDetailEnumType.None,
            ArtefactType.ResponseDetailEnumType respDetail = ArtefactType.ResponseDetailEnumType.Null,
            bool includeCrossReference = true, bool orderItems = false)
        {
            var keys = id.Split('+');
            var result = await _nsiConnector.GetArtefactAsync(DataModelParser.ArtefactType(type), keys[1], keys[0],
                keys[2], DataModelParser.ReferenceType(refDetail), DataModelParser.ResponseType(respDetail),
                TryUseCache, includeCrossReference, orderItems);
            return DataModelParser.ConvertArtefact(result, _nsiConnector.EndPointCustomAnnotationConfig);
        }

        public async Task<ArtefactContainer> GetCategorySchemesAndCategorisationsAsync()
        {
            var result = await _nsiConnector.GetCategorySchemesAndCategorisationsAsync(TryUseCache);
            //TODO convert ISdmxObjects to ArtefactContainer
            throw new NotImplementedException();
        }

        public async Task<ArtefactContainer> GetCodeListCostraintAsync(Dataflow dataflow, Dsd dsd, string component,
            bool orderItems)
        {
            var result = await _nsiConnector.GetCodeListCostraintAsync(DataModelParser.ConvertArtefact(dataflow),
                DataModelParser.ConvertArtefact(dsd), component, TryUseCache, orderItems);

            var containerWithCodelist = DataModelParser.ConvertArtefact(result, _nsiConnector.EndPointCustomAnnotationConfig);

            foreach (var item in result.ConceptSchemes)
            {
                var measureDim = dsd?.Dimensions?.FirstOrDefault(i => i.Type == DimensionType.MeasureDimension)?.Representation;
                containerWithCodelist.Codelists.Add(DataModelParser.ConvertArtefact(item,
                    _nsiConnector.EndPointCustomAnnotationConfig, measureDim?.Id));
            }

            return containerWithCodelist;
        }

        public async Task<ArtefactContainer> GetCodeListCostraintFilterAsync(Dataflow dataflow, Dsd dsd,
            string criteriaId, List<FilterCriteria> filterComponents, bool orderItems = false)
        {
            var artefactContainer = new ArtefactContainer();

            var results = await _nsiConnector.GetCodeListCostraintFilterAsync(DataModelParser.ConvertArtefact(dataflow),
                DataModelParser.ConvertArtefact(dsd), criteriaId, filterComponents, TryUseCache, orderItems);

            artefactContainer.Codelists = new List<Codelist>();
            foreach (var item in results.Codelists)
            {
                artefactContainer.Codelists.Add(DataModelParser.ConvertArtefact(item,
                    _nsiConnector.EndPointCustomAnnotationConfig));
            }
            foreach (var item in results.ConceptSchemes)
            {
                var measureDim = dsd?.Dimensions?.FirstOrDefault(i => i.Type == DimensionType.MeasureDimension)?.Representation;
                artefactContainer.Codelists.Add(DataModelParser.ConvertArtefact(item,
                    _nsiConnector.EndPointCustomAnnotationConfig, measureDim?.Id));
            }

            artefactContainer.Criterias = new List<Criteria>();
            foreach (var item in results.ContentConstraintObjects)
            {
                artefactContainer.Criterias.Add(DataModelParser.ConvertArtefact(item,
                    _nsiConnector.EndPointCustomAnnotationConfig));
            }

            var obsCount = results?.ContentConstraintObjects?.FirstOrDefault()?.Annotations?.FirstOrDefault(i => "obs_count".Equals(i.Id, StringComparison.InvariantCultureIgnoreCase));
            if (obsCount != null)
            {
                if (Int32.TryParse(obsCount.Title, out int obsValue))
                {
                    artefactContainer.ObsCount = obsValue;
                }
            }

            return artefactContainer;
        }

        public async Task<Dataflow> GetDataflowsAsync()
        {
            var dataflow = await _nsiConnector.GetDataflowsAsync(TryUseCache);
            return DataModelParser.ConvertArtefact(dataflow.Dataflows.FirstOrDefault(),
                _nsiConnector.EndPointCustomAnnotationConfig);
        }

        public async Task<GenericResponseData<string>> GetDataflowDataAsync(Dataflow df, Dsd kf,
            List<FilterCriteria> filterCriteria, ISdmxObjects extraDataflowData)
        {
            return await _nsiConnector.GetDataflowJsonStatDataAsync(DataModelParser.ConvertArtefact(df),
                DataModelParser.ConvertArtefact(kf), filterCriteria, extraDataflowData, TryUseCache);
        }

        public async Task<NodeCatalogDto> GetNodeCatalogAsync(string lang)
        {
            return await _nsiConnector.GetNodeCatalogAsync(lang, TryUseCache);
        }

        public async Task<long> GetDataflowObservationCountAsync(Dataflow df, Dsd kf,
            List<FilterCriteria> filterCriteria)
        {
            return await _nsiConnector.GetDataflowObservationCountAsync(DataModelParser.ConvertArtefact(df),
                DataModelParser.ConvertArtefact(kf), filterCriteria, TryUseCache);
        }

        public async Task<GenericResponseData<string>> DownloadDataflowsAsync(Dataflow df, Dsd kf,
            List<FilterCriteria> filterCriteria, string downloadFormat, int? maxObservations = null)
        {
            return await _nsiConnector.DownloadDataflowsAsync(DataModelParser.ConvertArtefact(df),
                DataModelParser.ConvertArtefact(kf), filterCriteria, downloadFormat, maxObservations);
        }

        public async Task<ArtefactContainer> GetOnlyDataflowsValidForCatalogWithDsdCodelistAsync()
        {
            var sdmxObjects = await _nsiConnector.GetOnlyDataflowsValidForCatalogWithDsdAsync();
            return DataModelParser.ConvertArtefact(sdmxObjects, _nsiConnector.EndPointCustomAnnotationConfig);
        }

        public async Task<ISdmxObjects> GetDataflowWithAllUsedDataAsync(Dataflow dataflow)
        {
            if (!_nsiConnector.EndPointConfig.RestDataResponseXml)
            {
                return null;
            }
            return await _nsiConnector.GetDataflowWithUsedData(DataModelParser.ConvertArtefact(dataflow));
        }
        
    }
}