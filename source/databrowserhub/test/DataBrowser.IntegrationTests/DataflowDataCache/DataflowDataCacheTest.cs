using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DataBrowser.DomainServices.Interfaces;
using DataBrowser.IntegrationTests.Controllers;
using DataBrowser.IntegrationTests.HelperTest;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Cache;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Dto;
using DataBrowser.Interfaces.Dto.UseCases.Requests;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using EndPointConnector.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WSHUB;
using Xunit;

namespace DataBrowser.IntegrationTests.DataflowDataCache
{
    public class DataflowDataCacheTest : BaseControllerTest
    {
        public DataflowDataCacheTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
            UtilityTest.RemoveDataflowDataCacheIfExist();
        }

        

        [Fact]
        public async Task InsertDataCache_WithEmptyCache_Ok()
        {
            IOptionsSnapshot<DataflowDataCacheConfig> dataflowDataCacheConfig = null;
            IRequestContext requestContext = null;
            IDataflowDataCache dataflowDataCache = null;
            using (var scope = _factory.Services.CreateScope())
            {
                dataflowDataCacheConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DataflowDataCacheConfig>>();
                requestContext = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                dataflowDataCache = scope.ServiceProvider.GetRequiredService<IDataflowDataCache>();
                var datasetService = scope.ServiceProvider.GetRequiredService<IDatasetService>();


                var dataFromDataflowRequest =
                JsonConvert.DeserializeObject<DataFromDataflowRequest>(
                    File.ReadAllText("DataflowDataCache/MockData/request.json"));
                var cacheValue =
                    JsonConvert.DeserializeObject<GetDataFromDataflowResponse>(
                        File.ReadAllText("DataflowDataCache/MockData/dataFromDataflowResponse.json"));
                var dataflowInfo =
                    JsonConvert.DeserializeObject<Dataflow>(File.ReadAllText("DataflowDataCache/MockData/dataflow.json"));
                var dsdInfo = JsonConvert.DeserializeObject<Dsd>(File.ReadAllText("DataflowDataCache/MockData/dsd.json"));

                var dataset = datasetService.CreateDataset(dataflowInfo, dsdInfo, "en");
                await dataflowDataCache.SetJsonStatForDataflowData(dataFromDataflowRequest, cacheValue, dataset);

                string dbDataflowId = null;
                string dbFilter = null;
                string dbLanguage = null;
                string dbAnnotations = null;
                using (var conn = new SqliteConnection($"Data Source={UtilityTest.DataflowDataCacheFileName}"))
                {
                    conn.Open();


                    using (var scdCommand = new SqliteCommand
                    {
                        Connection = conn,
                        CommandText =
                            @"SELECT ci.DataflowId, cf.Filter, cf.Language, cf.Annotations FROM [CacheInfo] ci INNER JOIN [CacheFiles] cf ON cf.CacheInfoId=ci.Id WHERE [NodeId]=@NodeId AND [DataflowId]=@DataflowId LIMIT 1"
                    })
                    {
                        scdCommand.Parameters.Add(new SqliteParameter("@NodeId", DbType.String)
                        { Value = requestContext.NodeId });
                        scdCommand.Parameters.Add(new SqliteParameter("@DataflowId", DbType.String)
                        { Value = dataFromDataflowRequest.DataflowId });
                        var reader = await scdCommand.ExecuteReaderAsync();
                        if (reader.Read())
                        {
                            dbDataflowId = (string)reader["DataflowId"];
                            dbFilter = (string)reader["Filter"];
                            dbLanguage = (string)reader["Language"];
                            dbAnnotations = (string)reader["Annotations"];
                        }
                    }
                }

                var dataFilter =
                    "[{\"Id\":\"ITTER107\",\"FilterValues\":[\"IT\"]},{\"Id\":\"FREQ\",\"FilterValues\":[\"A\"]},{\"Id\":\"SERV_AEREO_TIPO\",\"FilterValues\":[\"ALL\"]}]";
                Assert.Equal(dataFilter, dbFilter);
                Assert.Equal("IT1+DF_TR_AEREO+1.0", dbDataflowId);
                Assert.Equal("EN", dbLanguage);
                Assert.Equal("MaxObs_500000:NotDisplay_[]", dbAnnotations);
            }
        }

