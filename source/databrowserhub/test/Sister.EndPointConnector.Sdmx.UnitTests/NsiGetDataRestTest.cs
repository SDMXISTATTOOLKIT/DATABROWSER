using EndPointConnector.Interfaces.Sdmx.Models;
using EndPointConnector.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.MetadataStructure;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using Sister.EndPointConnector.Sdmx.Nsi.Rest.Get;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace Sister.EndPointConnector.Sdmx.UnitTests
{
    public class NsiGetDataRestTest
    {
        [Fact]
        public void GetStructure_DataflowDataGet_Ok()
        {
            var nsiGetDataRest = new NsiGetDataRest(NullLoggerFactory.Instance, new EndPointSdmxConfig());

            var dfMutable = new DataflowMutableCore
            {
                Id = "DFId",
                AgencyId = "DFAg",
                Version = "1.1"
            };
            dfMutable.AddName("IT", "testname");
            dfMutable.DataStructureRef = new StructureReferenceImpl("DSDAg", "DSDId", "1.1", SdmxStructureEnumType.Dsd);
            var dsdMutable = new DataStructureMutableCore
            {
                Id = "DSDId",
                AgencyId = "DSDAg",
                Version = "1.1"
            };
            dsdMutable.AddName("IT", "testname");
            dsdMutable.AddPrimaryMeasure(new StructureReferenceImpl("AG", "ID", "2.5", SdmxStructureEnumType.Concept,
                "PR"));
            IDimensionMutableObject dimension = new DimensionMutableCore
            {
                ConceptRef =
                new StructureReferenceImpl("AgDim1", "IdDim1", "2.1", SdmxStructureEnumType.Concept, "ID1"),
                Representation = new RepresentationMutableCore
                {
                    Representation =
                    new StructureReferenceImpl("AgDim1Cl", "IdDim1Cl", "2.1", SdmxStructureEnumType.CodeList)
                }
            };
            dsdMutable.AddDimension(dimension);
            dimension = new DimensionMutableCore
            {
                ConceptRef =
                new StructureReferenceImpl("AgDim3", "IdDim3", "3.3", SdmxStructureEnumType.Concept, "ID3"),
                Representation = new RepresentationMutableCore
                {
                    Representation =
                    new StructureReferenceImpl("AgDim1Cl", "IdDim1Cl", "2.1", SdmxStructureEnumType.CodeList)
                }
            };
            dsdMutable.AddDimension(dimension);
            dimension = new DimensionMutableCore
            {
                ConceptRef =
                new StructureReferenceImpl("AgDim2", "IdDim2", "3.1", SdmxStructureEnumType.Concept, "ID2"),
                Representation = new RepresentationMutableCore
                {
                    Representation =
                    new StructureReferenceImpl("AgDim2Cl", "IdDim2Cl", "2.2", SdmxStructureEnumType.CodeList)
                }
            };
            dsdMutable.AddDimension(dimension);
            dimension = new DimensionMutableCore
            {
                ConceptRef = new StructureReferenceImpl("AgIDTimeDim", "IdIDTimeDim", "1.0",
                SdmxStructureEnumType.Concept, "IDTimeDim"),
                Representation = new RepresentationMutableCore
                {
                    Representation = new StructureReferenceImpl("AgIDTimeDimCl", "IdIDTimeDimCl", "1.2",
                    SdmxStructureEnumType.CodeList)
                },
                TimeDimension = true
            };
            dsdMutable.AddDimension(dimension);

            var filterCriteria = new List<FilterCriteria>
            {
                new FilterCriteria {Id = "ID1", FilterValues = new List<string> {"valueA"}},
                new FilterCriteria {Id = "ID2", FilterValues = new List<string> {"valueA2", "valueB2"}},
                new FilterCriteria {Id = "ID3", FilterValues = new List<string> {"valueA3", "valueB3", "valueC3"}},
                new FilterCriteria {Id = "TIME_PERIOD", FilterValues = new List<string> {"2020-01-03"}},
                new FilterCriteria {Id = "TIME_PERIOD", Type = FilterType.TimeRange, From = new System.DateTime(2020,1,3)}
            };

            var results = nsiGetDataRest.GetDataflowData(dfMutable.ImmutableInstance, dsdMutable.ImmutableInstance,
                filterCriteria, null);

            Assert.Equal(
                "data/DFAg,DFId,1.1/valueA.valueA3+valueB3+valueC3.valueA2+valueB2/ALL/?detail=full&startPeriod=2020-01-03&endPeriod=2020-01-03&dimensionAtObservation=TIME_PERIOD",
                results.QueryString);


            filterCriteria = new List<FilterCriteria>
            {
                new FilterCriteria {Id = "ID1", FilterValues = new List<string> {"valueA"}},
                new FilterCriteria {Id = "ID2", FilterValues = new List<string> {"valueA2", "valueB2"}},
                new FilterCriteria {Id = "ID3", FilterValues = new List<string> {"valueA3", "valueB3", "valueC3"}}
            };

            results = nsiGetDataRest.GetDataflowData(dfMutable.ImmutableInstance, dsdMutable.ImmutableInstance,
                filterCriteria, null);

            Assert.Equal(
                "data/DFAg,DFId,1.1/valueA.valueA3+valueB3+valueC3.valueA2+valueB2/ALL/?detail=full&dimensionAtObservation=TIME_PERIOD",
                results.QueryString);


            filterCriteria = new List<FilterCriteria>
            {
                new FilterCriteria {Id = "ID1", FilterValues = new List<string> {"valueA"}},
                new FilterCriteria {Id = "ID2", FilterValues = new List<string> {"valueA2", "valueB2"}},
                new FilterCriteria {Id = "ID3", FilterValues = new List<string> {"valueA3", "valueB3", "valueC3"}},
                new FilterCriteria {Id = "TIME_PERIOD", Type = FilterType.TimeRange, From = new System.DateTime(2020,1,3), To = new System.DateTime(2022,12,25)}
            };

            results = nsiGetDataRest.GetDataflowData(dfMutable.ImmutableInstance, dsdMutable.ImmutableInstance,
                filterCriteria, null);

            Assert.Equal(
                "data/DFAg,DFId,1.1/valueA.valueA3+valueB3+valueC3.valueA2+valueB2/ALL/?detail=full&startPeriod=2020-01-03&endPeriod=2022-12-25&dimensionAtObservation=TIME_PERIOD",
                results.QueryString);


            filterCriteria = new List<FilterCriteria>
            {
                new FilterCriteria {Id = "ID1", FilterValues = new List<string> {"valueA"}},
                new FilterCriteria {Id = "ID2", FilterValues = new List<string> {"valueA2", "valueB2"}}
            };

            results = nsiGetDataRest.GetDataflowData(dfMutable.ImmutableInstance, dsdMutable.ImmutableInstance,
                filterCriteria, null);
            Assert.Equal(
                "data/DFAg,DFId,1.1/valueA..valueA2+valueB2/ALL/?detail=full&dimensionAtObservation=TIME_PERIOD",
                results.QueryString);
        }

        [Fact]
        public void GetStructure_DataflowDataPost_Ok()
        {
            var nsiGetDataRest = new NsiGetDataRest(NullLoggerFactory.Instance,
                new EndPointSdmxConfig { SupportPostFilters = true });

            var dfMutable = new DataflowMutableCore
            {
                Id = "DFId",
                AgencyId = "DFAg",
                Version = "1.1"
            };
            dfMutable.AddName("IT", "testname");
            dfMutable.DataStructureRef = new StructureReferenceImpl("DSDAg", "DSDId", "1.1", SdmxStructureEnumType.Dsd);
            var dsdMutable = new DataStructureMutableCore
            {
                Id = "DSDId",
                AgencyId = "DSDAg",
                Version = "1.1"
            };
            dsdMutable.AddName("IT", "testname");
            dsdMutable.AddPrimaryMeasure(new StructureReferenceImpl("AG", "ID", "2.5", SdmxStructureEnumType.Concept,
                "PR"));
            IDimensionMutableObject dimension = new DimensionMutableCore
            {
                ConceptRef =
                new StructureReferenceImpl("AgDim1", "IdDim1", "2.1", SdmxStructureEnumType.Concept, "ID1"),
                Representation = new RepresentationMutableCore
                {
                    Representation =
                    new StructureReferenceImpl("AgDim1Cl", "IdDim1Cl", "2.1", SdmxStructureEnumType.CodeList)
                }
            };
            dsdMutable.AddDimension(dimension);
            dimension = new DimensionMutableCore
            {
                ConceptRef =
                new StructureReferenceImpl("AgDim3", "IdDim3", "3.3", SdmxStructureEnumType.Concept, "ID3"),
                Representation = new RepresentationMutableCore
                {
                    Representation =
                    new StructureReferenceImpl("AgDim1Cl", "IdDim1Cl", "2.1", SdmxStructureEnumType.CodeList)
                }
            };
            dsdMutable.AddDimension(dimension);
            dimension = new DimensionMutableCore
            {
                ConceptRef =
                new StructureReferenceImpl("AgDim2", "IdDim2", "3.1", SdmxStructureEnumType.Concept, "ID2"),
                Representation = new RepresentationMutableCore
                {
                    Representation =
                    new StructureReferenceImpl("AgDim2Cl", "IdDim2Cl", "2.2", SdmxStructureEnumType.CodeList)
                }
            };
            dsdMutable.AddDimension(dimension);
            dimension = new DimensionMutableCore
            {
                ConceptRef = new StructureReferenceImpl("AgIDTimeDim", "IdIDTimeDim", "1.0",
                SdmxStructureEnumType.Concept, "IDTimeDim"),
                Representation = new RepresentationMutableCore
                {
                    Representation = new StructureReferenceImpl("AgIDTimeDimCl", "IdIDTimeDimCl", "1.2",
                    SdmxStructureEnumType.CodeList)
                },
                TimeDimension = true
            };
            dsdMutable.AddDimension(dimension);

            var filterCriteria = new List<FilterCriteria>
            {
                new FilterCriteria {Id = "ID1", FilterValues = new List<string> {"valueA"}},
                new FilterCriteria {Id = "ID2", FilterValues = new List<string> {"valueA2", "valueB2"}},
                new FilterCriteria {Id = "ID3", FilterValues = new List<string> {"valueA3", "valueB3", "valueC3"}},
                new FilterCriteria {Id = "TIME_PERIOD", Type = FilterType.TimeRange, From = new System.DateTime(2020,1,3)}
            };

            var results = nsiGetDataRest.GetDataflowData(dfMutable.ImmutableInstance, dsdMutable.ImmutableInstance,
                filterCriteria, null);

            Assert.Equal(
                "data/DFAg,DFId,1.1/body?detail=full&startPeriod=2020-01-03&endPeriod=2020-01-03&dimensionAtObservation=TIME_PERIOD",
                results.QueryString);
            Assert.Equal(HttpMethod.Post, results.HttpMethod);
            Assert.Equal(1, results.Keys.Count);
            Assert.Equal("valueA.valueA3+valueB3+valueC3.valueA2+valueB2", results.Keys["key"]);

            filterCriteria = new List<FilterCriteria>
            {
                new FilterCriteria {Id = "ID1", FilterValues = new List<string> {"valueA"}},
                new FilterCriteria {Id = "ID2", FilterValues = new List<string> {"valueA2", "valueB2"}},
                new FilterCriteria {Id = "ID3", FilterValues = new List<string> {"valueA3", "valueB3", "valueC3"}}
            };

            results = nsiGetDataRest.GetDataflowData(dfMutable.ImmutableInstance, dsdMutable.ImmutableInstance,
                filterCriteria, null);

            Assert.Equal("data/DFAg,DFId,1.1/body?detail=full&dimensionAtObservation=TIME_PERIOD", results.QueryString);
            Assert.Equal(HttpMethod.Post, results.HttpMethod);
            Assert.Equal(1, results.Keys.Count);
            Assert.Equal("valueA.valueA3+valueB3+valueC3.valueA2+valueB2", results.Keys["key"]);

            filterCriteria = new List<FilterCriteria>
            {
                new FilterCriteria {Id = "ID1", FilterValues = new List<string> {"valueA"}},
                new FilterCriteria {Id = "ID2", FilterValues = new List<string> {"valueA2", "valueB2"}},
                new FilterCriteria {Id = "ID3", FilterValues = new List<string> {"valueA3", "valueB3", "valueC3"}},
                new FilterCriteria {Id = "TIME_PERIOD", Type = FilterType.TimeRange, From = new System.DateTime(2020,1,3), To = new System.DateTime(2022,12,25)}
            };

            results = nsiGetDataRest.GetDataflowData(dfMutable.ImmutableInstance, dsdMutable.ImmutableInstance,
                filterCriteria, null);

            Assert.Equal(
                "data/DFAg,DFId,1.1/body?detail=full&startPeriod=2020-01-03&endPeriod=2022-12-25&dimensionAtObservation=TIME_PERIOD",
                results.QueryString);
            Assert.Equal(HttpMethod.Post, results.HttpMethod);
            Assert.Equal(1, results.Keys.Count);
            Assert.Equal("valueA.valueA3+valueB3+valueC3.valueA2+valueB2", results.Keys["key"]);

            filterCriteria = new List<FilterCriteria>
            {
                new FilterCriteria {Id = "ID1", FilterValues = new List<string> {"valueA"}},
                new FilterCriteria {Id = "ID2", FilterValues = new List<string> {"valueA2", "valueB2"}}
            };

            results = nsiGetDataRest.GetDataflowData(dfMutable.ImmutableInstance, dsdMutable.ImmutableInstance,
                filterCriteria, null);
            Assert.Equal("data/DFAg,DFId,1.1/body?detail=full&dimensionAtObservation=TIME_PERIOD", results.QueryString);
            Assert.Equal(HttpMethod.Post, results.HttpMethod);
            Assert.Equal(1, results.Keys.Count);
            Assert.Equal("valueA..valueA2+valueB2", results.Keys["key"]);
        }
    }
}