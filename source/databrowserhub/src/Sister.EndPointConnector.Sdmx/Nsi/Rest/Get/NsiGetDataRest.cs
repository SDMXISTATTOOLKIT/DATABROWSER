using EndPointConnector.Interfaces.Sdmx.Models;
using EndPointConnector.Interfaces.Sdmx.Nsi.Models;
using EndPointConnector.Models;
using Microsoft.Extensions.Logging;
using Org.Sdmxsource.Sdmx.Api.Engine;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Data.Query;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.SdmxQueryBuilder.Factory;
using Org.Sdmxsource.Sdmx.SdmxQueryBuilder.Manager;
using Org.Sdmxsource.Sdmx.SdmxQueryBuilder.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;

namespace Sister.EndPointConnector.Sdmx.Nsi.Rest.Get
{
    public class NsiGetDataRest
    {
        private readonly EndPointSdmxConfig _endPointSDMXNodeConfig;
        private readonly ILogger<NsiGetDataRest> _logger;

        public NsiGetDataRest(ILoggerFactory loggerFactory,
            EndPointSdmxConfig endPointSDMXNodeConfig)
        {
            _logger = loggerFactory.CreateLogger<NsiGetDataRest>();
            _endPointSDMXNodeConfig = endPointSDMXNodeConfig;
        }

        public DataflowDataRequest GetDataflowData(IDataflowObject df, IDataStructureObject kf,
            List<FilterCriteria> filterCriteria, DataflowDataRange dataflowDataRange)
        {
            ISet<IDataQuerySelection> selections = new HashSet<IDataQuerySelection>();
            var startTime = string.Empty;
            var endTime = string.Empty;

            // Under the DataWhere only one child MUST reside.
            if (filterCriteria != null)
            {
                foreach (var queryComponent in filterCriteria)
                {
                    if (queryComponent != null)
                    {
                        if (string.IsNullOrEmpty(queryComponent.Id))
                        {
                            continue;
                        }

                        if (queryComponent.Type == FilterType.CodeValues ||
                            queryComponent.Type == FilterType.StringValues)
                        {
                            if (queryComponent.FilterValues != null && queryComponent.FilterValues.Count > 0 &&
                                !string.IsNullOrEmpty(queryComponent.FilterValues[0]))
                            {
                                ISet<string> valuern = new HashSet<string>();
                                foreach (var c in queryComponent.FilterValues)
                                {
                                    if (!string.IsNullOrEmpty(c))
                                    {
                                        valuern.Add(c);
                                    }
                                }

                                IDataQuerySelection selection =
                                    new DataQueryDimensionSelectionImpl(queryComponent.Id, valuern);
                                selections.Add(selection);
                            }
                        }
                        else if (queryComponent.Type == FilterType.TimeRange)
                        {
                            if (queryComponent.From.HasValue)
                            {
                                startTime = queryComponent.From.Value.ToString("yyyy-MM-dd");
                                if (queryComponent.To.HasValue)
                                {
                                    endTime = queryComponent.To.Value.ToString("yyyy-MM-dd");
                                }
                            }
                        }
                        else if (queryComponent.Type == FilterType.TimePeriod)
                        {
                            startTime = dataflowDataRange.CalcolateStartRangeFromPeriod().ToString("yyyy-MM-dd");
                            endTime = dataflowDataRange.EndRange.ToString("yyyy-MM-dd");
                        }
                    }
                }
            }

            IDataQuerySelectionGroup sel = new DataQuerySelectionGroupImpl(selections, null, null);
            if (string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                sel = new DataQuerySelectionGroupImpl(selections, new SdmxDateCore(endTime), new SdmxDateCore(endTime));
            }
            else if (!string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
            {
                sel = new DataQuerySelectionGroupImpl(selections, new SdmxDateCore(startTime),
                    new SdmxDateCore(startTime));
            }
            else if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                sel = new DataQuerySelectionGroupImpl(selections, new SdmxDateCore(startTime),
                    new SdmxDateCore(endTime));
            }

            IList<IDataQuerySelectionGroup> selGroup = new List<IDataQuerySelectionGroup>
            {
                sel
            };
            //IDataQuery query = new DataQueryFluentBuilder().Initialize(kf, df).WithDataQuerySelectionGroup(selGroup).WithDimensionAtObservation("AllDimensions").Build();
            var query = new DataQueryFluentBuilder().Initialize(kf, df).WithDataQuerySelectionGroup(selGroup).Build();

            var structureQueryFormat = new RestQueryFormat();
            var dataQueryFactory = new DataQueryFactory();
            var dataQueryBuilderManager = new DataQueryBuilderManager(dataQueryFactory);
            var request = dataQueryBuilderManager.BuildDataQuery(query, structureQueryFormat);

            if (_endPointSDMXNodeConfig.SupportPostFilters)
            {
                return generatePostRequest(request);
            }

            return new DataflowDataRequest { HttpMethod = HttpMethod.Get, QueryString = request };
        }



