using BenchmarkDotNet.Attributes;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.Util.Objects.Container;

namespace DataBrowser.Benchmark.SdmxParser
{
    [MemoryDiagnoser]
    public class SdmxParserBenchmark
    {
/*
| Method |     Mean |   Error |  StdDev | Ratio |     Gen 0 |     Gen 1 |     Gen 2 | Allocated |
|------- |---------:|--------:|--------:|------:|----------:|----------:|----------:|----------:|
|  Parse | 203.6 ms | 3.24 ms | 3.03 ms |  1.00 | 8000.0000 | 3000.0000 | 1000.0000 |  52.72 MB |

| Method |     Mean |    Error |   StdDev | Ratio |     Gen 0 |     Gen 1 | Gen 2 | Allocated |
|------- |---------:|---------:|---------:|------:|----------:|----------:|------:|----------:|
|  Parse | 43.92 ms | 0.872 ms | 2.571 ms |  1.00 | 2000.0000 | 1000.0000 |     - |  13.79 MB |
*/


        [Benchmark(Baseline = true)]
        public void Parse()
        {
            var id = "iddd";
            var agency = "aggg";
            var version = "2.0";

            ISdmxObjects sdmxObjects = new SdmxObjectsImpl();
            sdmxObjects.AddCodelist(CreateCodelist(id, agency, version, 20));
            sdmxObjects.AddCodelist(CreateCodelist(id, agency + "2", version, 20));
            sdmxObjects.AddCodelist(CreateCodelist(id + "2", agency, version, 20));
            sdmxObjects.AddCodelist(CreateCodelist(id + "3", agency, version, 20));
            sdmxObjects.AddCodelist(CreateCodelist(id + "4", agency, version, 90));
            sdmxObjects.AddCodelist(CreateCodelist(id + "41", agency, version, 20));
            sdmxObjects.AddCodelist(CreateCodelist(id + "42", agency, version, 80));
            sdmxObjects.AddCodelist(CreateCodelist(id + "43", agency, version, 20));
            sdmxObjects.AddCodelist(CreateCodelist(id + "44", agency, version, 20));
            sdmxObjects.AddCodelist(CreateCodelist(id + "45", agency, version, 50));
            sdmxObjects.AddCodelist(CreateCodelist(id + "46", agency, version, 420));
            sdmxObjects.AddCodelist(CreateCodelist(id + "47", agency, version, 120));
            sdmxObjects.AddCodelist(CreateCodelist(id + "48", agency, version, 120));
            //new DataBrowser.Parser.Sdmx.SdmxParser().GetSdmxJsonFromSdmxObjects(sdmxObjects);
            //new SH.SdmxParser.SdmxParser().GetSdmxJsonFromSdmxObjectsWithBug(sdmxObjects);
        }

        public static ICodelistObject CreateCodelist(string id, string agency, string version, int items)
        {
            var codelistMutableObject = new CodelistMutableCore
            {
                Id = id,
                Version = version,
                AgencyId = agency
            };
            codelistMutableObject.AddName("it", "nameIT");
            codelistMutableObject.AddName("en", "nameEN");
            codelistMutableObject.AddName("de", "nameDE");
            for (var i = 0; i < items; i++)
            {
                var item = new CodeMutableCore();
                item.Id = $"name{i}";
                item.AddName("en", $"nameEN{i}");
                item.AddName("it", $"nameIT{i}");
                item.AddName("fr", $"nameFR{i}");
                var annotation = new AnnotationMutableCore();
                annotation.Id = "AnnotationId";
                annotation.Type = "AnnotationType";
                var itemText = new TextTypeWrapperMutableCore();
                itemText.Locale = "en";
                itemText.Value = "textEN";
                annotation.AddText(itemText);
                itemText = new TextTypeWrapperMutableCore();
                itemText.Locale = "de";
                itemText.Value = "textDE";
                annotation.AddText(itemText);
                itemText = new TextTypeWrapperMutableCore();
                itemText.Locale = "it";
                itemText.Value = "textIT";
                annotation.AddText(itemText);
                item.AddAnnotation(annotation);

                codelistMutableObject.AddItem(item);
            }

            return codelistMutableObject.ImmutableInstance;
        }
    }
}