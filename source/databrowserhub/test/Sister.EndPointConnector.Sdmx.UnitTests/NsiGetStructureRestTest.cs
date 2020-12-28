using System.Collections.Generic;
using System.Linq;
using EndPointConnector.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.MetadataStructure;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using Sister.EndPointConnector.Sdmx.Nsi.Rest.Get;
using Xunit;

namespace Sister.EndPointConnector.Sdmx.UnitTests
{
    public class NsiGetStructureRestTest
    {
        [Fact]
        public void GetStructure_CategorySchemesAndCategorisations_Ok()
        {
            var nsiGetArtefactRest = new NsiGetStructureRest(NullLoggerFactory.Instance);

            var result = nsiGetArtefactRest.GetCategorySchemesAndCategorisations();

            Assert.Equal("categoryscheme/all/all/all/?references=parents&detail=full", result);
        }

        [Fact]
        public void GetStructure_Dataflows_Ok()
        {
            var nsiGetArtefactRest = new NsiGetStructureRest(NullLoggerFactory.Instance);

            var result = nsiGetArtefactRest.GetDataflows(false);

            Assert.Equal("dataflow/all/all/all/?references=none&detail=full", result);
        }

        [Fact]
        public void GetStructure_DataflowsWithChildren_Ok()
        {
            var nsiGetArtefactRest = new NsiGetStructureRest(NullLoggerFactory.Instance);

            var result = nsiGetArtefactRest.GetDataflows(true);

            Assert.Equal("dataflow/all/all/all/?references=children&detail=full", result);
        }

        [Fact]
        public void GetStructure_CodeListsCostraint_Ok()
        {
            var nsiGetArtefactRest = new NsiGetStructureRest(NullLoggerFactory.Instance);

            var dfMutable = new DataflowMutableCore();
            dfMutable.Id = "DFId";
            dfMutable.AgencyId = "DFAg";
            dfMutable.Version = "1.1";
            dfMutable.AddName("IT", "testname");
            dfMutable.DataStructureRef = new StructureReferenceImpl("DSDAg", "DSDId", "1.1", SdmxStructureEnumType.Dsd);
            var dsdMutable = new DataStructureMutableCore();
            dsdMutable.Id = "DSDId";
            dsdMutable.AgencyId = "DSDAg";
            dsdMutable.Version = "1.1";
            dsdMutable.AddName("IT", "testname");
            dsdMutable.AddPrimaryMeasure(new StructureReferenceImpl("AG", "ID", "2.5", SdmxStructureEnumType.Concept,
                "PR"));
            IDimensionMutableObject dimension = new DimensionMutableCore();
            dimension.ConceptRef =
                new StructureReferenceImpl("AgDim1", "IdDim1", "2.1", SdmxStructureEnumType.Concept, "ID1");
            dimension.Representation = new RepresentationMutableCore
            {
                Representation =
                    new StructureReferenceImpl("AgDim1Cl", "IdDim1Cl", "2.1", SdmxStructureEnumType.CodeList)
            };
            dsdMutable.AddDimension(dimension);
            dimension = new DimensionMutableCore();
            dimension.ConceptRef =
                new StructureReferenceImpl("AgDim3", "IdDim3", "3.3", SdmxStructureEnumType.Concept, "ID3");
            dimension.Representation = new RepresentationMutableCore
            {
                Representation =
                    new StructureReferenceImpl("AgDim1Cl", "IdDim1Cl", "2.1", SdmxStructureEnumType.CodeList)
            };
            dsdMutable.AddDimension(dimension);
            dimension = new DimensionMutableCore();
            dimension.ConceptRef =
                new StructureReferenceImpl("AgDim2", "IdDim2", "3.1", SdmxStructureEnumType.Concept, "ID2");
            dimension.Representation = new RepresentationMutableCore
            {
                Representation =
                    new StructureReferenceImpl("AgDim2Cl", "IdDim2Cl", "2.2", SdmxStructureEnumType.CodeList)
            };
            dsdMutable.AddDimension(dimension);
            dimension = new DimensionMutableCore();
            dimension.ConceptRef = new StructureReferenceImpl("AgIDTimeDim", "IdIDTimeDim", "1.0",
                SdmxStructureEnumType.Concept, "TIME_PERIOD");
            dimension.Representation = new RepresentationMutableCore
            {
                Representation = new StructureReferenceImpl("AgIDTimeDimCl", "IdIDTimeDimCl", "1.2",
                    SdmxStructureEnumType.CodeList)
            };
            dimension.TimeDimension = true;
            dsdMutable.AddDimension(dimension);
            var components = new List<string> {"ID1", "ID2", "ID3", "TIME_PERIOD"};

            var results =
                nsiGetArtefactRest.GetCodeListCostraint(dfMutable.ImmutableInstance, dsdMutable.ImmutableInstance,
                    "ID2");

            Assert.Equal(5, results.Count);
            Assert.Empty(results[ComponentType.EnumComponentType.FrequencyDimension]);
            Assert.Empty(results[ComponentType.EnumComponentType.MeasureDimension]);
            Assert.Empty(results[ComponentType.EnumComponentType.Attribute]);
            Assert.Empty(results[ComponentType.EnumComponentType.TimeDimension]);
            Assert.Single(results[ComponentType.EnumComponentType.Dimension]);
        }

