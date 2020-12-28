using System.Collections.Generic;
using EndPointConnector.Models;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Base;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Codelist;

namespace DataBrowser.UnitTests.HelperTest
{
    public class Utility
    {
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

        public static List<Criteria> GenerateCriteria()
        {
            var criterias = new List<Criteria>();
            criterias.Add(new Criteria
            {
                Id = "Id1",
                Titles = new Dictionary<string, string> {{"IT", "Titolo IT"}, {"EN", "title EN"}},
                Values = new List<Code>
                {
                    new Code
                    {
                        Id = "A",
                        Names = new Dictionary<string, string> {{"IT", "A IT"}, {"EN", "A EN"}}
                    },
                    new Code
                    {
                        Id = "B",
                        Names = new Dictionary<string, string> {{"EN", "B EN"}}
                    },
                    new Code
                    {
                        Id = "C",
                        ParentId = "B",
                        Names = new Dictionary<string, string> {{"IT", "C IT"}, {"EN", "C EN"}}
                    },
                    new Code
                    {
                        Id = "D",
                        ParentId = "C",
                        Names = new Dictionary<string, string> {{"EN", "D EN"}}
                    },
                    new Code
                    {
                        Id = "E",
                        ParentId = "A",
                        Names = new Dictionary<string, string> {{"IT", "E IT"}, {"DE", "E DE"}}
                    },
                    new Code
                    {
                        Id = "F",
                        Names = new Dictionary<string, string> {{"EN", "F EN"}, {"IT", "F IT"}}
                    },
                    new Code
                    {
                        Id = "G",
                        ParentId = "C",
                        Names = new Dictionary<string, string> {{"EN", "G EN"}, {"IT", "G IT"}}
                    }
                }
            });
            criterias.Add(new Criteria
            {
                Id = "IdTwo",
                Titles = new Dictionary<string, string> {{"FR", "title IdTwo FR"}},
                Values = new List<Code>
                {
                    new Code
                    {
                        Id = "A2",
                        Names = new Dictionary<string, string> {{"FR", "A2 FR"}, {"EN", "A2 EN"}}
                    },
                    new Code
                    {
                        Id = "B2",
                        Names = new Dictionary<string, string> {{"EN", "B2 EN"}}
                    },
                    new Code
                    {
                        Id = "C2",
                        ParentId = "B2",
                        Names = new Dictionary<string, string> {{"EN", "C2 EN"}, {"IT", "C2 IT"}}
                    },
                    new Code
                    {
                        Id = "D2",
                        ParentId = "C2",
                        Names = new Dictionary<string, string> {{"DE", "D2 DE"}}
                    },
                    new Code
                    {
                        Id = "E2",
                        ParentId = "A2",
                        Names = new Dictionary<string, string> {{"ES", "E ES"}, {"DE", "E2 DE"}}
                    },
                    new Code
                    {
                        Id = "F2",
                        Names = new Dictionary<string, string> {{"EN", "F2 EN"}, {"IT", "F2 IT"}}
                    },
                    new Code
                    {
                        Id = "G2",
                        ParentId = "C2",
                        Names = new Dictionary<string, string> {{"EN", "G2 EN"}, {"IT", "G2 IT"}}
                    }
                }
            });
            criterias.Add(new Criteria
            {
                Id = "IdOther",
                Titles = new Dictionary<string, string> {{"EN", "title IdOther EN"}},
                Values = new List<Code>
                {
                    new Code
                    {
                        Id = "A3",
                        Names = new Dictionary<string, string> {{"FR", "A3 FR"}, {"EN", "A3 EN"}}
                    },
                    new Code
                    {
                        Id = "B3",
                        Names = new Dictionary<string, string> {{"EN", "B3 EN"}}
                    },
                    new Code
                    {
                        Id = "C3",
                        ParentId = "B3",
                        Names = new Dictionary<string, string> {{"EN", "C3 EN"}, {"IT", "C3 IT"}}
                    },
                    new Code
                    {
                        Id = "D3",
                        ParentId = "C3",
                        Names = new Dictionary<string, string> {{"DE", "D3 DE"}}
                    },
                    new Code
                    {
                        Id = "E3",
                        ParentId = "A3",
                        Names = new Dictionary<string, string> {{"ES", "E ES"}, {"DE", "E3 DE"}}
                    },
                    new Code
                    {
                        Id = "F3",
                        Names = new Dictionary<string, string> {{"EN", "F3 EN"}, {"IT", "F3 IT"}}
                    },
                    new Code
                    {
                        Id = "G3",
                        ParentId = "C3",
                        Names = new Dictionary<string, string> {{"EN", "G3 EN"}, {"IT", "G3 IT"}}
                    }
                }
            });

            return criterias;
        }
    }
}