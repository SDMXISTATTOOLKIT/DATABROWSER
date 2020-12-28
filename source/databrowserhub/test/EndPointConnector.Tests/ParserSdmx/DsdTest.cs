using System.Collections.Generic;
using System.Linq;
using EndPointConnector.Models;
using EndPointConnector.ParserSdmx;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using Xunit;

namespace EndPointConnector.Tests.ParserSdmx
{
    public class DsdTest
    {
        [Fact]
        public void Convert_SdmxToModel_Ok()
        {
            //var lang = "IT";
            var sdmxDsd = createSdmxDsd();

            var modelDsd = DataModelParser.ConvertArtefact(sdmxDsd.ImmutableInstance, null);

            Assert.NotNull(modelDsd);
            Assert.Equal("agIT+dataflowId+3.1", modelDsd.Id);
            Assert.Equal("name ITA", modelDsd.Names["IT"]);
            Assert.Equal("name de", modelDsd.Names["DE"]);
            Assert.NotNull(modelDsd.PrimaryMeasure);
            Assert.Equal("OBS_VALUE", modelDsd.PrimaryMeasure.Id);
            Assert.Equal(
                $"{sdmxDsd.PrimaryMeasure.ConceptRef.AgencyId}+{sdmxDsd.PrimaryMeasure.ConceptRef.MaintainableId}+{sdmxDsd.PrimaryMeasure.ConceptRef.Version}",
                modelDsd.PrimaryMeasure.ConceptRef.Id);
            Assert.Equal(ArtefactType.ArtefactEnumType.Concept, modelDsd.PrimaryMeasure.ConceptRef.RefType);

            Assert.Equal(sdmxDsd.Dimensions.Count, modelDsd.Dimensions.Count);
            foreach (var sdmxDim in sdmxDsd.Dimensions)
            {
                var modelDim = modelDsd.Dimensions.First(i => i.Id.Equals(sdmxDim.Id));
                Assert.NotNull(modelDim);
                Assert.Equal(
                    $"{sdmxDim.ConceptRef.AgencyId}+{sdmxDim.ConceptRef.MaintainableId}+{sdmxDim.ConceptRef.Version}",
                    modelDim.ConceptRef.Id);
                Assert.Equal(ArtefactType.ArtefactEnumType.Concept, modelDim.ConceptRef.RefType);
                Assert.Equal(
                    $"{sdmxDim.Representation.Representation.AgencyId}+{sdmxDim.Representation.Representation.MaintainableId}+{sdmxDim.Representation.Representation.Version}",
                    modelDim.Representation.Id);
                Assert.Equal(ArtefactType.ArtefactEnumType.CodeList, modelDim.Representation.RefType);
            }
        }

        [Fact]
        public void Convert_ModelToSdmx_Ok()
        {
            var modelDsd = createModelDsd();

            var sdmxDsd = DataModelParser.ConvertArtefact(modelDsd);

            Assert.NotNull(sdmxDsd);
            Assert.Equal(modelDsd.Id, $"{sdmxDsd.AgencyId}+{sdmxDsd.Id}+{sdmxDsd.Version}");
            Assert.Equal("dsdId", sdmxDsd.Id);
            Assert.Equal("agIT", sdmxDsd.AgencyId);
            Assert.Equal("3.2", sdmxDsd.Version);
            Assert.Equal("name ITA", sdmxDsd.Names[0].Value);
            Assert.Equal("name es", sdmxDsd.Names[1].Value);


            Assert.Equal(modelDsd.Dimensions.Count, sdmxDsd.GetDimensions().Count);
            foreach (var modelDim in modelDsd.Dimensions)
            {
                var sdmxDim = sdmxDsd.GetDimension(modelDim.Id);
                Assert.NotNull(sdmxDim);
                Assert.Equal(modelDim.ConceptRef.Id,
                    $"{sdmxDim.ConceptRef.AgencyId}+{sdmxDim.ConceptRef.MaintainableId}+{sdmxDim.ConceptRef.Version}");
                Assert.Equal(SdmxStructureEnumType.ConceptScheme,
                    sdmxDim.ConceptRef.MaintainableStructureEnumType.EnumType);
                Assert.Equal(modelDim.Representation.Id,
                    $"{sdmxDim.Representation.Representation.AgencyId}+{sdmxDim.Representation.Representation.MaintainableId}+{sdmxDim.Representation.Representation.Version}");
                Assert.Equal(SdmxStructureEnumType.CodeList,
                    sdmxDim.Representation.Representation.MaintainableStructureEnumType.EnumType);
            }
        }