        [Fact]
        public void GetStructure_SingleCodeListDimension_Ok()
        {
            var nsiGetArtefactRest = new NsiGetStructureRest(NullLoggerFactory.Instance);

            var dfMutable = new DataflowMutableCore();
            dfMutable.Id = "DFId";
            dfMutable.AgencyId = "DFAg";
            dfMutable.Version = "1.1";
            dfMutable.AddName("IT", "testname");
            dfMutable.DataStructureRef = new StructureReferenceImpl("DSDAg", "DSDId", "1.1", SdmxStructureEnumType.Dsd);
            var dsdMutable = new DataStructureMutableCore();
            dsdMutable.Id = "DSDId";
            dsdMutable.AgencyId = "DSDAg";
            dsdMutable.Version = "1.1";
            dsdMutable.AddName("IT", "testname");
            dsdMutable.AddPrimaryMeasure(new StructureReferenceImpl("AG", "ID", "2.5", SdmxStructureEnumType.Concept,
                "PR"));
            IDimensionMutableObject dimension = new DimensionMutableCore();
            dimension.ConceptRef =
                new StructureReferenceImpl("AgDim1", "IdDim1", "2.1", SdmxStructureEnumType.Concept, "ID1");
            dimension.Representation = new RepresentationMutableCore
            {
                Representation =
                    new StructureReferenceImpl("AgDim1Cl", "IdDim1Cl", "2.1", SdmxStructureEnumType.CodeList)
            };
            dsdMutable.AddDimension(dimension);
            var components = new List<string> {"ID1"};

            var results =
                nsiGetArtefactRest.GetCodeListCostraint(dfMutable.ImmutableInstance, dsdMutable.ImmutableInstance,
                    "ID1");

            Assert.Equal(5, results.Count);
            Assert.Empty(results[ComponentType.EnumComponentType.FrequencyDimension]);
            Assert.Empty(results[ComponentType.EnumComponentType.MeasureDimension]);
            Assert.Empty(results[ComponentType.EnumComponentType.Attribute]);
            Assert.Empty(results[ComponentType.EnumComponentType.TimeDimension]);
            Assert.Single(results[ComponentType.EnumComponentType.Dimension]);
            Assert.Equal("availableconstraint/DFAg,DFId,1.1/ALL/ALL/ID1?references=codelist",
                results[ComponentType.EnumComponentType.Dimension].FirstOrDefault());
        }

