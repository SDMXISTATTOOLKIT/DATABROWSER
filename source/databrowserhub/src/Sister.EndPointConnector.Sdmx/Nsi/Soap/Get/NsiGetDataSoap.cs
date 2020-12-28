using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using EndPointConnector.Interfaces.Sdmx.Models;
using EndPointConnector.Models;
using Estat.Sdmxsource.Extension.Builder;
using Estat.Sri.CustomRequests.Factory;
using Estat.Sri.CustomRequests.Manager;
using Estat.Sri.CustomRequests.Model;
using Microsoft.Extensions.Logging;
using Org.Sdmxsource.Sdmx.Api.Builder;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query.Complex;
using Org.Sdmxsource.Sdmx.Api.Model.Format;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Data.Query;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Registry;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;

namespace Sister.EndPointConnector.Sdmx.Nsi.Soap.Get
{
    public class NsiGetDataSoap
    {
        private const string dummyMemberValue = "FIXED_CUSTOM_REQUEST_DUMMY_VALUE";
        private readonly EndPointSdmxConfig _endPointSDMXNodeConfig;
        private readonly ILogger<NsiGetDataSoap> _logger;

        public NsiGetDataSoap(EndPointSdmxConfig endPointSDMXNodeConfig, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<NsiGetDataSoap>();
            _endPointSDMXNodeConfig = endPointSDMXNodeConfig;
        }

