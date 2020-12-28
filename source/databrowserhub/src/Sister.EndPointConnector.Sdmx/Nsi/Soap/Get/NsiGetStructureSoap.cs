using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EndPointConnector.Interfaces.Sdmx.Models;
using EndPointConnector.Models;
using EndPointConnector.ParserSdmx;
using Estat.Sdmxsource.Extension.Builder;
using Estat.Sri.CustomRequests.Model;
using Microsoft.Extensions.Logging;
using Org.Sdmxsource.Sdmx.Api.Builder;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference.Complex;
using Org.Sdmxsource.Sdmx.Api.Model.Query;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.Reference.Complex;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;

namespace Sister.EndPointConnector.Sdmx.Nsi.Soap.Get
{
    public class NsiGetStructureSoap
    {
        private const string InfoPartialCodelistFormat3 = "Dataflow {0}, Component : {1} , Codelist : {2}";
        private const string InfoGettingCodelistFormat1 = "Getting codelist for {0}";

        private readonly ILogger<NsiGetStructureSoap> _logger;

        public NsiGetStructureSoap(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<NsiGetStructureSoap>();
        }

        public List<List<IStructureReference>> GetCodeListCostraint(IDataflowObject dataflow, IDataStructureObject dsd,
            string componentId, bool useCache = false, bool orderItems = false)
        {
            var codelistCostraints = new List<List<IStructureReference>>();

            var component = dsd.GetComponent(componentId);
            if (component == null)
            {
                _logger.LogDebug("component is null");
                component = dsd.Components.FirstOrDefault(x => x.ConceptRef.FullId == componentId);
            }

            var contrained = component.StructureType.EnumType != SdmxStructureEnumType.DataAttribute;
            _logger.LogDebug($"is contrained: {contrained}");

            var criterias = new List<IContentConstraintMutableObject>();
            if (contrained)
            {
                var currentComponent = component.ConceptRef.ChildReference.Id;

                IContentConstraintMutableObject criteria = new ContentConstraintMutableCore();
                criteria.Id = currentComponent ?? "SPECIAL";
                criteria.AddName("en", "english");
                criteria.AgencyId = "agency";

                ICubeRegionMutableObject region = new CubeRegionMutableCore();

                if (currentComponent != null)
                {
                    IKeyValuesMutable keyValue = new KeyValuesMutableImpl();
                    keyValue.Id = currentComponent;
                    keyValue.AddValue("FIXED_CUSTOM_REQUEST_DUMMY_VALUE");
                    region.AddKeyValue(keyValue);
                }

                criteria.IncludedCubeRegion = region;
                criterias.Add(criteria);
            }

            var codelistRef = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.CodeList));
            var dimension = component as IDimension;
            if (dimension != null && dimension.TimeDimension)
            {
                _logger.LogDebug("TimeDimension");
                codelistRef.MaintainableId = CustomCodelistConstants.TimePeriodCodeList;
                codelistRef.AgencyId = CustomCodelistConstants.Agency;
                codelistRef.Version = CustomCodelistConstants.Version;
            }
            else if (dimension != null && dimension.MeasureDimension && dsd is ICrossSectionalDataStructureObject)
            {
                _logger.LogDebug("ICrossSectionalDataStructureObject");
                var crossDsd = dsd as ICrossSectionalDataStructureObject;
                codelistRef.MaintainableId = crossDsd.GetCodelistForMeasureDimension(dimension.Id).MaintainableReference
                    .MaintainableId;
                codelistRef.AgencyId =
                    crossDsd.GetCodelistForMeasureDimension(dimension.Id).MaintainableReference.AgencyId;
                codelistRef.Version =
                    crossDsd.GetCodelistForMeasureDimension(dimension.Id).MaintainableReference.Version;
            }
            else
            {
                if (component.HasCodedRepresentation())
                {
                    _logger.LogDebug("HasCodedRepresentation");
                    codelistRef.MaintainableId =
                        component.Representation.Representation.MaintainableReference.MaintainableId;
                    codelistRef.AgencyId = component.Representation.Representation.MaintainableReference.AgencyId;
                    codelistRef.Version = component.Representation.Representation.MaintainableReference.Version;
                }
            }