        [Fact]
        public async Task IncreseAccessDataCache_Ok()
        {
            IOptionsSnapshot<DataflowDataCacheConfig> dataflowDataCacheConfig = null;
            IRequestContext requestContext = null;
            IDataflowDataCache dataflowDataCache = null;
            using (var scope = _factory.Services.CreateScope())
            {
                dataflowDataCacheConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DataflowDataCacheConfig>>();
                requestContext = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                dataflowDataCache = scope.ServiceProvider.GetRequiredService<IDataflowDataCache>();
                var datasetService = scope.ServiceProvider.GetRequiredService<IDatasetService>();

                var dataFromDataflowRequest =
                JsonConvert.DeserializeObject<DataFromDataflowRequest>(
                    File.ReadAllText("DataflowDataCache/MockData/request.json"));
                var cacheValue =
                    JsonConvert.DeserializeObject<GetDataFromDataflowResponse>(
                        File.ReadAllText("DataflowDataCache/MockData/dataFromDataflowResponse.json"));
                var dataflowInfo =
                    JsonConvert.DeserializeObject<Dataflow>(File.ReadAllText("DataflowDataCache/MockData/dataflow.json"));
                var dsdInfo = JsonConvert.DeserializeObject<Dsd>(File.ReadAllText("DataflowDataCache/MockData/dsd.json"));

                var dataset = datasetService.CreateDataset(dataflowInfo, dsdInfo, "en");
                await dataflowDataCache.SetJsonStatForDataflowData(dataFromDataflowRequest, cacheValue, dataset);

                //ATTENTION: for wrong business reason count start 1 instead of 0!!
                await dataflowDataCache.GetJsonStatForDataflowDataFromValidKey(dataFromDataflowRequest, dataset);
                await dataflowDataCache.GetJsonStatForDataflowDataFromValidKey(dataFromDataflowRequest, dataset);
                await dataflowDataCache.GetJsonStatForDataflowDataFromValidKey(dataFromDataflowRequest, dataset);
                await dataflowDataCache.GetJsonStatForDataflowDataFromValidKey(dataFromDataflowRequest, dataset);
                await dataflowDataCache.GetJsonStatForDataflowDataFromValidKey(dataFromDataflowRequest, dataset);

                var result = await dataflowDataCache.GetOnlyCachedKeyInfoDataflowData(dataFromDataflowRequest.DataflowId,
                    dataFromDataflowRequest.DataCriterias, false, dataset);

                Assert.Equal(6, result.Accesses);
            }
        }

        [Fact]
        public async Task AccessExpiredCache_ReturnNull()
        {
            IOptionsSnapshot<DataflowDataCacheConfig> dataflowDataCacheConfig = null;
            IRequestContext requestContext = null;
            IDataflowDataCache dataflowDataCache = null;
            using (var scope = _factory.Services.CreateScope())
            {
                dataflowDataCacheConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DataflowDataCacheConfig>>();
                requestContext = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                dataflowDataCache = scope.ServiceProvider.GetRequiredService<IDataflowDataCache>();
                var datasetService = scope.ServiceProvider.GetRequiredService<IDatasetService>();


                var dataFromDataflowRequest =
                JsonConvert.DeserializeObject<DataFromDataflowRequest>(
                    File.ReadAllText("DataflowDataCache/MockData/request.json"));
                var cacheValue =
                    JsonConvert.DeserializeObject<GetDataFromDataflowResponse>(
                        File.ReadAllText("DataflowDataCache/MockData/dataFromDataflowResponse.json"));
                var dataflowInfo =
                    JsonConvert.DeserializeObject<Dataflow>(File.ReadAllText("DataflowDataCache/MockData/dataflow.json"));
                var dsdInfo = JsonConvert.DeserializeObject<Dsd>(File.ReadAllText("DataflowDataCache/MockData/dsd.json"));

                var bk = dataflowDataCacheConfig.Value.Expiration;
                dataflowDataCacheConfig.Value.Expiration = -100;

                var dataset = datasetService.CreateDataset(dataflowInfo, dsdInfo, "en");

                await dataflowDataCache.SetJsonStatForDataflowData(dataFromDataflowRequest, cacheValue, dataset);

                var resultCache =
                    await dataflowDataCache.GetJsonStatForDataflowDataFromValidKey(dataFromDataflowRequest, dataset);
                Assert.Null(resultCache);
            }
        }