        public XmlDocument GetDataflowData(IDataflowObject df, IDataStructureObject kf,
            List<FilterCriteria> filterCriteria)
        {
            ISet<IDataQuerySelection> selections = new HashSet<IDataQuerySelection>();
            var startTime = string.Empty;
            var endTime = string.Empty;

            // Under the DataWhere only one child MUST reside.
            if (filterCriteria != null)
                foreach (var queryComponent in filterCriteria)
                    if (queryComponent != null)
                    {
                        if (!string.IsNullOrEmpty(queryComponent.Id) && queryComponent.Id != kf.TimeDimension.Id)
                        {
                            if (queryComponent.FilterValues.Count > 0 &&
                                !string.IsNullOrEmpty(queryComponent.FilterValues[0]))
                            {
                                ISet<string> valuern = new HashSet<string>();
                                foreach (var c in queryComponent.FilterValues)
                                    if (!string.IsNullOrEmpty(c))
                                        valuern.Add(c);
                                IDataQuerySelection selection =
                                    new DataQueryDimensionSelectionImpl(queryComponent.Id, valuern);
                                selections.Add(selection);
                            }
                        }
                        else if (!string.IsNullOrEmpty(queryComponent.Id) && queryComponent.Id == kf.TimeDimension.Id)
                        {
                            if (queryComponent.FilterValues.Count > 0 &&
                                !string.IsNullOrEmpty(queryComponent.FilterValues[0]))
                            {
                                startTime = queryComponent.FilterValues[0];
                                if (queryComponent.FilterValues.Count > 1 &&
                                    !string.IsNullOrEmpty(
                                        queryComponent.FilterValues[queryComponent.FilterValues.Count - 1]))
                                    endTime = queryComponent.FilterValues[queryComponent.FilterValues.Count - 1];
                            }
                        }
                    }

            IDataQuerySelectionGroup sel = new DataQuerySelectionGroupImpl(selections, null, null);
            if (string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                sel = new DataQuerySelectionGroupImpl(selections, new SdmxDateCore(endTime), new SdmxDateCore(endTime));
            else if (!string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
                sel = new DataQuerySelectionGroupImpl(selections, new SdmxDateCore(startTime),
                    new SdmxDateCore(startTime));
            else if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                sel = new DataQuerySelectionGroupImpl(selections, new SdmxDateCore(startTime),
                    new SdmxDateCore(endTime));
            IList<IDataQuerySelectionGroup> selGroup = new List<IDataQuerySelectionGroup>();
            selGroup.Add(sel);

            var maximumObservations = _endPointSDMXNodeConfig.MaxObservationsAfterCriteria;
            var query = new DataQueryFluentBuilder().Initialize(kf, df).WithOrderAsc(true)
                .WithMaxObservations(maximumObservations).WithDataQuerySelectionGroup(selGroup).Build();


            IDataQueryFormat<XDocument> queryFormat = new StructSpecificDataFormatV21();
            IBuilder<IComplexDataQuery, IDataQuery> transformer = new DataQuery2ComplexQueryBuilder(true);
            var complexDataQuery = transformer.Build(query);

            IComplexDataQueryBuilderManager complexDataQueryBuilderManager =
                new ComplexDataQueryBuilderManager(new ComplexDataQueryFactoryV21());
            var xdoc = complexDataQueryBuilderManager.BuildComplexDataQuery(complexDataQuery, queryFormat);
            var doc = new XmlDocument();
            doc.LoadXml(xdoc.ToString());

            return doc;
        }

        public List<IStructureReference> GetCountObservation(IDataflowObject df, IDataStructureObject kf,
            List<FilterCriteria> filterCriteria)
        {
            if (df == null)
                throw new InvalidOperationException("Dataflow is not set");

            var currentComponent = "CL_COUNT";

            IContentConstraintMutableObject criteria = new ContentConstraintMutableCore();
            criteria.Id = currentComponent;
            criteria.AddName("en", "english");
            criteria.AgencyId = "agency";
            ICubeRegionMutableObject region = new CubeRegionMutableCore();

            if (currentComponent != null)
            {
                IKeyValuesMutable keyValue = new KeyValuesMutableImpl();
                keyValue.Id = currentComponent;
                keyValue.AddValue(dummyMemberValue);
                region.AddKeyValue(keyValue);

                if (filterCriteria != null)
                    foreach (var itemCriteria in filterCriteria)
                    {
                        if (itemCriteria.Id == currentComponent) continue;
                        if (itemCriteria.Id == kf.TimeDimension.Id)
                        {
                            if (itemCriteria.FilterValues.Count > 1)
                            {
                                var minDate = getDateTimeFromSDMXTimePeriod(itemCriteria.FilterValues[0], 'M');
                                var maxDate = getDateTimeFromSDMXTimePeriod(itemCriteria.FilterValues[1], 'M');
                                if (minDate.CompareTo(maxDate) > 0)
                                {
                                    criteria.StartDate = maxDate;
                                    criteria.EndDate = minDate;
                                }
                                else
                                {
                                    criteria.StartDate = minDate;
                                    criteria.EndDate = maxDate;
                                }
                            }
                        }
                        else
                        {
                            foreach (var code in itemCriteria.FilterValues)
                            {
                                IKeyValuesMutable _keyValue = new KeyValuesMutableImpl
                                {
                                    Id = itemCriteria.Id
                                };
                                _keyValue.AddValue(code);
                                region.AddKeyValue(_keyValue);
                            }
                        }
                    }
            }

            criteria.IncludedCubeRegion = region;

            var criterias = new List<IContentConstraintMutableObject>();
            if (criteria == null) criteria = new ContentConstraintMutableCore();

            criteria.Id = CustomCodelistConstants.CountCodeList;
            criteria.AddName("en", "name");
            criteria.AgencyId = "agency";
            criterias.Add(criteria);

            var codelistRef = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.CodeList))
            {
                MaintainableId = CustomCodelistConstants.CountCodeList,
                AgencyId = CustomCodelistConstants.Agency,
                Version = CustomCodelistConstants.Version
            };

            var refs = new List<IStructureReference>();
            refs.Add(codelistRef);


            var dataflowRef = new StructureReferenceImpl(SdmxStructureType.GetFromEnum(SdmxStructureEnumType.Dataflow))
            {
                MaintainableId = df.Id,
                AgencyId = df.AgencyId,
                Version = df.Version
            };

            foreach (var itemCriteria in criterias)
            {
                var dataflowRefBean = new ConstrainableStructureReference(dataflowRef, itemCriteria.ImmutableInstance);

                refs.Add(dataflowRefBean);
            }

            return refs;
        }

