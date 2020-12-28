using System;
using System.Collections.Generic;
using System.Linq;
using EndPointConnector.Models;
using EndPointConnector.ParserSdmx;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Codelist;
using Xunit;

namespace EndPointConnector.Tests.ParserSdmx
{
    public class CodelistTest
    {
        [Fact]
        public void Convert_SdmxToModel_Ok()
        {
            var lang = "IT";
            var sdmxCodelist = createSdmxCodelist();

            var modelCodelist = DataModelParser.ConvertArtefact(sdmxCodelist.ImmutableInstance, null);

            Assert.Equal($"{sdmxCodelist.AgencyId}+{sdmxCodelist.Id}+{sdmxCodelist.Version}", modelCodelist.Id);
            var sdmxName =
                sdmxCodelist.Names.FirstOrDefault(i =>
                    i.Locale.Equals(lang, StringComparison.InvariantCultureIgnoreCase));
            Assert.Equal(2, sdmxCodelist.Names.Count);
            if (sdmxName != null)
                Assert.Equal(sdmxName.Value, modelCodelist.Names["IT"]);
            else
                Assert.Equal(sdmxCodelist.Names.First().Value, modelCodelist.Names["IT"]);

            Assert.Equal(sdmxCodelist.Items.Count, modelCodelist.Items.Count);
            foreach (var sdmxItem in sdmxCodelist.Items)
            {
                var itemModelFind = modelCodelist.Items.FirstOrDefault(i => i.Id.Equals(sdmxItem.Id));
                Assert.NotNull(itemModelFind);
            }

            var itemModel = modelCodelist.Items.FirstOrDefault(i => i.Id.Equals("ID1"));
            Assert.NotNull(itemModel);
            Assert.Single(itemModel.Names);
            Assert.Equal("item name 1", itemModel.Names["IT"]);
            Assert.Null(itemModel.ParentId);
            itemModel = modelCodelist.Items.FirstOrDefault(i => i.Id.Equals("ID4"));
            Assert.NotNull(itemModel);
            Assert.Equal(2, itemModel.Names.Count);
            Assert.Equal("item name IT 4", itemModel.Names["IT"]);
            Assert.Null(itemModel.ParentId);
            itemModel = modelCodelist.Items.FirstOrDefault(i => i.Id.Equals("ID8"));
            Assert.NotNull(itemModel);
            Assert.Single(itemModel.Names);
            Assert.Equal("item name 8", itemModel.Names["IT"]);
            Assert.Equal("ID1", itemModel.ParentId);
            itemModel = modelCodelist.Items.FirstOrDefault(i => i.Id.Equals("ID9"));
            Assert.NotNull(itemModel);
            Assert.Single(itemModel.Names);
            Assert.Equal("item name fr 9", itemModel.Names["FR"]);
            Assert.Null(itemModel.ParentId);
            itemModel = modelCodelist.Items.FirstOrDefault(i => i.Id.Equals("ID10"));
            Assert.NotNull(itemModel);
            Assert.Equal("item name 10", itemModel.Names["IT"]);
            Assert.Null(itemModel.ParentId);
            itemModel = modelCodelist.Items.FirstOrDefault(i => i.Id.Equals("ID15"));
            Assert.NotNull(itemModel);
            Assert.Single(itemModel.Names);
            Assert.Equal("item name 15", itemModel.Names["IT"]);
            Assert.Equal("ID11", itemModel.ParentId);
            itemModel = modelCodelist.Items.FirstOrDefault(i => i.Id.Equals("ID19"));
            Assert.NotNull(itemModel);
            Assert.Equal("item name 19", itemModel.Names["IT"]);
            Assert.Equal("ID17", itemModel.ParentId);
        }

