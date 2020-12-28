using EndPointConnector.Interfaces.Sdmx.Models;
using EndPointConnector.Models;
using Microsoft.Extensions.Logging;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Factory;
using Org.Sdmxsource.Sdmx.Api.Manager.Query;
using Org.Sdmxsource.Sdmx.Api.Model.Format;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Query;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.SdmxQueryBuilder.Factory;
using Org.Sdmxsource.Sdmx.SdmxQueryBuilder.Manager;
using Org.Sdmxsource.Sdmx.SdmxQueryBuilder.Model;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sister.EndPointConnector.Sdmx.Nsi.Rest.Get
{
    public class NsiGetStructureRest
    {
        private readonly ILogger<NsiGetStructureRest> _logger;

        public NsiGetStructureRest(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<NsiGetStructureRest>();
        }

        public Dictionary<ComponentType.EnumComponentType, List<string>> GetCodeListCostraint(IDataflowObject dataflow,
            IDataStructureObject dsd, string componentId)
        {
            _logger.LogDebug($"START{MethodBase.GetCurrentMethod().Name}");

            var requestsCodelist = new Dictionary<ComponentType.EnumComponentType, List<string>>
            {
                { ComponentType.EnumComponentType.TimeDimension, new List<string>() },
                { ComponentType.EnumComponentType.MeasureDimension, new List<string>() },
                { ComponentType.EnumComponentType.FrequencyDimension, new List<string>() },
                { ComponentType.EnumComponentType.Dimension, new List<string>() },
                { ComponentType.EnumComponentType.Attribute, new List<string>() }
            };

            var reference = "codelist";
            var component = dsd.GetComponent(componentId);
            if (component == null)
            {
                _logger.LogDebug("component is null");
                component = dsd.Components.FirstOrDefault(x => x.ConceptRef.FullId == componentId);
            }

            var dimension = component as IDimension;
            if (dimension == null)
            {
                _logger.LogWarning($"Exclude non dimension value: {component.Id} \t");
                return requestsCodelist;
            }

            if (dimension.MeasureDimension)
            {
                reference = "conceptscheme";
            }

            var request =
                $"availableconstraint/{dataflow.AgencyId},{dataflow.Id},{dataflow.Version}/ALL/ALL/{componentId}?references={reference}";

            if (dimension.TimeDimension)
            {
                requestsCodelist[ComponentType.EnumComponentType.TimeDimension].Add(request);
                _logger.LogDebug(
                    $"Component id: {dimension.Id} \t Type: {ComponentType.EnumComponentType.TimeDimension}");
            }
            else if (dimension.MeasureDimension)
            {
                requestsCodelist[ComponentType.EnumComponentType.MeasureDimension].Add(request);
                _logger.LogDebug(
                    $"Component id: {dimension.Id} \t Type: {ComponentType.EnumComponentType.MeasureDimension}");
            }
            else if (dimension.FrequencyDimension)
            {
                requestsCodelist[ComponentType.EnumComponentType.FrequencyDimension].Add(request);
                _logger.LogDebug(
                    $"Component id: {dimension.Id} \t Type: {ComponentType.EnumComponentType.FrequencyDimension}");
            }
            else
            {
                requestsCodelist[ComponentType.EnumComponentType.Dimension].Add(request);
                _logger.LogDebug($"Component id: {dimension.Id} \t Type: {ComponentType.EnumComponentType.Dimension}");
            }


            return requestsCodelist;
        }

        /// <summary>
        ///     Retrieves all available categorisations and category schemes.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string GetCategorySchemesAndCategorisations()
        {
            _logger.LogDebug($"START {MethodBase.GetCurrentMethod().Name}");

            var catSchema =
                new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.CategoryScheme));
            IStructureQueryFormat<string> structureQueryFormat = new RestQueryFormat();
            IRestStructureQuery structureQuery = new RESTStructureQueryCore(
                StructureQueryDetail.GetFromEnum(StructureQueryDetailEnumType.Full),
                StructureReferenceDetail.GetFromEnum(StructureReferenceDetailEnumType.Parents), null, catSchema, false);
            IStructureQueryFactory factory = new RestStructureQueryFactory();

            IStructureQueryBuilderManager structureQueryBuilderManager = new StructureQueryBuilderManager(factory);
            var request = structureQueryBuilderManager.BuildStructureQuery(structureQuery, structureQueryFormat);
            _logger.LogDebug($"request value: {request}");


            _logger.LogDebug($"END {MethodBase.GetCurrentMethod().Name}");
            return request;
        }

        /// <summary>
        ///     Retrieves all available dataflows.
        /// </summary>
        public string GetDataflows(bool includeChildren)
        {
            _logger.LogDebug($"START {MethodBase.GetCurrentMethod().Name}");

            var refDetail = StructureReferenceDetailEnumType.None;
            if (includeChildren)
            {
                refDetail = StructureReferenceDetailEnumType.Children;
            }

            var dataflowRefBean =
                new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dataflow));
            IStructureQueryFormat<string> structureQueryFormat = new RestQueryFormat();
            IRestStructureQuery structureQuery = new RESTStructureQueryCore(
                StructureQueryDetail.GetFromEnum(StructureQueryDetailEnumType.Full),
                StructureReferenceDetail.GetFromEnum(refDetail),
                SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Null), dataflowRefBean, false);
            IStructureQueryFactory factory = new RestStructureQueryFactory();

            IStructureQueryBuilderManager structureQueryBuilderManager = new StructureQueryBuilderManager(factory);
            var request = structureQueryBuilderManager.BuildStructureQuery(structureQuery, structureQueryFormat);
            _logger.LogDebug($"request value: {request}");

            _logger.LogDebug($"END {MethodBase.GetCurrentMethod().Name}");
            return request;
        }

        public string GetCodeListCostraintFilter(IDataflowObject dataflow, IDataStructureObject dsd, string criteriaId,
            List<FilterCriteria> filterCriteria, DataflowDataRange timeRange)
        {
            _logger.LogDebug($"START{MethodBase.GetCurrentMethod().Name}");

            var strFilter = new StringBuilder();
            var timePeriodFilter = "";
            var nConstraint = 0;
            var reference = "codelist";
            foreach (var component in dsd.Components)
            {
                var dimension = component as IDimension;
                if (dimension == null)
                {
                    continue;
                }

                if (dimension.TimeDimension)
                {
                    var constraintTimePeriodFilter = filterCriteria.FirstOrDefault(i =>
                        i.Id.Equals("TIME_PERIOD", StringComparison.InvariantCultureIgnoreCase));

                    if (constraintTimePeriodFilter != null &&
                        constraintTimePeriodFilter.Type == FilterType.TimeRange)
                    {
                        var start = constraintTimePeriodFilter.From.Value.ToString("yyyy-MM-dd");
                        var end = "";
                        if (constraintTimePeriodFilter.To.HasValue)
                        {
                            end = "&endPeriod=" + constraintTimePeriodFilter.To.Value.ToString("yyyy-MM-dd");
                        }
                        timePeriodFilter = $"&startPeriod={start}{end}";
                    }
                    else if (constraintTimePeriodFilter != null &&
                        constraintTimePeriodFilter.Type == FilterType.TimePeriod)
                    {
                        var start = timeRange.CalcolateStartRangeFromPeriod().ToString("yyyy-MM-dd");
                        var end = timeRange.EndRange.ToString("yyyy-MM-dd");
                        timePeriodFilter = $"&startPeriod={start}&endPeriod={end}";
                    }

                    continue;
                }

                if (dimension.Id.Equals(criteriaId, StringComparison.InvariantCultureIgnoreCase) &&
                    dimension.MeasureDimension)
                {
                    reference = "ConceptScheme";
                }

                nConstraint++;
                if (nConstraint > 1)
                {
                    strFilter.Append('.');
                }

                var constraintFilter = filterCriteria.FirstOrDefault(i => i.Id.Equals(dimension.Id));
                if (constraintFilter != null && constraintFilter.FilterValues != null &&
                    constraintFilter.FilterValues.Count > 0)
                {
                    strFilter.Append(string.Join("+", constraintFilter.FilterValues));
                }
            }

            var filterSingleCriteria = "";
            if (!string.IsNullOrWhiteSpace(criteriaId))
            {
                filterSingleCriteria = $"ALL/{criteriaId}";
            }

            var request =
                $"availableconstraint/{dataflow.AgencyId},{dataflow.Id},{dataflow.Version}/{strFilter}/{filterSingleCriteria}?references={reference}{timePeriodFilter}";

            _logger.LogDebug($"request value: {request}");
            return request;
        }

        public string GetFrequences(IDataflowObject dataflow, IDataStructureObject dsd, List<FilterCriteria> filterCriteria)
        {
            _logger.LogDebug($"START{MethodBase.GetCurrentMethod().Name}");


            var strFilter = new StringBuilder();
            var nConstraint = 0;
            var filterSingleCriteria = "";
            foreach (var component in dsd.Components)
            {
                var dimension = component as IDimension;
                if (dimension == null)
                {
                    continue;
                }

                if (dimension.TimeDimension)
                {
                    continue;
                }
                if (dimension.FrequencyDimension)
                {
                    filterSingleCriteria = dimension.Id;
                }

                nConstraint++;
                if (nConstraint > 1)
                {
                    strFilter.Append('.');
                }

                var constraintFilter = filterCriteria?.FirstOrDefault(i => i.Id.Equals(dimension.Id));
                if (constraintFilter != null && constraintFilter.FilterValues != null &&
                    constraintFilter.FilterValues.Count > 0)
                {
                    strFilter.Append(string.Join("+", constraintFilter.FilterValues));
                }
            }

            if (string.IsNullOrWhiteSpace(filterSingleCriteria))
            {
                _logger.LogInformation("Dimension frequency not found");
                return null;
            }

            var request =
                $"availableconstraint/{dataflow.AgencyId},{dataflow.Id},{dataflow.Version}/{strFilter}/{filterSingleCriteria}?references=codelist";

            _logger.LogDebug($"request value: {request}");
            return request;
        }
    }
}