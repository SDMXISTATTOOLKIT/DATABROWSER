using EndPointConnector.Interfaces.JsonStat;
using EndPointConnector.JsonStatParser.Factories;
using EndPointConnector.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using EndPointConnector.JsonStatParser.Model.JsonStat;
using EndPointConnector.JsonStatParser.Model.JsonStat.ExtensionMethods;
using Xunit;

namespace EndPointConnector.JsonStatParser.UnitTests
{
    public class SdmxJsonToJsonStatConverterTest
    {


        [Fact]
        public void CreateJsonStat_FromSortedJsonSdmx_OK()
        {
            var converterFactory = new FromSdmxJsonToJsonStatConverterFactory(NullLoggerFactory.Instance);
            var jsonSdmx = File.ReadAllText("SDMXJson/SortedSDMXJsonData.json");
            var config = DefaultJsonStatConverterConfig.GetNew();
            var converter = converterFactory.GetConverter(jsonSdmx, "en", config, null);
            var jsonStat = converter.Convert();
            var jsonGoldenMaster = File.ReadAllText("SDMXJson/SortedSDMXJsonData_converted_to_JSONStat.json");

            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonStat);
            var deserializedGoldenMaster = JsonStatDataset.Deserialize(jsonGoldenMaster);
            Assert.Equal(JsonStatDataset.Serialize(deserializedJsonStat), JsonStatDataset.Serialize(deserializedGoldenMaster));
        }
        