        private DataflowDataRequest generatePostRequest(string request)
        {
            var requestSplit = request.Split("/");
            string queryString = null;
            if (requestSplit.Length > 1)
            {
                queryString = $"{requestSplit[0]}/{requestSplit[1]}/body";
            }

            if (requestSplit.Length > 3 &&
                requestSplit[^1].Contains("?"))
            {
                queryString += $"{requestSplit[^1]}";
            }

            Dictionary<string, string> keyForm = null;
            if (requestSplit.Length > 2)
            {
                keyForm = new Dictionary<string, string> { { "key", requestSplit[2] } };
            }

            return new DataflowDataRequest
            {
                HttpMethod = HttpMethod.Post,
                QueryString = queryString,
                Keys = keyForm,
                ContentType = "application/x-www-form-urlencoded"
            };
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


                    //int t_fix = (int.Parse(time_p_c[1].Substring(1))) * mul;
                    //time_normal = string.Format("{0}-{1}-{2}", time_p_c[0], (t_fix > 9) ? t_fix.ToString() : "0" + t_fix.ToString(), "01");
                    /*fabio 04/11/2015 v3.0.0.0*/
                    var t_fix = int.Parse(time_p_c[1].Substring(1)) * mul;
                    var time_normal_qs = string.Format("{0}-{1}-{2}", time_p_c[0],
                        t_fix > 9 ? t_fix.ToString() : "0" + t_fix, "01");

                    time_normal =
                        frequencyDominant == 'M' ? string.Format("{0}-{1}-{2}", time_p_c[0], time_p_c[1], "01") :
                        frequencyDominant == 'Q' ? time_normal_qs :
                        frequencyDominant == 'S' ? time_normal_qs :
                        string.Format("{0}-{1}-{2}", time_p_c[0], time_p_c[1], "01");
                    /*fine fabio 04/11/2015 v3.0.0.0*/
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
            //CultureInfo enEn = new CultureInfo("en");
            //return DateTime.ParseExact(time_normal, "yyyy-MM-dd", enEn, DateTimeStyles.None);

            return DateTime.ParseExact(time_normal, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None);
        }

        private void readData(IDataReaderEngine dataReader, IDataflowObject df, IDataStructureObject kf)
        {
            var isTimeSeries = kf.TimeDimension != null;


            while (dataReader.MoveNextKeyable())
            {
                // In DatasetAttributes ci sono gli attributi a livello di dataset
                foreach (var key in dataReader.DatasetAttributes)
                {
                    //this.DataSetStore.AddToStore(key.Concept, key.Code);
                }

                // In CurrentKey.Key ci sono le dimensioni
                foreach (var key in dataReader.CurrentKey.Key)
                {
                    //this.DataSetStore.AddToStore(key.Concept, key.Code);
                }

                // In CurrentKey.Attributes ci sono gli attributi a livello di serie
                foreach (var key in dataReader.CurrentKey.Attributes)
                {
                    //this.DataSetStore.AddToStore(key.Concept, key.Code);
                }

                while (dataReader.MoveNextObservation())
                {
                    if (isTimeSeries)
                    {
                        //this.DataSetStore.AddToStore(DimensionObject.TimeDimensionFixedId, dataReader.CurrentObservation.ObsTime);
                    }

                    if (dataReader.CurrentObservation.CrossSection)
                    {
                        //this.DataSetStore.AddToStore(dataReader.CurrentObservation.CrossSectionalValue.Concept, dataReader.CurrentObservation.CrossSectionalValue.Code);
                    }

                    // In CurrentObservation.Attributes ci sono gli attributi a livello di osservazione
                    foreach (var key in dataReader.CurrentObservation.Attributes)
                    {
                        //this.DataSetStore.AddToStore(key.Concept, key.Code);
                    }

                    //this.DataSetStore.AddToStore(PrimaryMeasure.FixedId, dataReader.CurrentObservation.ObservationValue);

                    //this.DataSetStore.AddRow();
                }
            }
        }
    }
}