        private static IDataStructureMutableObject createSdmxDsd()
        {
            IDataStructureMutableObject mutable = new DataStructureMutableCore();
            mutable.Id = "dataflowId";
            mutable.AgencyId = "agIT";
            mutable.Version = "3.1";
            mutable.AddName("DE", "name de");
            mutable.AddName("IT", "name ITA");

            mutable.AddPrimaryMeasure(new StructureReferenceImpl("agId", "idtest", "1.0", SdmxStructureEnumType.Concept,
                "PriMeaId"));

            IDimensionMutableObject dimension = new DimensionMutableCore();
            dimension.Id = "IDMes1";
            dimension.ConceptRef = new StructureReferenceImpl("agConcp1", "idCon1", "1.1",
                SdmxStructureEnumType.Concept, "IDMes1Concept");
            dimension.Representation = new RepresentationMutableCore
                {Representation = new StructureReferenceImpl("clAg1", "idCl1", "10.1", SdmxStructureEnumType.CodeList)};
            mutable.AddDimension(dimension);
            dimension = new DimensionMutableCore();
            dimension.Id = "IDMes2";
            dimension.ConceptRef = new StructureReferenceImpl("agConcp2", "idCon2", "1.2",
                SdmxStructureEnumType.Concept, "IDMes2Concept");
            dimension.Representation = new RepresentationMutableCore
                {Representation = new StructureReferenceImpl("clAg2", "idCl2", "10.2", SdmxStructureEnumType.CodeList)};
            mutable.AddDimension(dimension);
            dimension = new DimensionMutableCore();
            dimension.Id = "TIME_PERIOD";
            dimension.ConceptRef = new StructureReferenceImpl("agConcp3", "idCon3", "1.3",
                SdmxStructureEnumType.Concept, "TIME_PERIODConcept");
            dimension.Representation = new RepresentationMutableCore
                {Representation = new StructureReferenceImpl("clAg3", "idCl3", "10.3", SdmxStructureEnumType.CodeList)};
            dimension.TimeDimension = true;
            mutable.AddDimension(dimension);
            dimension = new DimensionMutableCore();
            dimension.Id = "IDMes4";
            dimension.ConceptRef = new StructureReferenceImpl("agConcp4", "idCon4", "1.4",
                SdmxStructureEnumType.Concept, "IDMes4Concept");
            dimension.Representation = new RepresentationMutableCore
                {Representation = new StructureReferenceImpl("clAg1", "idCl1", "10.1", SdmxStructureEnumType.CodeList)};
            mutable.AddDimension(dimension);

            return mutable;
        }

        private static Dsd createModelDsd()
        {
            var dsd = new Dsd();
            dsd.Id = "agIT+dsdId+3.2";
            dsd.Names = new Dictionary<string, string> {{"IT", "name ITA"}, {"ES", "name es"}};
            dsd.PrimaryMeasure = new PrimaryMeasure
            {
                Id = "PriMeaId",
                ConceptRef = new ArtefactRef {Id = "agId+idtest+1.0", RefType = ArtefactType.ArtefactEnumType.Concept}
            };
            dsd.Dimensions = new List<Dimension>();
            dsd.Dimensions.Add(new Dimension
            {
                Id = "IDMes1",
                ConceptRef = new ArtefactRef
                    {Id = "agConcp1+idCon1+1.1", RefType = ArtefactType.ArtefactEnumType.ConceptScheme},
                Representation = new ArtefactRef
                    {Id = "clAg1+idCl1+10.1", RefType = ArtefactType.ArtefactEnumType.CodeList}
            });
            dsd.Dimensions.Add(new Dimension
            {
                Id = "IDMes2",
                ConceptRef = new ArtefactRef
                    {Id = "agConcp2+idCon2+1.2", RefType = ArtefactType.ArtefactEnumType.ConceptScheme},
                Representation = new ArtefactRef
                    {Id = "clAg2+idCl2+10.2", RefType = ArtefactType.ArtefactEnumType.CodeList}
            });
            dsd.Dimensions.Add(new Dimension
            {
                Id = "TIME_PERIOD",
                ConceptRef = new ArtefactRef
                    {Id = "agConcp3+idCon3+1.3", RefType = ArtefactType.ArtefactEnumType.ConceptScheme},
                Representation = new ArtefactRef
                    {Id = "clAg3+idCl3+10.3", RefType = ArtefactType.ArtefactEnumType.CodeList}
            });
            dsd.Dimensions.Add(new Dimension
            {
                Id = "IDMes4",
                ConceptRef = new ArtefactRef
                    {Id = "agConcp4+idCon4+1.4", RefType = ArtefactType.ArtefactEnumType.ConceptScheme},
                Representation = new ArtefactRef
                    {Id = "clAg1+idCl1+10.1", RefType = ArtefactType.ArtefactEnumType.CodeList}
            });

            return dsd;
        }
    }
}