        [Fact]
        public void GetStructure_SingleCodeListTime_Ok()
        {
            var nsiGetArtefactRest = new NsiGetStructureRest(NullLoggerFactory.Instance);

            var dfMutable = new DataflowMutableCore();
            dfMutable.Id = "DFId";
            dfMutable.AgencyId = "DFAg";
            dfMutable.Version = "1.1";
            dfMutable.AddName("IT", "testname");
            dfMutable.DataStructureRef = new StructureReferenceImpl("DSDAg", "DSDId", "1.1", SdmxStructureEnumType.Dsd);
            var dsdMutable = new DataStructureMutableCore();
            dsdMutable.Id = "DSDId";
            dsdMutable.AgencyId = "DSDAg";
            dsdMutable.Version = "1.1";
            dsdMutable.AddName("IT", "testname");
            dsdMutable.AddPrimaryMeasure(new StructureReferenceImpl("AG", "ID", "2.5", SdmxStructureEnumType.Concept,
                "PR"));
            IDimensionMutableObject dimension = new DimensionMutableCore();
            dimension.ConceptRef =
                new StructureReferenceImpl("AgDim1", "IdDim1", "2.1", SdmxStructureEnumType.Concept, "TIME_PERIOD");
            dimension.Representation = new RepresentationMutableCore
            {
                Representation =
                    new StructureReferenceImpl("AgDim1Cl", "IdDim1Cl", "2.1", SdmxStructureEnumType.CodeList)
            };
            dimension.TimeDimension = true;
            dsdMutable.AddDimension(dimension);
            var components = new List<string> {"TIME_PERIOD"};

            var results = nsiGetArtefactRest.GetCodeListCostraint(dfMutable.ImmutableInstance,
                dsdMutable.ImmutableInstance, "TIME_PERIOD");

            Assert.Equal(5, results.Count);
            Assert.Empty(results[ComponentType.EnumComponentType.FrequencyDimension]);
            Assert.Empty(results[ComponentType.EnumComponentType.MeasureDimension]);
            Assert.Empty(results[ComponentType.EnumComponentType.Attribute]);
            Assert.Single(results[ComponentType.EnumComponentType.TimeDimension]);
            Assert.Empty(results[ComponentType.EnumComponentType.Dimension]);
            Assert.Equal("availableconstraint/DFAg,DFId,1.1/ALL/ALL/TIME_PERIOD?references=codelist",
                results[ComponentType.EnumComponentType.TimeDimension].FirstOrDefault());
        }

