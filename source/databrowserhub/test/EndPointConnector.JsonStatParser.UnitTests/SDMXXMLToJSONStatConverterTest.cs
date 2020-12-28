using EndPointConnector.Interfaces.JsonStat;
using EndPointConnector.JsonStatParser.Factories;
using Microsoft.Extensions.Logging.Abstractions;
using Org.Sdmxsource.Sdmx.Structureparser.Manager.Parsing;
using System.IO;
using System.Linq;
using System.Xml;
using EndPointConnector.JsonStatParser.Model.JsonStat;
using Xunit;

namespace EndPointConnector.JsonStatParser.UnitTests
{
    public class SdmxXmlToJsonStatConverterTest
    {

        [Fact]
        public void CreateJsonStat_From_SDMX_JUSTICE_GROUP_ATTR_OK()
        {
            var converteFactory = new FromSdmxXmlToJsonStatConverterFactory(NullLoggerFactory.Instance);
            var dataXml = new XmlDocument();
            dataXml.LoadXml(File.ReadAllText("SDMXXML/SDMX_JUSTICE_GROUP_ATTR_DATA.xml"));

            var xDocStructure = new XmlDocument();
            xDocStructure.Load("SDMXXML/SDMX_JUSTICE_GROUP_ATTR_DSD_CL.xml");
            Org.Sdmxsource.Sdmx.Api.Util.IReadableDataLocation rdl = new Org.Sdmxsource.Util.Io.XmlDocReadableDataLocation(xDocStructure);
            var spm = new StructureParsingManager();
            var workspace = spm.ParseStructures(rdl);
            //ISdmxObjects sdmxObjects = workspace.GetStructureObjects(true);
            var sdmxObjects = workspace.GetStructureObjects(false);

            var dataflow = sdmxObjects.Dataflows.First();
            var dataStructure = sdmxObjects.DataStructures.First();
            var codelists = sdmxObjects.Codelists;
            var conceptSchemes = sdmxObjects.ConceptSchemes;

            var config = DefaultJsonStatConverterConfig.GetNew();
            var converter = converteFactory.GetConverter(dataXml, dataflow, dataStructure, codelists, conceptSchemes, "fr", config);
            var jsonStat = converter.Convert();

            var jsonGoldenMaster = File.ReadAllText("SDMXXML/SDMX_JUSTICE_GROUP_ATTR_GM.json");
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonStat);
            var deserializedGoldenMaster = JsonStatDataset.Deserialize(jsonGoldenMaster);
            Assert.Equal(JsonStatDataset.Serialize(deserializedJsonStat), JsonStatDataset.Serialize(deserializedGoldenMaster));
        }

        [Fact]
        public void CreateJsonStat_From_SDMX_JUSTICE_NOT_DISPLAYED_OK()
        {
            var converteFactory = new FromSdmxXmlToJsonStatConverterFactory(NullLoggerFactory.Instance);
            var dataXml = new XmlDocument();
            dataXml.LoadXml(File.ReadAllText("SDMXXML/SDMX_JUSTICE_GROUP_ATTR_DATA.xml"));

            var xDocStructure = new XmlDocument();
            xDocStructure.Load("SDMXXML/SDMX_JUSTICE_GROUP_ATTR_DSD_CL.xml");
            Org.Sdmxsource.Sdmx.Api.Util.IReadableDataLocation rdl = new Org.Sdmxsource.Util.Io.XmlDocReadableDataLocation(xDocStructure);
            var spm = new StructureParsingManager();
            var workspace = spm.ParseStructures(rdl);
            //ISdmxObjects sdmxObjects = workspace.GetStructureObjects(true);
            var sdmxObjects = workspace.GetStructureObjects(false);

            var dataflow = sdmxObjects.Dataflows.First();
            var dataStructure = sdmxObjects.DataStructures.First();
            var codelists = sdmxObjects.Codelists;
            var conceptSchemes = sdmxObjects.ConceptSchemes;

            var config = DefaultJsonStatConverterConfig.GetNew();
            config.NotDisplayedAnnotationId = "NOT_DISPLAYED_NEW_NAME";
            var converter = converteFactory.GetConverter(dataXml, dataflow, dataStructure, codelists, conceptSchemes, "fr", config);
            var jsonStat = converter.Convert();

            var jsonGoldenMaster = File.ReadAllText("SDMXXML/SDMX_JUSTICE_CUSTOM_NOT_DISPLAYED_GM.json");
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonStat);
            var deserializedGoldenMaster = JsonStatDataset.Deserialize(jsonGoldenMaster);
            Assert.Equal(JsonStatDataset.Serialize(deserializedJsonStat), JsonStatDataset.Serialize(deserializedGoldenMaster));
        }

        [Fact]
        public void CreateJsonStat_From_NA_SU278324_OK()
        {
            var converteFactory = new FromSdmxXmlToJsonStatConverterFactory(NullLoggerFactory.Instance);
            var dataXml = new XmlDocument();
            dataXml.LoadXml(File.ReadAllText("SDMXXML/NA_SU278324_DATA.xml"));

            var xDocStructure = new XmlDocument();
            xDocStructure.Load("SDMXXML/NA_SU278324_DSD_CL.xml");
            Org.Sdmxsource.Sdmx.Api.Util.IReadableDataLocation rdl = new Org.Sdmxsource.Util.Io.XmlDocReadableDataLocation(xDocStructure);
            var spm = new StructureParsingManager();
            var workspace = spm.ParseStructures(rdl);
            //ISdmxObjects sdmxObjects = workspace.GetStructureObjects(true);
            var sdmxObjects = workspace.GetStructureObjects(false);
            var dataflow = sdmxObjects.Dataflows.First();
            var dataStructure = sdmxObjects.DataStructures.First();
            var codelists = sdmxObjects.Codelists;
            var conceptSchemes = sdmxObjects.ConceptSchemes;

            var config = DefaultJsonStatConverterConfig.GetNew();
            var converter = converteFactory.GetConverter(dataXml, dataflow, dataStructure, codelists, conceptSchemes, "fr", config);
            var jsonStat = converter.Convert();

            var jsonGoldenMaster = File.ReadAllText("SDMXXML/NA_SU278324_GM.json", System.Text.Encoding.UTF8);
            
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonStat);
            var deserializedGoldenMaster = JsonStatDataset.Deserialize(jsonGoldenMaster);
            Assert.Equal(JsonStatDataset.Serialize(deserializedJsonStat), JsonStatDataset.Serialize(deserializedGoldenMaster));
        }
    }
        
}