        private DateTime getDateTimeFromSDMXTimePeriod(string sdmxTime, char frequencyDominant)
        {
            var time_normal = string.Empty;

            if (!sdmxTime.Contains("-"))
            {
                time_normal = string.Format("{0}-{1}-{2}", sdmxTime, "01", "01");
            }
            else
            {
                var time_p_c = sdmxTime.Split('-');

                if (time_p_c.Length == 2)
                {
                    var mul =
                        frequencyDominant == 'M' ? 1 :
                        frequencyDominant == 'Q' ? 3 :
                        frequencyDominant == 'S' ? 6 : 0;

                    var t_fix = int.Parse(time_p_c[1].Substring(1)) * mul;
                    var time_normal_qs = string.Format("{0}-{1}-{2}", time_p_c[0],
                        t_fix > 9 ? t_fix.ToString() : "0" + t_fix, "01");

                    time_normal =
                        frequencyDominant == 'M' ? string.Format("{0}-{1}-{2}", time_p_c[0], time_p_c[1], "01") :
                        frequencyDominant == 'Q' ? time_normal_qs :
                        frequencyDominant == 'S' ? time_normal_qs :
                        string.Format("{0}-{1}-{2}", time_p_c[0], time_p_c[1], "01");
                }


                if (sdmxTime.Contains("S"))
                {
                    var time_p = sdmxTime.Split('-');
                    var t_fix = int.Parse(time_p[1].Substring(1)) * 6;
                    time_normal = string.Format("{0}-{1}-{2}", time_p[0], t_fix > 9 ? t_fix.ToString() : "0" + t_fix,
                        "01");
                }

                if (sdmxTime.Contains("Q"))
                {
                    var time_p = sdmxTime.Split('-');
                    var t_fix = int.Parse(time_p[1].Substring(1)) * 3;
                    time_normal = string.Format("{0}-{1}-{2}", time_p[0], t_fix > 9 ? t_fix.ToString() : "0" + t_fix,
                        "01");
                }

                if (sdmxTime.Contains("M"))
                {
                    var time_p = sdmxTime.Split('-');
                    var t_fix = int.Parse(time_p[1].Substring(1)) * 1;
                    time_normal = string.Format("{0}-{1}-{2}", time_p[0], t_fix > 9 ? t_fix.ToString() : "0" + t_fix,
                        "01");
                }
            }

            return DateTime.ParseExact(time_normal, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None);
        }

        /*
            <StructureSpecificDataQuery
   xmlns="http://www.sdmx.org/resources/sdmxml/schemas/v2_1/message">
   <Header>
       <ID>IDREF12</ID>
       <Test>false</Test>
       <Prepared>2020-03-31T16:19:11.1647659+02:00</Prepared>
       <Sender id="Unknown" />
       <Receiver id="Unknown" />
   </Header>
   <Query>
       <ReturnDetails defaultLimit="2147483647" detail="Full" observationAction="Active"
           xmlns="http://www.sdmx.org/resources/sdmxml/schemas/v2_1/query">
           <Structure dimensionAtObservation="TIME_PERIOD" structureID="StructureId">
               <Structure
                   xmlns="http://www.sdmx.org/resources/sdmxml/schemas/v2_1/common">
                   <Ref agencyID="IT1" id="DCSC_INDTRAEREO" version="1.2"
                       xmlns="" />
                   </Structure>
               </Structure>
           </ReturnDetails>
           <DataWhere
               xmlns="http://www.sdmx.org/resources/sdmxml/schemas/v2_1/query">
               <Dataflow>
                   <Ref agencyID="IT1" id="DF_TR_AEREO" version="1.0"
                       xmlns="" />
                   </Dataflow>
                   <TimeDimensionValue>
                       <TimeValue operator="greaterThanOrEqual">2003</TimeValue>
                       <TimeValue operator="lessThanOrEqual">2018</TimeValue>
                   </TimeDimensionValue>
                   <Or>
                       <DimensionValue>
                           <ID>ITTER107</ID>
                           <Value operator="equal">IT</Value>
                       </DimensionValue>
                   </Or>
                   <Or>
                       <DimensionValue>
                           <ID>AEROPORTI</ID>
                           <Value operator="equal">GRS</Value>
                       </DimensionValue>
                   </Or>
               </DataWhere>
           </Query>
       </StructureSpecificDataQuery> 
            */
        //TODO get compact data
    }
}