        [Fact]
        public void Convert_ModelToSdmx_Ok()
        {
            var lang = "IT";
            var modelCodelist = createModelCodelist();

            var sdmxCodelist = DataModelParser.ConvertArtefact(modelCodelist);

            Assert.Equal($"{sdmxCodelist.AgencyId}+{sdmxCodelist.Id}+{sdmxCodelist.Version}", modelCodelist.Id);
            var sdmxName =
                sdmxCodelist.Names.FirstOrDefault(i =>
                    i.Locale.Equals(lang, StringComparison.InvariantCultureIgnoreCase));
            Assert.Equal(2, sdmxCodelist.Names.Count);
            if (sdmxName != null)
                Assert.Equal(modelCodelist.Names["IT"], sdmxName.Value);
            else
                Assert.Equal(modelCodelist.Names["IT"], sdmxCodelist.Names.First().Value);

            Assert.Equal(sdmxCodelist.Items.Count, modelCodelist.Items.Count);
            foreach (var modelItem in modelCodelist.Items)
            {
                var itemModelFind = sdmxCodelist.Items.FirstOrDefault(i => i.Id.Equals(modelItem.Id));
                Assert.NotNull(itemModelFind);
            }

            var itemSdmx = sdmxCodelist.Items.FirstOrDefault(i => i.Id.Equals("ID1"));
            Assert.NotNull(itemSdmx);
            Assert.Equal(2, itemSdmx.Names.Count);
            Assert.Equal("item name 1", itemSdmx.Name);
            Assert.Null(itemSdmx.ParentCode);
            itemSdmx = sdmxCodelist.Items.FirstOrDefault(i => i.Id.Equals("ID4"));
            Assert.NotNull(itemSdmx);
            Assert.Equal(2, itemSdmx.Names.Count);
            Assert.Equal("item name IT 4", itemSdmx.Name);
            Assert.Null(itemSdmx.ParentCode);
            itemSdmx = sdmxCodelist.Items.FirstOrDefault(i => i.Id.Equals("ID8"));
            Assert.NotNull(itemSdmx);
            Assert.Equal(2, itemSdmx.Names.Count);
            Assert.Equal("item name 8", itemSdmx.Name);
            Assert.Equal("ID1", itemSdmx.ParentCode);
            itemSdmx = sdmxCodelist.Items.FirstOrDefault(i => i.Id.Equals("ID9"));
            Assert.NotNull(itemSdmx);
            Assert.Equal(2, itemSdmx.Names.Count);
            Assert.Equal("item name fr 9", itemSdmx.Name);
            Assert.Null(itemSdmx.ParentCode);
            itemSdmx = sdmxCodelist.Items.FirstOrDefault(i => i.Id.Equals("ID10"));
            Assert.NotNull(itemSdmx);
            Assert.Equal("item name 10", itemSdmx.Name);
            Assert.Null(itemSdmx.ParentCode);
            itemSdmx = sdmxCodelist.Items.FirstOrDefault(i => i.Id.Equals("ID15"));
            Assert.NotNull(itemSdmx);
            Assert.Equal(2, itemSdmx.Names.Count);
            Assert.Equal("item name 15", itemSdmx.Name);
            Assert.Equal("ID11", itemSdmx.ParentCode);
            itemSdmx = sdmxCodelist.Items.FirstOrDefault(i => i.Id.Equals("ID19"));
            Assert.NotNull(itemSdmx);
            Assert.Equal("item name 19", itemSdmx.Name);
            Assert.Equal("ID17", itemSdmx.ParentCode);
        }