        [Fact]
        public async Task AccessExpiredCacheKey_WithExpiredTrue_ReturnKey()
        {
            IOptionsSnapshot<DataflowDataCacheConfig> dataflowDataCacheConfig = null;
            IRequestContext requestContext = null;
            IDataflowDataCache dataflowDataCache = null;
            using (var scope = _factory.Services.CreateScope())
            {
                dataflowDataCacheConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DataflowDataCacheConfig>>();
                requestContext = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                dataflowDataCache = scope.ServiceProvider.GetRequiredService<IDataflowDataCache>();
                var datasetService = scope.ServiceProvider.GetRequiredService<IDatasetService>();


                var dataFromDataflowRequest =
                JsonConvert.DeserializeObject<DataFromDataflowRequest>(
                    File.ReadAllText("DataflowDataCache/MockData/request.json"));
                var cacheValue =
                    JsonConvert.DeserializeObject<GetDataFromDataflowResponse>(
                        File.ReadAllText("DataflowDataCache/MockData/dataFromDataflowResponse.json"));
                var dataflowInfo =
                    JsonConvert.DeserializeObject<Dataflow>(File.ReadAllText("DataflowDataCache/MockData/dataflow.json"));
                var dsdInfo = JsonConvert.DeserializeObject<Dsd>(File.ReadAllText("DataflowDataCache/MockData/dsd.json"));

                var bk = dataflowDataCacheConfig.Value.Expiration;
                dataflowDataCacheConfig.Value.Expiration = -100;

                var dataset = datasetService.CreateDataset(dataflowInfo, dsdInfo, "en");

                await dataflowDataCache.SetJsonStatForDataflowData(dataFromDataflowRequest, cacheValue, dataset);
                dataflowDataCacheConfig.Value.Expiration = bk;

                var resultCache =
                    await dataflowDataCache.GetJsonStatForDataflowDataFromValidKey(dataFromDataflowRequest, dataset);
                Assert.Null(resultCache);

                var resultKey = await dataflowDataCache.GetOnlyCachedKeyInfoDataflowData(dataFromDataflowRequest.DataflowId,
                    dataFromDataflowRequest.DataCriterias, false, dataset);
                Assert.Null(resultKey);

                resultKey = await dataflowDataCache.GetOnlyCachedKeyInfoDataflowData(dataFromDataflowRequest.DataflowId,
                    dataFromDataflowRequest.DataCriterias, true, dataset);
                Assert.NotNull(resultKey);
            }
        }

        [Fact]
        public async Task SetNewValueForExpiredKey_ReNewKeyValidity()
        {
            IOptionsSnapshot<DataflowDataCacheConfig> dataflowDataCacheConfig = null;
            IRequestContext requestContext = null;
            IDataflowDataCache dataflowDataCache = null;
            using (var scope = _factory.Services.CreateScope())
            {
                dataflowDataCacheConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DataflowDataCacheConfig>>();
                requestContext = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                dataflowDataCache = scope.ServiceProvider.GetRequiredService<IDataflowDataCache>();
                var datasetService = scope.ServiceProvider.GetRequiredService<IDatasetService>();


                var dataFromDataflowRequest =
                JsonConvert.DeserializeObject<DataFromDataflowRequest>(
                    File.ReadAllText("DataflowDataCache/MockData/request.json"));
                var cacheValue =
                    JsonConvert.DeserializeObject<GetDataFromDataflowResponse>(
                        File.ReadAllText("DataflowDataCache/MockData/dataFromDataflowResponse.json"));
                var dataflowInfo =
                    JsonConvert.DeserializeObject<Dataflow>(File.ReadAllText("DataflowDataCache/MockData/dataflow.json"));
                var dsdInfo = JsonConvert.DeserializeObject<Dsd>(File.ReadAllText("DataflowDataCache/MockData/dsd.json"));

                var bk = dataflowDataCacheConfig.Value.Expiration;
                dataflowDataCacheConfig.Value.Expiration = -100;

                var dataset = datasetService.CreateDataset(dataflowInfo, dsdInfo, "en");

                await dataflowDataCache.SetJsonStatForDataflowData(dataFromDataflowRequest, cacheValue, dataset);
                dataflowDataCacheConfig.Value.Expiration = bk;

                var resultCache =
                    await dataflowDataCache.GetJsonStatForDataflowDataFromValidKey(dataFromDataflowRequest, dataset);
                Assert.Null(resultCache);

                var resultKey = await dataflowDataCache.GetOnlyCachedKeyInfoDataflowData(dataFromDataflowRequest.DataflowId,
                    dataFromDataflowRequest.DataCriterias, true, dataset);
                await dataflowDataCache.UpdateDataflowTTLFromNodeId(resultKey.CacheInfoId, 7200);
                await dataflowDataCache.SetJsonStatForDataflowData(dataFromDataflowRequest, cacheValue, dataset);

                resultCache =
                    await dataflowDataCache.GetJsonStatForDataflowDataFromValidKey(dataFromDataflowRequest, dataset);
                Assert.NotNull(resultCache);
            }
        }