            var info = string.Format(
                CultureInfo.InvariantCulture,
                InfoPartialCodelistFormat3,
                SDMXUtils.MakeKey(dataflow),
                component.ConceptRef,
                SDMXUtils.MakeKey(codelistRef));

            ICodelistObject codelist;

            var refs = new List<IStructureReference>
            {
                codelistRef
            };

            if (contrained)
            {
                var dataflowRef =
                    new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dataflow))
                    {
                        MaintainableId = dataflow.Id,
                        AgencyId = dataflow.AgencyId,
                        Version = dataflow.Version
                    };

                foreach (var criteria in criterias)
                {
                    _logger.LogTrace("item criteria");
                    var dataflowRefBean = new ConstrainableStructureReference(dataflowRef, criteria.ImmutableInstance);
                    _logger.LogTrace(string.Format(InfoGettingCodelistFormat1, info));
                    refs.Add(dataflowRefBean);
                }
            }

            codelistCostraints.Add(refs);


            return codelistCostraints;
        }

        /// <summary>
        ///     Retrieves all available categorisations and category schemes.
        /// </summary>
        public IComplexStructureQuery GetCategorySchemesAndCategorisations()
        {
            _logger.LogDebug($"START {MethodBase.GetCurrentMethod().Name}");


            var catSch =
                new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.CategoryScheme));
            IRestStructureQuery structureQueryCategoryScheme = new RESTStructureQueryCore(
                StructureQueryDetail.GetFromEnum(StructureQueryDetailEnumType.Full),
                StructureReferenceDetail.GetFromEnum(StructureReferenceDetailEnumType.Parents), null, catSch, false);
            IBuilder<IComplexStructureQuery, IRestStructureQuery> transformerCategoryScheme =
                new StructureQuery2ComplexQueryBuilder();

            var complexStructureQueryCategoryScheme = transformerCategoryScheme.Build(structureQueryCategoryScheme);

            _logger.LogDebug($"END {MethodBase.GetCurrentMethod().Name}");
            return complexStructureQueryCategoryScheme;
        }

        /// <summary>
        ///     Retrieves all available dataflows.
        /// </summary>
        public IComplexStructureQuery GetDataflows()
        {
            _logger.LogDebug($"START {MethodBase.GetCurrentMethod().Name}");

            var dataflowRefBean =
                new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dataflow));
            IRestStructureQuery structureQueryDataflow = new RESTStructureQueryCore(dataflowRefBean);

            IBuilder<IComplexStructureQuery, IRestStructureQuery> transformerDataFlow =
                new StructureQuery2ComplexQueryBuilder();

            var complexStructureQueryDataflow = transformerDataFlow.Build(structureQueryDataflow);

            IList<SdmxStructureType> specificObjects = new List<SdmxStructureType>
            {
                SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dsd)
            };

            IComplexStructureQueryMetadata complexStructureQueryMetadataWithDsd =
                new ComplexStructureQueryMetadataCore(false,
                    ComplexStructureQueryDetail.GetFromEnum(ComplexStructureQueryDetailEnumType.Full),
                    ComplexMaintainableQueryDetail.GetFromEnum(ComplexMaintainableQueryDetailEnumType.Full),
                    StructureReferenceDetail.GetFromEnum(StructureReferenceDetailEnumType.Specific),
                    specificObjects);

            IComplexStructureQuery complexStructureQueryTempDataflow = new ComplexStructureQueryCore(
                complexStructureQueryDataflow.StructureReference, complexStructureQueryMetadataWithDsd);

            _logger.LogDebug("send IComplexStructureQuery to sendQueryStructureRequestAsync");

            return complexStructureQueryTempDataflow;
        }

        public List<IStructureReference> GetCodeListCostraintFilter(IDataflowObject dataflow, IDataStructureObject dsd,
            string criteriaId, List<FilterCriteria> filterCriteria, DataflowDataRange timeRange)
        {
            var codelistRef = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.CodeList));

            foreach (var itemDim in dsd.Components)
            {
                if (!criteriaId.Equals(itemDim.Id, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }
                var dimension = itemDim as IDimension;
                if (dimension != null && dimension.TimeDimension)
                {
                    codelistRef.MaintainableId = CustomCodelistConstants.TimePeriodCodeList;
                    codelistRef.AgencyId = CustomCodelistConstants.Agency;
                    codelistRef.Version = CustomCodelistConstants.Version;
                    break;
                }
                else if (dimension != null && dimension.MeasureDimension && dsd is ICrossSectionalDataStructureObject)
                {
                    var crossDsd = dsd as ICrossSectionalDataStructureObject;
                    codelistRef.MaintainableId = crossDsd.GetCodelistForMeasureDimension(dimension.Id).MaintainableReference.MaintainableId;
                    codelistRef.AgencyId = crossDsd.GetCodelistForMeasureDimension(dimension.Id).MaintainableReference.AgencyId;
                    codelistRef.Version = crossDsd.GetCodelistForMeasureDimension(dimension.Id).MaintainableReference.Version;
                    break;
                }
                else
                {
                    if (itemDim.HasCodedRepresentation())
                    {
                        codelistRef.MaintainableId = itemDim.Representation.Representation.MaintainableReference.MaintainableId;
                        codelistRef.AgencyId = itemDim.Representation.Representation.MaintainableReference.AgencyId;
                        codelistRef.Version = itemDim.Representation.Representation.MaintainableReference.Version;
                        break;
                    }
                }
            }

            var refs = new List<IStructureReference>();
            refs.Add(codelistRef);

            var dataflowRef = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dataflow))
            {
                MaintainableId = dataflow.Id,
                AgencyId = dataflow.AgencyId,
                Version = dataflow.Version,
            };

            foreach (var itemDim in dsd.Components)
            {
                if (!criteriaId.Equals(itemDim.Id, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                string currentComponent = itemDim.ConceptRef.ChildReference.Id;

                IContentConstraintMutableObject criteriaObj = new ContentConstraintMutableCore();
                criteriaObj.Id = currentComponent ?? "SPECIAL";
                criteriaObj.AddName("en", "english");
                criteriaObj.AgencyId = "agency";

                ICubeRegionMutableObject region = new CubeRegionMutableCore();

                if (currentComponent != null)
                {
                    IKeyValuesMutable keyValue = new KeyValuesMutableImpl();
                    keyValue.Id = currentComponent;
                    keyValue.AddValue(Estat.Sri.CustomRequests.Constants.SpecialValues.DummyMemberValue);
                    region.AddKeyValue(keyValue);

                    if (filterCriteria != null)
                    {
                        foreach (var costreintKey in filterCriteria)
                        {
                            if (costreintKey.Type == FilterType.TimePeriod)
                            {
                                // Qui considerare il caso in qui in CodemapObj.PreviusCostraint[costreintKey][0] ci sia solo un valore, ke equivale alla data da.
                                if (costreintKey.From != null &&
                                    costreintKey.To != null)
                                {
                                    DateTime MinDate = GetDateTimeFromSDMXTimePeriod(costreintKey.From.Value.ToString("yyyy-MM-dd"), 'M');
                                    DateTime MaxDate = GetDateTimeFromSDMXTimePeriod(costreintKey.To.Value.ToString("yyyy-MM-dd"), 'M');

                                    if (MinDate.CompareTo(MaxDate) > 0)
                                    {
                                        criteriaObj.StartDate = MaxDate;
                                        criteriaObj.EndDate = MinDate;
                                    }
                                    else
                                    {
                                        criteriaObj.StartDate = MinDate;
                                        criteriaObj.EndDate = MaxDate;
                                    }

                                }
                            }
                            else if (costreintKey.Type == FilterType.TimeRange)
                            {
                                // Qui considerare il caso in qui in CodemapObj.PreviusCostraint[costreintKey][0] ci sia solo un valore, ke equivale alla data da.

                                DateTime MinDate = GetDateTimeFromSDMXTimePeriod(timeRange.CalcolateStartRangeFromPeriod().ToString("yyyy-MM-dd"), 'M');
                                DateTime MaxDate = GetDateTimeFromSDMXTimePeriod(timeRange.EndRange.ToString("yyyy-MM-dd"), 'M');

                                criteriaObj.StartDate = MinDate;
                                criteriaObj.EndDate = MaxDate;
                            }
                            else
                            {
                                foreach (var filterValue in costreintKey.FilterValues)
                                {
                                    IKeyValuesMutable _keyValue = new KeyValuesMutableImpl();
                                    _keyValue.Id = costreintKey.Id;
                                    _keyValue.AddValue(filterValue);
                                    region.AddKeyValue(_keyValue);
                                }
                            }
                        }
                    }
                }
                criteriaObj.IncludedCubeRegion = region;

                var dataflowRefBean = new ConstrainableStructureReference(dataflowRef, criteriaObj.ImmutableInstance);

                refs.Add(dataflowRefBean);

                break;
            }

            return refs;
        }

        private DateTime GetDateTimeFromSDMXTimePeriod(string SDMXTime, char FrequencyDominant)
        {

            string time_normal = string.Empty;

            if (!SDMXTime.Contains("-"))
            {
                time_normal = string.Format("{0}-{1}-{2}", SDMXTime, "01", "01");
            }
            else
            {
                var time_p_c = SDMXTime.Split('-');

                if (time_p_c.Length == 2)
                {
                    int mul =
                        (FrequencyDominant == 'M') ? 1 :
                        (FrequencyDominant == 'Q') ? 3 :
                        (FrequencyDominant == 'S') ? 6 : 0;


                    //int t_fix = (int.Parse(time_p_c[1].Substring(1))) * mul;
                    //time_normal = string.Format("{0}-{1}-{2}", time_p_c[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                    /*fabio 04/11/2015 v3.0.0.0*/
                    int t_fix = (int.Parse(time_p_c[1].Substring(1))) * mul;
                    string time_normal_qs = string.Format("{0}-{1}-{2}", time_p_c[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");

                    time_normal =
                        (FrequencyDominant == 'M') ? string.Format("{0}-{1}-{2}", time_p_c[0], time_p_c[1], "01") :
                        (FrequencyDominant == 'Q') ? time_normal_qs :
                        (FrequencyDominant == 'S') ? time_normal_qs : string.Format("{0}-{1}-{2}", time_p_c[0], time_p_c[1], "01");
                    /*fine fabio 04/11/2015 v3.0.0.0*/
                }


                if (SDMXTime.Contains("S"))
                {
                    var time_p = SDMXTime.Split('-');
                    int t_fix = (int.Parse(time_p[1].Substring(1))) * 6;
                    time_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                }
                if (SDMXTime.Contains("Q"))
                {
                    var time_p = SDMXTime.Split('-');
                    int t_fix = ((int.Parse(time_p[1].Substring(1))) * 3);
                    time_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                }
                if (SDMXTime.Contains("M"))
                {
                    var time_p = SDMXTime.Split('-');
                    int t_fix = (int.Parse(time_p[1].Substring(1))) * 1;
                    time_normal = string.Format("{0}-{1}-{2}", time_p[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                }
            }
            //CultureInfo enEn = new CultureInfo("en");
            //return DateTime.ParseExact(time_normal, "yyyy-MM-dd", enEn, DateTimeStyles.None);

            return DateTime.ParseExact(time_normal, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None);

        }

        public List<IStructureReference> GetFrequences(IDataflowObject dataflow, IDataStructureObject dsd, List<FilterCriteria> filterCriteria)
        {
            _logger.LogDebug($"START{MethodBase.GetCurrentMethod().Name}");

            var criteriaId = "";
            foreach (var component in dsd.Components)
            {
                var dimension = component as IDimension;
                if (dimension == null) continue;

                if (dimension.FrequencyDimension)
                {
                    criteriaId = dimension.Id;
                }
            }

            var codelistRef = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.CodeList));

            foreach (var itemDim in dsd.Components)
            {
                if (!criteriaId.Equals(itemDim.Id, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }
                var dimension = itemDim as IDimension;
                if (dimension != null && dimension.TimeDimension)
                {
                    codelistRef.MaintainableId = CustomCodelistConstants.TimePeriodCodeList;
                    codelistRef.AgencyId = CustomCodelistConstants.Agency;
                    codelistRef.Version = CustomCodelistConstants.Version;
                }
                else if (dimension != null && dimension.MeasureDimension && dsd is ICrossSectionalDataStructureObject)
                {
                    var crossDsd = dsd as ICrossSectionalDataStructureObject;
                    codelistRef.MaintainableId = crossDsd.GetCodelistForMeasureDimension(dimension.Id).MaintainableReference.MaintainableId;
                    codelistRef.AgencyId = crossDsd.GetCodelistForMeasureDimension(dimension.Id).MaintainableReference.AgencyId;
                    codelistRef.Version = crossDsd.GetCodelistForMeasureDimension(dimension.Id).MaintainableReference.Version;
                }
                else
                {
                    if (itemDim.HasCodedRepresentation())
                    {
                        codelistRef.MaintainableId = itemDim.Representation.Representation.MaintainableReference.MaintainableId;
                        codelistRef.AgencyId = itemDim.Representation.Representation.MaintainableReference.AgencyId;
                        codelistRef.Version = itemDim.Representation.Representation.MaintainableReference.Version;
                    }
                }
            }

            var refs = new List<IStructureReference>();
            refs.Add(codelistRef);

            if (filterCriteria != null &&
                filterCriteria.Count > 0)
            {
                var dataflowRef = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dataflow))
                {
                    MaintainableId = dataflow.Id,
                    AgencyId = dataflow.AgencyId,
                    Version = dataflow.Version,
                };

                foreach (var itemDim in dsd.Components)
                {
                    if (!criteriaId.Equals(itemDim.Id, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    string currentComponent = itemDim.ConceptRef.ChildReference.Id;

                    IContentConstraintMutableObject criteriaObj = new ContentConstraintMutableCore();
                    criteriaObj.Id = currentComponent ?? "SPECIAL";
                    criteriaObj.AddName("en", "english");
                    criteriaObj.AgencyId = "agency";

                    ICubeRegionMutableObject region = new CubeRegionMutableCore();

                    if (currentComponent != null)
                    {
                        IKeyValuesMutable keyValue = new KeyValuesMutableImpl();
                        keyValue.Id = currentComponent;
                        keyValue.AddValue(Estat.Sri.CustomRequests.Constants.SpecialValues.DummyMemberValue);
                        region.AddKeyValue(keyValue);

                        if (filterCriteria != null)
                        {
                            foreach (var costreintKey in filterCriteria)
                            {
                                if (costreintKey.Type == FilterType.TimePeriod ||
                                    costreintKey.Id.Equals(criteriaId))
                                {
                                    continue;
                                }
                                else
                                {
                                    foreach (var filterValue in costreintKey.FilterValues)
                                    {
                                        IKeyValuesMutable _keyValue = new KeyValuesMutableImpl();
                                        _keyValue.Id = costreintKey.Id;
                                        _keyValue.AddValue(filterValue);
                                        region.AddKeyValue(_keyValue);
                                    }
                                }
                            }
                        }

                    }
                    criteriaObj.IncludedCubeRegion = region;

                    var dataflowRefBean = new ConstrainableStructureReference(dataflowRef, criteriaObj.ImmutableInstance);

                    refs.Add(dataflowRefBean);

                    break;
                }
            }

            return refs;
        }
    }
}