        [Fact]
        public void CreateJsonStat_FromCustomSortedJsonSdmx_OK()
        {
            var converterFactory = new FromSdmxJsonToJsonStatConverterFactory(NullLoggerFactory.Instance);
            var jsonSdmx = File.ReadAllText("SDMXJson/CustomSortedSDMXJsonData.json");
            var conversionConfig = DefaultJsonStatConverterConfig.GetNew();
            conversionConfig.OrderAnnotationId = "CUSTOM_ORDER_ANNOTATION";
            conversionConfig.TerritorialDimensionIds.Add("ITTER107");
            var converter = converterFactory.GetConverter(jsonSdmx, "en", conversionConfig, null);            
            var jsonStat = converter.Convert();
            var jsonGoldenMaster = File.ReadAllText("SDMXJson/CustomSortedSDMXJsonData_converted_to_JSONStat.json");

            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonStat);
            var deserializedGoldenMaster = JsonStatDataset.Deserialize(jsonGoldenMaster);
            Assert.Equal(JsonStatDataset.Serialize(deserializedJsonStat), JsonStatDataset.Serialize(deserializedGoldenMaster));
        }

        [Fact]
        public void CreateJsonStat_IsValidJSONStat_OK()
        {
            var converterFactory = new FromSdmxJsonToJsonStatConverterFactory(NullLoggerFactory.Instance);
            var jsonStatSchemaText = File.ReadAllText("SDMXJson/JsonStatSchema.json");
            var jsonSdmx = File.ReadAllText("SDMXJson/SDMXJsonData.json");
            var converter = converterFactory.GetConverter(jsonSdmx, "en", DefaultJsonStatConverterConfig.GetNew(), null);
            var schema = JSchema.Parse(jsonStatSchemaText);            
            var serializedJsonStat = converter.Convert();

            var deserializedJsonStat = JObject.Parse(serializedJsonStat);
            var isValidJsonStat = deserializedJsonStat.IsValid(schema, out IList<string> _);

            Assert.True(isValidJsonStat);
        }

        [Fact]
        public void Filter_OK()
        {
            var jsonstat = File.ReadAllText("SDMXJson/FilterData.json");
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonstat);
            var reSerializedJsonStat = JsonStatDataset.Serialize(deserializedJsonStat);

            var o1 = JObject.Parse(jsonstat);
            var o2 = JObject.Parse(reSerializedJsonStat);
            Assert.True(JToken.DeepEquals(o1, o2));

            var dataCriterias = new List<FilterCriteria>
            {
                new FilterCriteria {Id = "REF_AREA", FilterValues = new List<string> {"IT21"}}
            };


            deserializedJsonStat.Filter(dataCriterias);
            var filteredAndSerializedJsonStat = JsonStatDataset.Serialize(deserializedJsonStat);

            var jsonGoldenMaster = File.ReadAllText("SDMXJson/FilterDataGoldenMaster.json");

            var o3 = JObject.Parse(filteredAndSerializedJsonStat);
            var o4 = JObject.Parse(jsonGoldenMaster);
            Assert.True(JToken.DeepEquals(o3, o4));
        }


        [Fact]
        public void Filter_with_not_displayed_OK()
        {
            var jsonstat = File.ReadAllText("SDMXJson/FilterData.json");
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonstat);
            var reSerializedJsonStat = JsonStatDataset.Serialize(deserializedJsonStat);

            var o1 = JObject.Parse(jsonstat);
            var o2 = JObject.Parse(reSerializedJsonStat);
            Assert.True(JToken.DeepEquals(o1, o2));

            var dataCriterias = new List<FilterCriteria>
            {
                new FilterCriteria {Id = "REF_AREA", FilterValues = new List<string> {"IT21"}}
            };

            deserializedJsonStat.Filter(dataCriterias, new List<Criteria> { new Criteria { Id = "REF_AREA" } });
            var filteredAndSerializedJsonStat = JsonStatDataset.Serialize(deserializedJsonStat);

            var jsonGoldenMaster = File.ReadAllText("SDMXJson/FilterWithNotDisplayedGoldenMaster.json");

            var o3 = JObject.Parse(filteredAndSerializedJsonStat);
            var o4 = JObject.Parse(jsonGoldenMaster);
            Assert.True(JToken.DeepEquals(o3, o4));
        }

        [Fact]
        public void FilterWithInvalidCriteria_OK()
        {
            var jsonstat = File.ReadAllText("SDMXJson/FilterData.json");
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonstat);
            var reSerializedJsonStat = JsonStatDataset.Serialize(deserializedJsonStat);

            var o1 = JObject.Parse(jsonstat);
            var o2 = JObject.Parse(reSerializedJsonStat);
            Assert.True(JToken.DeepEquals(o1, o2));


            var dataCriterias = new List<FilterCriteria>
            {
                new FilterCriteria
                {
                    Type = FilterType.TimeRange,
                    From = new DateTime(2000, 1, 1),
                    To = new DateTime(2001, 12, 31),
                    Id = "TIME_PERIOD",
                    FilterValues = new List<string> {"2000", "2001"}
                }
            };

            // there is no data between years 2000 and 2001

            deserializedJsonStat.Filter(dataCriterias);
            var filteredAndSerializedJsonStat = JsonStatDataset.Serialize(deserializedJsonStat);

            var jsonGoldenMaster = File.ReadAllText("SDMXJson/FilterWithInvalidCriteriaGoldenMaster.json");

            var o3 = JObject.Parse(filteredAndSerializedJsonStat);
            var o4 = JObject.Parse(jsonGoldenMaster);
            Assert.True(JToken.DeepEquals(o3, o4));
        }

        [Fact]
        public void CreateJsonStatWithNotDisplayed_FromJsonSdmx_OK()
        {
            var converterFactory = new FromSdmxJsonToJsonStatConverterFactory(NullLoggerFactory.Instance);
            var jsonSdmx = File.ReadAllText("SDMXJson/NotDisplayedData.json");
            var converter = converterFactory.GetConverter(jsonSdmx, "en", DefaultJsonStatConverterConfig.GetNew(), null);
            var jsonStat = converter.Convert();
            var jsonGoldenMaster = File.ReadAllText("SDMXJson/NotDisplayedGoldenMaster.json");

            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonStat);
            var deserializedGoldenMaster = JsonStatDataset.Deserialize(jsonGoldenMaster);
            Assert.Equal(JsonStatDataset.Serialize(deserializedJsonStat), JsonStatDataset.Serialize(deserializedGoldenMaster));
        }


        [Fact]
        public void CreateJsonStatWithCustomNotDisplayed_FromJsonSdmx_OK()
        {
            var converterFactory = new FromSdmxJsonToJsonStatConverterFactory(NullLoggerFactory.Instance);
            var jsonSdmx = File.ReadAllText("SDMXJson/CustomNotDisplayedData.json");
            var conversionConfig = DefaultJsonStatConverterConfig.GetNew();
            conversionConfig.NotDisplayedAnnotationId = "not_displayed_new_name";
            var converter = converterFactory.GetConverter(jsonSdmx, "en",conversionConfig, null);
            var jsonStat = converter.Convert();
            var jsonGoldenMaster = File.ReadAllText("SDMXJson/CustomNotDisplayedGoldenMaster.json");

            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonStat);
            var deserializedGoldenMaster = JsonStatDataset.Deserialize(jsonGoldenMaster);
            Assert.Equal(JsonStatDataset.Serialize(deserializedJsonStat), JsonStatDataset.Serialize(deserializedGoldenMaster));
        }

        [Fact]
        public void CheckAnnotatedGeoDimension_FromJsonSdmx_OK()
        {
            var converterFactory = new FromSdmxJsonToJsonStatConverterFactory(NullLoggerFactory.Instance);
            var jsonSdmx = File.ReadAllText("SDMXJson/AnnotatedGeoDimensionRole.json");
            var conversionConfig = DefaultJsonStatConverterConfig.GetNew();
            conversionConfig.GeoAnnotationId = "LAYOUT_TERRITORIAL_DIMENSION_IDS";
            var converter = converterFactory.GetConverter(jsonSdmx, "en", conversionConfig, null);
            var jsonStat = converter.Convert();
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonStat);

            Assert.Equal(deserializedJsonStat.Role.Geo[0],"ITTER107");
        }

        [Fact]
        public void CheckNodeConfiguredGeoDimension_FromJsonSdmx_OK()
        {
            var converterFactory = new FromSdmxJsonToJsonStatConverterFactory(NullLoggerFactory.Instance);
            var jsonSdmx = File.ReadAllText("SDMXJson/AnnotatedGeoDimensionRole.json");
            var conversionConfig = DefaultJsonStatConverterConfig.GetNew();
            conversionConfig.TerritorialDimensionIds.Add("ITTER107");
            var converter = converterFactory.GetConverter(jsonSdmx, "en", conversionConfig, null);
            var jsonStat = converter.Convert();
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonStat);

            Assert.Equal(deserializedJsonStat.Role.Geo[0], "ITTER107");
        }

        [Fact]
        public void CheckEmptyGeoDimension_FromJsonSdmx_OK()
        {
            var converterFactory = new FromSdmxJsonToJsonStatConverterFactory(NullLoggerFactory.Instance);
            var jsonSdmx = File.ReadAllText("SDMXJson/AnnotatedGeoDimensionRole.json");
            var conversionConfig = DefaultJsonStatConverterConfig.GetNew();
            var converter = converterFactory.GetConverter(jsonSdmx, "en", conversionConfig, null);
            var jsonStat = converter.Convert();
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonStat);

            Assert.True(deserializedJsonStat.Role.Geo.Count == 0);
        }

        [Fact]
        public void CreateJsonStatWithAlphanumericObservation_FromJsonSdmx_OK()
        {
            var jsonSdmx = File.ReadAllText("SDMXJson/AlphanumericObservationData.json");
            var config = DefaultJsonStatConverterConfig.GetNew();
            var converterFactory = new FromSdmxJsonToJsonStatConverterFactory(NullLoggerFactory.Instance);
            var converter = converterFactory.GetConverter(jsonSdmx, "en", config, null);
            var jsonStat = converter.Convert();
            var jsonGoldenMaster = File.ReadAllText("SDMXJson/AlphanumericObservationDataGoldenMaster.json");

            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonStat);
            var deserializedGoldenMaster = JsonStatDataset.Deserialize(jsonGoldenMaster);
            Assert.Equal(JsonStatDataset.Serialize(deserializedJsonStat), JsonStatDataset.Serialize(deserializedGoldenMaster));
        }
        
        [Fact]
        public void CreateJsonStatWithNotDisplayed2_FromJsonSdmx_OK()
        {
            var converterFactory = new FromSdmxJsonToJsonStatConverterFactory(NullLoggerFactory.Instance);
            var jsonSdmx = File.ReadAllText("SDMXJson/NotDisplayedData2.json");
            var config = DefaultJsonStatConverterConfig.GetNew();
            config.TerritorialDimensionIds.Add("REF_AREA");
            var converter = converterFactory.GetConverter(jsonSdmx, "en", config, null);
            var jsonStat = converter.Convert();
            var jsonGoldenMaster = File.ReadAllText("SDMXJson/NotDisplayedGoldenMaster2.json");

            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonStat);
            var deserializedGoldenMaster = JsonStatDataset.Deserialize(jsonGoldenMaster);
            Assert.Equal(JsonStatDataset.Serialize(deserializedJsonStat), JsonStatDataset.Serialize(deserializedGoldenMaster));
        }

        [Fact]
        public void CreateJsonStatWithWholeDimensionNotDisplayed_FromJsonSdmx_OK()
        {
            var converterFactory = new FromSdmxJsonToJsonStatConverterFactory(NullLoggerFactory.Instance);
            var jsonSdmx = File.ReadAllText("SDMXJson/NotDisplayedWholeDimensionData.json");
            var converter = converterFactory.GetConverter(jsonSdmx, "en", DefaultJsonStatConverterConfig.GetNew(), null);
            var jsonStat = converter.Convert();
            var jsonGoldenMaster = File.ReadAllText("SDMXJson/NotDisplayedWholeDimensionGoldenMaster.json");

            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonStat);
            var deserializedGoldenMaster = JsonStatDataset.Deserialize(jsonGoldenMaster);
            Assert.Equal(JsonStatDataset.Serialize(deserializedJsonStat), JsonStatDataset.Serialize(deserializedGoldenMaster));
        }

        

        [Fact]
        public void CreateJsonWithMultiFrequencyTime_FromJsonSdmx_OK()
        {
            var converterFactory = new FromSdmxJsonToJsonStatConverterFactory(NullLoggerFactory.Instance);
            var jsonSdmx = File.ReadAllText("SDMXJson/MultiFreqData.json");
            var converter = converterFactory.GetConverter(jsonSdmx, "en", DefaultJsonStatConverterConfig.GetNew(), null);
            var jsonStat = converter.Convert();
            var jsonGoldenMaster = File.ReadAllText("SDMXJson/MultiFreqGoldenMaster.json");


            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonStat);
            var deserializedGoldenMaster = JsonStatDataset.Deserialize(jsonGoldenMaster);
            Assert.Equal(JsonStatDataset.Serialize(deserializedJsonStat), JsonStatDataset.Serialize(deserializedGoldenMaster));
        }


        [Fact]
        public void CreateJsonWithISO8601Time_FromJsonSdmx_OK()
        {
            var converterFactory = new FromSdmxJsonToJsonStatConverterFactory(NullLoggerFactory.Instance);
            var jsonSdmx = File.ReadAllText("SDMXJson/ISO8601DateData.json");
            var converter = converterFactory.GetConverter(jsonSdmx, "en", DefaultJsonStatConverterConfig.GetNew(), null);
            var jsonStat = converter.Convert();
            var jsonGoldenMaster = File.ReadAllText("SDMXJson/ISO8601DateGoldenMaster.json");

            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonStat);
            var deserializedGoldenMaster = JsonStatDataset.Deserialize(jsonGoldenMaster);
            Assert.Equal(JsonStatDataset.Serialize(deserializedJsonStat), JsonStatDataset.Serialize(deserializedGoldenMaster));
        }


        [Fact]
        public void CreateJsonWithSeries_FromJsonSdmx_OK()
        {
            var converterFactory = new FromSdmxJsonToJsonStatConverterFactory(NullLoggerFactory.Instance);
            var jsonSdmx = File.ReadAllText("SDMXJson/SeriesData.json");
            var converter = converterFactory.GetConverter(jsonSdmx, "en", DefaultJsonStatConverterConfig.GetNew(), null);
            var jsonStat = converter.Convert();
            var jsonGoldenMaster = File.ReadAllText("SDMXJson/SeriesDataGoldenMaster.json");

            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonStat);
            var deserializedGoldenMaster = JsonStatDataset.Deserialize(jsonGoldenMaster);
            Assert.Equal(JsonStatDataset.Serialize(deserializedJsonStat), JsonStatDataset.Serialize(deserializedGoldenMaster));
        }



        [Fact]
        public void CreateJsonWithSeriesAndMissingAttributes_FromJsonSdmx_OK()
        {
            var converterFactory = new FromSdmxJsonToJsonStatConverterFactory(NullLoggerFactory.Instance);
            var jsonSdmx = File.ReadAllText("SDMXJson/SeriesWithMissingAttribute.json");
            var converter = converterFactory.GetConverter(jsonSdmx, "en", DefaultJsonStatConverterConfig.GetNew(), null);
            var jsonStat = converter.Convert();
            var jsonGoldenMaster = File.ReadAllText("SDMXJson/SeriesWithMissingAttributeGoldenMaster.json");

            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonStat);
            var deserializedGoldenMaster = JsonStatDataset.Deserialize(jsonGoldenMaster);
            Assert.Equal(JsonStatDataset.Serialize(deserializedJsonStat), JsonStatDataset.Serialize(deserializedGoldenMaster));
        }

        
        public void CreateJsonWithHierarchicalCodelist_FromJsonSdmx_OK()
        {
            var converterFactory = new FromSdmxJsonToJsonStatConverterFactory(NullLoggerFactory.Instance);
            var jsonSdmx = File.ReadAllText("SDMXJson/HierarchyDFSData.json");
            var converter = converterFactory.GetConverter(jsonSdmx, "en", DefaultJsonStatConverterConfig.GetNew(), null);
            var jsonStat = converter.Convert();
            var jsonGoldenMaster = File.ReadAllText("SDMXJson/HierarchyDFSGoldenMaster.json");

            Assert.Equal(jsonGoldenMaster, jsonStat);
        }

        [Fact]
        public void Filter_Deserialize_OK()
        {
            var jsonstat = File.ReadAllText("SDMXJson/FilterDataGoldenMaster.json");
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonstat);
            var reSerializedJsonStat = JsonStatDataset.Serialize(deserializedJsonStat);

            var o1 = JObject.Parse(jsonstat);
            var o2 = JObject.Parse(reSerializedJsonStat);
            Assert.True(JToken.DeepEquals(o1, o2));
        }

        [Fact]
        public void FilterWithInvalidCriteria_Deserialize_OK()
        {
            var jsonstat = File.ReadAllText("SDMXJson/FilterWithInvalidCriteriaGoldenMaster.json");
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonstat);
            var reSerializedJsonStat = JsonStatDataset.Serialize(deserializedJsonStat);

            var o1 = JObject.Parse(jsonstat);
            var o2 = JObject.Parse(reSerializedJsonStat);
            Assert.True(JToken.DeepEquals(o1, o2));
        }



        [Fact]
        public void CreateJsonStat_FromJsonSdmx_Deserialize_OK()
        {
            var jsonstat = File.ReadAllText("SDMXJson/SDMXJsonData_converted_to_JSONStat.json");
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonstat);
            var reSerializedJsonStat = JsonStatDataset.Serialize(deserializedJsonStat);

            var o1 = JObject.Parse(jsonstat);
            var o2 = JObject.Parse(reSerializedJsonStat);
            Assert.True(JToken.DeepEquals(o1, o2));
        }


        [Fact]
        public void CreateJsonStat_FromSortedJsonSdmx_Deserialize_OK()
        {
            var jsonstat = File.ReadAllText("SDMXJson/SortedSDMXJsonData_converted_to_JSONStat.json");
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonstat);
            var reSerializedJsonStat = JsonStatDataset.Serialize(deserializedJsonStat);

            var o1 = JObject.Parse(jsonstat);
            var o2 = JObject.Parse(reSerializedJsonStat);
            Assert.True(JToken.DeepEquals(o1, o2));
        }


        [Fact]
        public void CreateJsonStatWithNotDisplayed_FromJsonSdmx_Deserialize_OK()
        {
            var jsonstat = File.ReadAllText("SDMXJson/NotDisplayedGoldenMaster.json");
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonstat);
            var reSerializedJsonStat = JsonStatDataset.Serialize(deserializedJsonStat);

            var o1 = JObject.Parse(jsonstat);
            var o2 = JObject.Parse(reSerializedJsonStat);
            Assert.True(JToken.DeepEquals(o1, o2));
        }

        [Fact]
        public void CreateJsonStatWithNotDisplayed2_FromJsonSdmx_Deserialize_Fail()
        {
            var jsonstat = File.ReadAllText("SDMXJson/NotDisplayedGoldenMaster2.json");
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonstat);
            deserializedJsonStat.Role.Geo.Add("zz");
            var reSerializedJsonStat = JsonStatDataset.Serialize(deserializedJsonStat);

            var o1 = JObject.Parse(jsonstat);
            var o2 = JObject.Parse(reSerializedJsonStat);
            Assert.False(JToken.DeepEquals(o1, o2));
        }

        [Fact]
        public void CreateJsonStatWithNotDisplayed2_FromJsonSdmx_Deserialize_OK()
        {
            var jsonstat = File.ReadAllText("SDMXJson/NotDisplayedGoldenMaster2.json");
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonstat);
            var reSerializedJsonStat = JsonStatDataset.Serialize(deserializedJsonStat);

            var o1 = JObject.Parse(jsonstat);
            var o2 = JObject.Parse(reSerializedJsonStat);
            Assert.True(JToken.DeepEquals(o1, o2));
        }

        [Fact]
        public void CreateJsonWithHierarchicalCodelist_FromJsonSdmx_Deserialize_OK()
        {
            var jsonstat = File.ReadAllText("SDMXJson/HierarchyDFSGoldenMaster.json");
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonstat);
            var reSerializedJsonStat = JsonStatDataset.Serialize(deserializedJsonStat);

            var o1 = JObject.Parse(jsonstat);
            var o2 = JObject.Parse(reSerializedJsonStat);
            Assert.True(JToken.DeepEquals(o1, o2));
        }

        [Fact]
        public void CreateJsonWithMultiFrequencyTime_FromJsonSdmx_Deserialize_OK()
        {
            var jsonstat = File.ReadAllText("SDMXJson/MultiFreqGoldenMaster.json");
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonstat);
            var reSerializedJsonStat = JsonStatDataset.Serialize(deserializedJsonStat);

            var o1 = JObject.Parse(jsonstat);
            var o2 = JObject.Parse(reSerializedJsonStat);
            Assert.True(JToken.DeepEquals(o1, o2));
        }


        [Fact]
        public void CreateJsonWithISO8601Time_FromJsonSdmx_Deserialize_OK()
        {
            var jsonstat = File.ReadAllText("SDMXJson/ISO8601DateGoldenMaster.json");
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonstat);
            var reSerializedJsonStat = JsonStatDataset.Serialize(deserializedJsonStat);

            var o1 = JObject.Parse(jsonstat);
            var o2 = JObject.Parse(reSerializedJsonStat);
            Assert.True(JToken.DeepEquals(o1, o2));
        }


        [Fact]
        public void CreateJsonWithSeries_FromJsonSdmx_Deserialize_OK()
        {
            var jsonstat = File.ReadAllText("SDMXJson/SeriesDataGoldenMaster.json");
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonstat);
            var reSerializedJsonStat = JsonStatDataset.Serialize(deserializedJsonStat);

            var o1 = JObject.Parse(jsonstat);
            var o2 = JObject.Parse(reSerializedJsonStat);
            Assert.True(JToken.DeepEquals(o1, o2));
        }

        [Fact]
        public void CreateJsonWithSeriesAndMissingAttributes_FromJsonSdmx_Deserialize_OK()
        {
            var jsonstat = File.ReadAllText("SDMXJson/SeriesWithMissingAttributeGoldenMaster.json");
            var deserializedJsonStat = JsonStatDataset.Deserialize(jsonstat);
            var reSerializedJsonStat = JsonStatDataset.Serialize(deserializedJsonStat);

            var o1 = JObject.Parse(jsonstat);
            var o2 = JObject.Parse(reSerializedJsonStat);
            Assert.True(JToken.DeepEquals(o1, o2));
        }
    }
}