        [Fact]
        public async Task GetDataFromEmptyCache_ReturnNullValue()
        {
            IOptionsSnapshot<DataflowDataCacheConfig> dataflowDataCacheConfig = null;
            IRequestContext requestContext = null;
            IDataflowDataCache dataflowDataCache = null;
            using (var scope = _factory.Services.CreateScope())
            {
                dataflowDataCacheConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DataflowDataCacheConfig>>();
                requestContext = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                dataflowDataCache = scope.ServiceProvider.GetRequiredService<IDataflowDataCache>();
                var datasetService = scope.ServiceProvider.GetRequiredService<IDatasetService>();


                var dataFromDataflowRequest =
                JsonConvert.DeserializeObject<DataFromDataflowRequest>(
                    File.ReadAllText("DataflowDataCache/MockData/request.json"));
                var cacheValue =
                    JsonConvert.DeserializeObject<GetDataFromDataflowResponse>(
                        File.ReadAllText("DataflowDataCache/MockData/dataFromDataflowResponse.json"));
                var dataflowInfo =
                    JsonConvert.DeserializeObject<Dataflow>(File.ReadAllText("DataflowDataCache/MockData/dataflow.json"));
                var dsdInfo = JsonConvert.DeserializeObject<Dsd>(File.ReadAllText("DataflowDataCache/MockData/dsd.json"));

                var dataset = datasetService.CreateDataset(dataflowInfo, dsdInfo, "en");

                var result =
                    await dataflowDataCache.GetJsonStatForDataflowDataFromValidKey(dataFromDataflowRequest, dataset);

                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetDataWithValueInCache_ReturnValue()
        {
            IOptionsSnapshot<DataflowDataCacheConfig> dataflowDataCacheConfig = null;
            IRequestContext requestContext = null;
            IDataflowDataCache dataflowDataCache = null;
            using (var scope = _factory.Services.CreateScope())
            {
                dataflowDataCacheConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DataflowDataCacheConfig>>();
                requestContext = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                dataflowDataCache = scope.ServiceProvider.GetRequiredService<IDataflowDataCache>();
                var datasetService = scope.ServiceProvider.GetRequiredService<IDatasetService>();


                var dataFromDataflowRequest =
                JsonConvert.DeserializeObject<DataFromDataflowRequest>(
                    File.ReadAllText("DataflowDataCache/MockData/request.json"));
                var cacheValue =
                    JsonConvert.DeserializeObject<GetDataFromDataflowResponse>(
                        File.ReadAllText("DataflowDataCache/MockData/dataFromDataflowResponse.json"));
                var dataflowInfo =
                    JsonConvert.DeserializeObject<Dataflow>(File.ReadAllText("DataflowDataCache/MockData/dataflow.json"));
                var dsdInfo = JsonConvert.DeserializeObject<Dsd>(File.ReadAllText("DataflowDataCache/MockData/dsd.json"));

                var dataset = datasetService.CreateDataset(dataflowInfo, dsdInfo, "en");

                await dataflowDataCache.SetJsonStatForDataflowData(dataFromDataflowRequest, cacheValue, dataset);

                var result =
                    await dataflowDataCache.GetJsonStatForDataflowDataFromValidKey(dataFromDataflowRequest, dataset);

                Assert.NotNull(result);
                Assert.Equal(File.ReadAllText("DataflowDataCache/MockData/cacheDataJson.json"),
                    JsonConvert.SerializeObject(result.JsonData));
            }
        }

        [Fact]
        public async Task GetDataWithValueInCache_AnnotationChange_ReturnNullValue()
        {
            IOptionsSnapshot<DataflowDataCacheConfig> dataflowDataCacheConfig = null;
            IRequestContext requestContext = null;
            IDataflowDataCache dataflowDataCache = null;
            using (var scope = _factory.Services.CreateScope())
            {
                dataflowDataCacheConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DataflowDataCacheConfig>>();
                requestContext = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                dataflowDataCache = scope.ServiceProvider.GetRequiredService<IDataflowDataCache>();
                var datasetService = scope.ServiceProvider.GetRequiredService<IDatasetService>();


                var dataFromDataflowRequest =
                JsonConvert.DeserializeObject<DataFromDataflowRequest>(
                    File.ReadAllText("DataflowDataCache/MockData/request.json"));
                var cacheValue =
                    JsonConvert.DeserializeObject<GetDataFromDataflowResponse>(
                        File.ReadAllText("DataflowDataCache/MockData/dataFromDataflowResponse.json"));
                var dataflowInfo =
                    JsonConvert.DeserializeObject<Dataflow>(File.ReadAllText("DataflowDataCache/MockData/dataflow.json"));
                var dsdInfo = JsonConvert.DeserializeObject<Dsd>(File.ReadAllText("DataflowDataCache/MockData/dsd.json"));

                var dataset = datasetService.CreateDataset(dataflowInfo, dsdInfo, "it");

                await dataflowDataCache.SetJsonStatForDataflowData(dataFromDataflowRequest, cacheValue, dataset);

                dataset.MaxObservation = 5000;

                var result =
                    await dataflowDataCache.GetJsonStatForDataflowDataFromValidKey(dataFromDataflowRequest, dataset);

                Assert.Null(result);
            }
        }

        [Fact]
        public async Task ClearSingleItemCache_WithNodeIdCorrect_DoClean()
        {
            IOptionsSnapshot<DataflowDataCacheConfig> dataflowDataCacheConfig = null;
            IRequestContext requestContext = null;
            IDataflowDataCache dataflowDataCache = null;
            using (var scope = _factory.Services.CreateScope())
            {
                dataflowDataCacheConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DataflowDataCacheConfig>>();
                requestContext = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                dataflowDataCache = scope.ServiceProvider.GetRequiredService<IDataflowDataCache>();
                var datasetService = scope.ServiceProvider.GetRequiredService<IDatasetService>();


                var dataFromDataflowRequest =
                JsonConvert.DeserializeObject<DataFromDataflowRequest>(
                    File.ReadAllText("DataflowDataCache/MockData/request.json"));
                var cacheValue =
                    JsonConvert.DeserializeObject<GetDataFromDataflowResponse>(
                        File.ReadAllText("DataflowDataCache/MockData/dataFromDataflowResponse.json"));
                var dataflowInfo =
                    JsonConvert.DeserializeObject<Dataflow>(File.ReadAllText("DataflowDataCache/MockData/dataflow.json"));
                var dsdInfo = JsonConvert.DeserializeObject<Dsd>(File.ReadAllText("DataflowDataCache/MockData/dsd.json"));

                var dataset = datasetService.CreateDataset(dataflowInfo, dsdInfo, "en");

                await dataflowDataCache.SetJsonStatForDataflowData(dataFromDataflowRequest, cacheValue, dataset);

                var cacheKey = await dataflowDataCache.GetOnlyCachedKeyInfoDataflowData(dataFromDataflowRequest.DataflowId,
                    dataFromDataflowRequest.DataCriterias, true, dataset);
                Assert.True(cacheKey.FileSize > 0);
                await dataflowDataCache.ClearSingleItemCache(cacheKey.CacheInfoId, requestContext.NodeId);

                cacheKey = await dataflowDataCache.GetOnlyCachedKeyInfoDataflowData(dataflowInfo.Id,
                    dataFromDataflowRequest.DataCriterias, true, dataset);
                Assert.True(cacheKey.FileSize == 0);

                var result =
                    await dataflowDataCache.GetJsonStatForDataflowDataFromValidKey(dataFromDataflowRequest, dataset);

                Assert.Null(result);
            }
        }

        [Fact]
        public async Task ClearSingleItemCache_WithWrongNodeId_NotClean()
        {
            IOptionsSnapshot<DataflowDataCacheConfig> dataflowDataCacheConfig = null;
            IRequestContext requestContext = null;
            IDataflowDataCache dataflowDataCache = null;
            using (var scope = _factory.Services.CreateScope())
            {
                dataflowDataCacheConfig = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DataflowDataCacheConfig>>();
                requestContext = scope.ServiceProvider.GetRequiredService<IRequestContext>();
                dataflowDataCache = scope.ServiceProvider.GetRequiredService<IDataflowDataCache>();
                var datasetService = scope.ServiceProvider.GetRequiredService<IDatasetService>();


                var dataFromDataflowRequest =
                    JsonConvert.DeserializeObject<DataFromDataflowRequest>(
                        File.ReadAllText("DataflowDataCache/MockData/request.json"));
                var cacheValue =
                    JsonConvert.DeserializeObject<GetDataFromDataflowResponse>(
                        File.ReadAllText("DataflowDataCache/MockData/dataFromDataflowResponse.json"));
                var dataflowInfo =
                    JsonConvert.DeserializeObject<Dataflow>(File.ReadAllText("DataflowDataCache/MockData/dataflow.json"));
                var dsdInfo = JsonConvert.DeserializeObject<Dsd>(File.ReadAllText("DataflowDataCache/MockData/dsd.json"));

                var dataset = datasetService.CreateDataset(dataflowInfo, dsdInfo, "en");

                await dataflowDataCache.SetJsonStatForDataflowData(dataFromDataflowRequest, cacheValue, dataset);

                var cacheKey = await dataflowDataCache.GetOnlyCachedKeyInfoDataflowData(dataflowInfo.Id,
                    dataFromDataflowRequest.DataCriterias, true, dataset);
                Assert.True(cacheKey.FileSize > 0);
                await dataflowDataCache.ClearSingleItemCache(cacheKey.CacheFileId, 1234567);

                cacheKey = await dataflowDataCache.GetOnlyCachedKeyInfoDataflowData(dataflowInfo.Id,
                    dataFromDataflowRequest.DataCriterias, true, dataset);
                Assert.True(cacheKey.FileSize > 0);

                var result =
                    await dataflowDataCache.GetJsonStatForDataflowDataFromValidKey(dataFromDataflowRequest, dataset);

                Assert.NotNull(result);
                Assert.Equal(File.ReadAllText("DataflowDataCache/MockData/cacheDataJson.json"),
                    JsonConvert.SerializeObject(result.JsonData));
            }
        }

        [Fact]
        public async Task CreateDataflowCacheInfo_Ok()
        {
            var nodeId = 1;

            var newData = new DataflowDataCacheInfo
            {
                Id = Guid.Empty,
                NodeId = nodeId,
                DataflowId = "test-identifier,IT1,1.0",
                TTL = 10000,
                CacheSize = 0,
                CachedDataflow = 0,
                CachedDataAccess = 0,
                Title = "Title"
            };

            var token = UtilityTest.GetAdminToken(httpClient).Result.Token;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.PostAsync($"/DataflowCache/DataflowData/Nodes/{nodeId}",
                new StringContent(JsonConvert.SerializeObject(newData), Encoding.UTF8, "application/json"));


            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());

            var strResult = await response.Content.ReadAsStringAsync();
            var creationResult = JsonConvert.DeserializeObject<DataflowDataCacheInfo>(strResult);
            Assert.NotEqual(creationResult?.Id, Guid.Empty);
        }

        [Fact]
        public async Task CreateExistingDataflowCacheInfoMustUpdate()
        {
            var nodeId = 1;
            var expectedTTL = 5;
            var dataflowId = "test-identifier,IT1,1.0";

            var dataToBeCreated = new DataflowDataCacheInfo
            {
                Id = Guid.Empty,
                NodeId = nodeId,
                DataflowId = dataflowId,
                TTL = 99999,
                CacheSize = 0,
                CachedDataflow = 0,
                CachedDataAccess = 0,
                Title = "Title"
            };

            var token = UtilityTest.GetAdminToken(httpClient).Result.Token;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.PostAsync($"/DataflowCache/DataflowData/Nodes/{nodeId}",
                new StringContent(JsonConvert.SerializeObject(dataToBeCreated), Encoding.UTF8, "application/json"));


            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var strResult = await response.Content.ReadAsStringAsync();
            var creationResult = JsonConvert.DeserializeObject<DataflowDataCacheInfo>(strResult);

            //same guid will result in update instead of creation
            var dataToBeUpdated = new DataflowDataCacheInfo
            {
                TTL = expectedTTL,
                DataflowId = dataflowId,
                NodeId = -1
            };

            response = await httpClient.PostAsync($"/DataflowCache/DataflowData/Nodes/{nodeId}",
                new StringContent(JsonConvert.SerializeObject(dataToBeUpdated), Encoding.UTF8, "application/json"));


            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());

            strResult = await response.Content.ReadAsStringAsync();
            var updateResult = JsonConvert.DeserializeObject<DataflowDataCacheInfo>(strResult);

            Assert.Equal(updateResult.TTL, expectedTTL);
            Assert.Equal(creationResult.DataflowId, updateResult.DataflowId);
        }

    }
}