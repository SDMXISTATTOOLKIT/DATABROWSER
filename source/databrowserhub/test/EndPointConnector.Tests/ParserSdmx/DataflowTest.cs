using System.Collections.Generic;
using EndPointConnector.Models;
using EndPointConnector.ParserSdmx;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.MetadataStructure;
using Org.Sdmxsource.Sdmx.Util.Objects.Reference;
using Xunit;

namespace EndPointConnector.Tests.ParserSdmx
{
    public class DataflowTest
    {
        [Fact]
        public void Convert_SdmxToModel_Ok()
        {
            //var lang = "IT";
            var sdmxDataflow = createSdmxDataflow();

            var modelDataflow = DataModelParser.ConvertArtefact(sdmxDataflow.ImmutableInstance, null);

            Assert.NotNull(modelDataflow);
            Assert.Equal("agIT+dataflowId+3.1", modelDataflow.Id);
            Assert.Equal("name ITA", modelDataflow.Names["IT"]);
            Assert.Equal("name fr", modelDataflow.Names["FR"]);
            Assert.Equal("dsdAg+dsdId+2.9", modelDataflow.DataStructureRef.Id);
        }

        [Fact]
        public void Convert_ModelToSdmx_Ok()
        {
            //var lang = "IT";
            var modelDataflow = createModelDataflow();

            var sdmxDataflow = DataModelParser.ConvertArtefact(modelDataflow);

            Assert.NotNull(sdmxDataflow);
            Assert.Equal($"{sdmxDataflow.AgencyId}+{sdmxDataflow.Id}+{sdmxDataflow.Version}", modelDataflow.Id);
            Assert.Equal("dataflowId", sdmxDataflow.Id);
            Assert.Equal("agIT", sdmxDataflow.AgencyId);
            Assert.Equal("3.1", sdmxDataflow.Version);
            Assert.Equal("name ITA", sdmxDataflow.Names[0].Value);
            Assert.Equal("name EN", sdmxDataflow.Names[1].Value);
            Assert.Equal("dsdId", sdmxDataflow.DataStructureRef.MaintainableId);
            Assert.Equal("dsdAg", sdmxDataflow.DataStructureRef.AgencyId);
            Assert.Equal("2.9", sdmxDataflow.DataStructureRef.Version);
            Assert.Equal(SdmxStructureEnumType.Dsd,
                sdmxDataflow.DataStructureRef.MaintainableStructureEnumType.EnumType);
        }

        private static IDataflowMutableObject createSdmxDataflow()
        {
            IDataflowMutableObject mutable = new DataflowMutableCore();
            mutable.Id = "dataflowId";
            mutable.AgencyId = "agIT";
            mutable.Version = "3.1";
            mutable.AddName("IT", "name ITA");
            mutable.AddName("FR", "name fr");

            mutable.DataStructureRef = new StructureReferenceImpl("dsdAg", "dsdId", "2.9", SdmxStructureEnumType.Dsd);

            return mutable;
        }

        private static Dataflow createModelDataflow()
        {
            var dataflow = new Dataflow();
            dataflow.Id = "agIT+dataflowId+3.1";
            dataflow.Names = new Dictionary<string, string> {{"IT", "name ITA"}, {"EN", "name EN"}};
            dataflow.DataStructureRef = new ArtefactRef
                {Id = "dsdAg+dsdId+2.9", RefType = ArtefactType.ArtefactEnumType.Dsd};

            return dataflow;
        }
    }
}