        [Fact]
        public void GetStructure_GetCodeListCostraintFilter_Ok()
        {
            var nsiGetArtefactRest = new NsiGetStructureRest(NullLoggerFactory.Instance);

            var dfMutable = new DataflowMutableCore();
            dfMutable.Id = "DFId";
            dfMutable.AgencyId = "DFAg";
            dfMutable.Version = "1.1";
            dfMutable.AddName("IT", "testname");
            dfMutable.DataStructureRef = new StructureReferenceImpl("DSDAg", "DSDId", "1.1", SdmxStructureEnumType.Dsd);
            var dsdMutable = new DataStructureMutableCore();
            dsdMutable.Id = "DSDId";
            dsdMutable.AgencyId = "DSDAg";
            dsdMutable.Version = "1.1";
            dsdMutable.AddName("IT", "testname");
            dsdMutable.AddPrimaryMeasure(new StructureReferenceImpl("AG", "ID", "2.5", SdmxStructureEnumType.Concept,
                "PR"));
            IDimensionMutableObject dimension = new DimensionMutableCore();
            dimension.ConceptRef =
                new StructureReferenceImpl("AgDim1", "IdDim1", "2.1", SdmxStructureEnumType.Concept, "ID1");
            dimension.Representation = new RepresentationMutableCore
            {
                Representation =
                    new StructureReferenceImpl("AgDim1Cl", "IdDim1Cl", "2.1", SdmxStructureEnumType.CodeList)
            };
            dsdMutable.AddDimension(dimension);
            dimension = new DimensionMutableCore();
            dimension.ConceptRef =
                new StructureReferenceImpl("AgDim3", "IdDim3", "3.3", SdmxStructureEnumType.Concept, "ID3");
            dimension.Representation = new RepresentationMutableCore
            {
                Representation =
                    new StructureReferenceImpl("AgDim1Cl", "IdDim1Cl", "2.1", SdmxStructureEnumType.CodeList)
            };
            dsdMutable.AddDimension(dimension);
            dimension = new DimensionMutableCore();
            dimension.ConceptRef =
                new StructureReferenceImpl("AgDim2", "IdDim2", "3.1", SdmxStructureEnumType.Concept, "ID2");
            dimension.Representation = new RepresentationMutableCore
            {
                Representation =
                    new StructureReferenceImpl("AgDim2Cl", "IdDim2Cl", "2.2", SdmxStructureEnumType.CodeList)
            };
            dsdMutable.AddDimension(dimension);
            dimension = new DimensionMutableCore();
            dimension.ConceptRef = new StructureReferenceImpl("AgIDTimeDim", "IdIDTimeDim", "1.0",
                SdmxStructureEnumType.Concept, "IDTimeDim");
            dimension.Representation = new RepresentationMutableCore
            {
                Representation = new StructureReferenceImpl("AgIDTimeDimCl", "IdIDTimeDimCl", "1.2",
                    SdmxStructureEnumType.CodeList)
            };
            dimension.TimeDimension = true;
            dsdMutable.AddDimension(dimension);
            var filterCriteria = new List<FilterCriteria>
            {
                new FilterCriteria {Id = "ID1", FilterValues = new List<string> {"valueA"}},
                new FilterCriteria {Id = "ID2", FilterValues = new List<string> {"valueA2", "valueB2"}},
                new FilterCriteria {Id = "ID3", FilterValues = new List<string> {"valueA3", "valueB3", "valueC3"}},
                new FilterCriteria {Id = "TIME_PERIOD", Type = FilterType.TimeRange, From = new System.DateTime(2020,1,3)}
            };

            var results = nsiGetArtefactRest.GetCodeListCostraintFilter(dfMutable.ImmutableInstance,
                dsdMutable.ImmutableInstance, null, filterCriteria, null);

            Assert.Equal(
                "availableconstraint/DFAg,DFId,1.1/valueA.valueA3+valueB3+valueC3.valueA2+valueB2/?references=codelist&startPeriod=2020-01-03",
                results);


            filterCriteria = new List<FilterCriteria>
            {
                new FilterCriteria {Id = "ID1", FilterValues = new List<string> {"valueA"}},
                new FilterCriteria {Id = "ID2", FilterValues = new List<string> {"valueA2", "valueB2"}},
                new FilterCriteria {Id = "ID3", FilterValues = new List<string> {"valueA3", "valueB3", "valueC3"}}
            };

            results = nsiGetArtefactRest.GetCodeListCostraintFilter(dfMutable.ImmutableInstance,
                dsdMutable.ImmutableInstance, null, filterCriteria, null);

            Assert.Equal(
                "availableconstraint/DFAg,DFId,1.1/valueA.valueA3+valueB3+valueC3.valueA2+valueB2/?references=codelist",
                results);


            filterCriteria = new List<FilterCriteria>
            {
                new FilterCriteria {Id = "ID1", FilterValues = new List<string> {"valueA"}},
                new FilterCriteria {Id = "ID2", FilterValues = new List<string> {"valueA2", "valueB2"}},
                new FilterCriteria {Id = "ID3", FilterValues = new List<string> {"valueA3", "valueB3", "valueC3"}},
                new FilterCriteria {Id = "TIME_PERIOD", Type = FilterType.TimeRange, From = new System.DateTime(2020,1,3), To = new System.DateTime(2022,12,25)}
            };

            results = nsiGetArtefactRest.GetCodeListCostraintFilter(dfMutable.ImmutableInstance,
                dsdMutable.ImmutableInstance, null, filterCriteria, null);

            Assert.Equal(
                "availableconstraint/DFAg,DFId,1.1/valueA.valueA3+valueB3+valueC3.valueA2+valueB2/?references=codelist&startPeriod=2020-01-03&endPeriod=2022-12-25",
                results);


            filterCriteria = new List<FilterCriteria>
            {
                new FilterCriteria {Id = "ID1", FilterValues = new List<string> {"valueA"}},
                new FilterCriteria {Id = "ID2", FilterValues = new List<string> {"valueA2", "valueB2"}}
            };

            results = nsiGetArtefactRest.GetCodeListCostraintFilter(dfMutable.ImmutableInstance,
                dsdMutable.ImmutableInstance, null, filterCriteria, null);
            Assert.Equal("availableconstraint/DFAg,DFId,1.1/valueA..valueA2+valueB2/?references=codelist", results);
        }
    }
}