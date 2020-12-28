using BenchmarkDotNet.Attributes;

namespace DataBrowser.Benchmark.JsonStataConvert
{
    [MemoryDiagnoser]
    public class JsonStatConvert
    {
        //[Benchmark(Baseline = true)]
        //public void ParseXml()
        //{
        //    var dataXml = new XmlDocument();
        //    dataXml.LoadXml(File.ReadAllText("D:\\repos\\DataBrowser_GitLab\\test\\DataBrowser.Benchmark\\JsonStatConvert\\DataFile.xml"));

        //    var xDocStructure = new XmlDocument();
        //    xDocStructure.Load("D:\\repos\\DataBrowser_GitLab\\test\\DataBrowser.Benchmark\\JsonStatConvert\\DF_TR_AEREO+IT1+1.0.xml");
        //    Org.Sdmxsource.Sdmx.Api.Util.IReadableDataLocation rdl = new Org.Sdmxsource.Util.Io.XmlDocReadableDataLocation(xDocStructure);
        //    StructureParsingManager spm = new StructureParsingManager();
        //    IStructureWorkspace workspace = spm.ParseStructures(rdl);
        //    ISdmxObjects sdmxObjects = workspace.GetStructureObjects(false);
        //    var converter = new SDMXMLToJsonStatConverter(null, null);
        //    converter.CreateJsonStatFromXmlSdmxCompactData(dataXml, sdmxObjects.Dataflows.First(), sdmxObjects.DataStructures.First(), null);
        //}

        [Benchmark(Baseline = true)]
        public void ParseJson()
        {
            //var converter = new FromSDMXJsonToJsonStatConverter(NullLoggerFactory.Instance, connectorContext.Object);
            //converter.FromSDMXJson(File.ReadAllText(@"C:\Users\b.zizi\sistan\test\DataBrowser.Benchmark\JsonStatConvert\JsonSdmxData.IT1+ACCIDENT+1.0.json"));
        }
    }
}