        private static ICodelistMutableObject createSdmxCodelist()
        {
            ICodelistMutableObject mutable = new CodelistMutableCore();
            mutable.Id = "codelistId";
            mutable.AgencyId = "agIT";
            mutable.Version = "3.1";
            mutable.AddName("IT", "name ITA");
            mutable.AddName("FR", "name fr");

            mutable.AddItem(createSdmxItem("ID1", new Dictionary<string, string> {{"IT", "item name 1"}}));
            mutable.AddItem(createSdmxItem("ID2",
                new Dictionary<string, string> {{"IT", "item name it 2"}, {"EN", "item name en 2"}}));
            mutable.AddItem(createSdmxItem("ID3", new Dictionary<string, string> {{"IT", "item name 3"}}));
            mutable.AddItem(createSdmxItem("ID4",
                new Dictionary<string, string> {{"DE", "item name 4"}, {"IT", "item name IT 4"}}));
            mutable.AddItem(createSdmxItem("ID5", new Dictionary<string, string> {{"IT", "item name 5"}}));
            mutable.AddItem(createSdmxItem("ID6", new Dictionary<string, string> {{"IT", "item name 6"}}, "ID1"));
            mutable.AddItem(createSdmxItem("ID7", new Dictionary<string, string> {{"EN", "item name 7"}}, "ID1"));
            mutable.AddItem(createSdmxItem("ID8", new Dictionary<string, string> {{"IT", "item name 8"}}, "ID1"));
            mutable.AddItem(createSdmxItem("ID9", new Dictionary<string, string> {{"FR", "item name fr 9"}}));
            mutable.AddItem(createSdmxItem("ID10",
                new Dictionary<string, string>
                    {{"IT", "item name 10"}, {"EN", "item en name 10"}, {"ES", "item name es 10"}}));
            mutable.AddItem(createSdmxItem("ID11", new Dictionary<string, string> {{"IT", "item name 11"}}, "ID8"));
            mutable.AddItem(createSdmxItem("ID12", new Dictionary<string, string> {{"IT", "item name 12"}}, "ID11"));
            mutable.AddItem(createSdmxItem("ID13", new Dictionary<string, string> {{"IT", "item name 13"}}, "ID11"));
            mutable.AddItem(createSdmxItem("ID14", new Dictionary<string, string> {{"IT", "item name 14"}}, "ID5"));
            mutable.AddItem(createSdmxItem("ID15", new Dictionary<string, string> {{"IT", "item name 15"}}, "ID11"));
            mutable.AddItem(createSdmxItem("ID16", new Dictionary<string, string> {{"IT", "item name 16"}}));
            mutable.AddItem(createSdmxItem("ID17", new Dictionary<string, string> {{"EN", "item name en 17"}}, "ID11"));
            mutable.AddItem(createSdmxItem("ID18", new Dictionary<string, string> {{"IT", "item name 18"}}, "ID17"));
            mutable.AddItem(createSdmxItem("ID19", new Dictionary<string, string> {{"IT", "item name 19"}}, "ID17"));

            return mutable;
        }

        private static CodeMutableCore createSdmxItem(string id, Dictionary<string, string> langs,
            string parentId = null)
        {
            var mutableCode = new CodeMutableCore {Id = id};
            if (!string.IsNullOrWhiteSpace(parentId)) mutableCode.ParentCode = parentId;
            foreach (var lang in langs) mutableCode.AddName(lang.Key, lang.Value);

            return mutableCode;
        }

        private static Codelist createModelCodelist()
        {
            var codelist = new Codelist();
            codelist.Id = "agIT+codelistId+3.1";
            codelist.Names = new Dictionary<string, string> {{"IT", "name ITA"}, {"EN", "name ENG"}};

            codelist.Items = new List<Code>();
            codelist.Items.Add(createModelItem("ID1", "item name 1"));
            codelist.Items.Add(createModelItem("ID2", "item name it 2"));
            codelist.Items.Add(createModelItem("ID3", "item name 3"));
            codelist.Items.Add(createModelItem("ID4", "item name IT 4"));
            codelist.Items.Add(createModelItem("ID5", "item name 5"));
            codelist.Items.Add(createModelItem("ID6", "item name 6", "ID1"));
            codelist.Items.Add(createModelItem("ID7", "item name 7", "ID1"));
            codelist.Items.Add(createModelItem("ID8", "item name 8", "ID1"));
            codelist.Items.Add(createModelItem("ID9", "item name fr 9"));
            codelist.Items.Add(createModelItem("ID10", "item name 10"));
            codelist.Items.Add(createModelItem("ID11", "item name 11", "ID8"));
            codelist.Items.Add(createModelItem("ID12", "item name 12", "ID11"));
            codelist.Items.Add(createModelItem("ID13", "item name 13", "ID11"));
            codelist.Items.Add(createModelItem("ID14", "item name 14", "ID5"));
            codelist.Items.Add(createModelItem("ID15", "item name 15", "ID11"));
            codelist.Items.Add(createModelItem("ID16", "item name 16"));
            codelist.Items.Add(createModelItem("ID17", "item name en 17", "ID11"));
            codelist.Items.Add(createModelItem("ID18", "item name 18", "ID17"));
            codelist.Items.Add(createModelItem("ID19", "item name 19", "ID17"));

            return codelist;
        }

        private static Code createModelItem(string id, string name, string parentId = null, string lang = "IT")
        {
            var code = new Code {Id = id};
            if (!string.IsNullOrWhiteSpace(parentId)) code.ParentId = parentId;
            code.Names = new Dictionary<string, string> {{lang, name}, {"EN", "TEST Lang EN"}};

            return code;
        }
    }
}