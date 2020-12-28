using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DataBrowser.IntegrationTests.HelperTest;
using Newtonsoft.Json;
using WSHUB;
using WSHUB.Models.Response;
using Xunit;

namespace DataBrowser.IntegrationTests.Controllers
{
    public class NodeControllerTest : BaseControllerTest
    {
        public NodeControllerTest(CustomWebApplicationFactory<Startup> factory)
            : base(factory)
        {
            var token = UtilityTest.GetAdminToken(httpClient).Result.Token;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        [Theory]
        [InlineData("/Nodes/2")]
        [InlineData("/Nodes/4")]
        public async Task Get_NodeNotActive_ReturnNotFoundAndCorrectContentType(string url)
        {
            // Act
            httpClient.DefaultRequestHeaders.Authorization = null;
            var response = await httpClient.GetAsync(url);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
            Assert.Equal("application/text", response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/Nodes/1")]
        [InlineData("/Nodes/3")]
        [InlineData("/Nodes/5")]
        public async Task Get_NodeActive_ReturnSuccessAndCorrectContentType(string url)
        {
            // Act
            httpClient.DefaultRequestHeaders.Authorization = null;
            var response = await httpClient.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Get_AllNodeActive_ReturnSuccessAndCorrectContentType()
        {
            // Act
            httpClient.DefaultRequestHeaders.Authorization = null;
            var response = await httpClient.GetAsync("/Nodes");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());

            var strResult = await response.Content.ReadAsStringAsync();
            var nodes = JsonConvert.DeserializeObject<List<NodeModelView>>(strResult);

            Assert.Equal(3, nodes.Count);

            Assert.Contains(nodes, i => i.Code.Equals("TestFede3", StringComparison.InvariantCultureIgnoreCase));
            Assert.Contains(nodes, i => i.Code.Equals("Other", StringComparison.InvariantCultureIgnoreCase));
            Assert.Contains(nodes, i => i.Code.Equals("NodeFull", StringComparison.InvariantCultureIgnoreCase));
        }

        [Theory]
        [InlineData(1)]
        public async Task Update_Node_ReturnSuccessAndCorrectContentType(int nodeId)
        {
            // Act
            var json = @"{
	""nodeId"": " + nodeId + @",
    ""type"": ""SDMX-REST"",
    ""code"": ""TestTEST" + nodeId + @""",
    ""logo"": ""Logoooo"",
    ""endPoint"": ""http://url.com/rest"",
    ""order"": 1,
    ""active"": true,
    ""enableHttpAuth"": 1,
    ""authHttpUsername"": ""admin"",
    ""authHttpPassword"": ""admin"",
    ""Extras"": [
        {
            ""Key"": ""Prefix"",
            ""Value"": ""web"",
            ""IsPublic"": 1,
            ""Transaltes"": { ""EN"": ""webEN""}
        },
        {
            ""Key"": ""XmlResultNeedFix"",
            ""Value"": 0,
            ""IsPublic"": 1
        },
        {
            ""Key"": ""AnnotationOrder"",
            ""Value"": ""{ \""CodeList\"": \""ORDER\"", \""Concept\"": \""ORDER\"" }""
        },
        {
            ""Key"": ""ExcludeCategorySchema"",
            ""Value"": ""[ \""ESTAT+ESTAT_DATAFLOWS_SCHEME+2.0\"", \""ESTAT+ESTAT_DATAFLOWS_SCHEME+3.0\"", \""INPS+CAT_SCHEME_INPS+1.0\"", \""INPS+ISTAT_DW+1.0\"", \""IT1+ISTAT_DW+1.0\"", \""IT1+TEST+1.0\"", \""ESTAT+ESTAT_DATAFLOWS_SCHEME+2.4\"", \""IT1+DDB_TEST+1.0\"", \""IT1+TEST_CIPO+1.1\"", \""IT1+TEST_DD+1.0\"", \""SDMX+TESTFEDE+1.0\"" ]""
        }
    ]
}";
            var response = await httpClient.PutAsync($"/Nodes/{nodeId}",
                new StringContent(json, Encoding.UTF8, "application/json"));

            // Assert

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());
        }
    }
}