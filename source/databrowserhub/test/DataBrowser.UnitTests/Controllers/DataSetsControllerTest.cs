using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Dto.UseCases.Requests;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using DataBrowser.Query.Nodes;
using DataBrowser.Services.Interfaces;
using DataBrowser.UnitTests.HelperTest;
using EndPointConnector.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WSHUB.Controllers;
using Xunit;

namespace DataBrowser.UnitTests.Controllers
{
    public class DataSetsControllerTest
    {
        private readonly Mock<IRequestContext> _requestContextMock;

        public DataSetsControllerTest()
        {
            _requestContextMock = new Mock<IRequestContext>();
            _requestContextMock.Setup(x => x.NodeId).Returns(1);
            _requestContextMock.Setup(x => x.NodeCode).Returns("TestCode");
            _requestContextMock.Setup(x => x.IgnoreCache).Returns(true);
            _requestContextMock.Setup(x => x.UserLang).Returns("IT");
            _requestContextMock.Setup(x => x.LoggedUserId).Returns(-1);
        }

        [Fact]
        public async Task CriteriaStructure_Find_200()
        {
            var _mediatorMock = new Mock<IMediatorService>();
            _mediatorMock.Setup(x => x.Send(It.IsAny<NodeIsActiveQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var _loggerMock = new Mock<ILogger<DataSetsController>>();
            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var dataSetsController = new DataSetsController(_loggerMock.Object, _requestContextMock.Object,
                _contexAccessortMock.Object, _mediatorMock.Object);

            // Arrange
            var response = new StructureCriteriaForDataflowResponse();
            var criteria = Utility.GenerateCriteria();
            response.CriteriaMode = "CriteriaModeTest";
            response.Criterias = criteria;
            _mediatorMock.Setup(x => x.Send(It.IsAny<StructureCriteriaForDataflowRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var actionResult = await dataSetsController.GetStructure("1", "dataset+id+1.1");

            // Assert
            Assert.IsType<ContentResult>(actionResult);
            var jsonContentResult = actionResult as ContentResult;
            Assert.Equal(200, jsonContentResult.StatusCode);
            Assert.Equal("application/json", jsonContentResult.ContentType);
            Assert.Equal(
                "{\"criteria\":[{\"id\":\"Id1\",\"label\":\"Titolo IT\"},{\"id\":\"IdTwo\",\"label\":\"title IdTwo FR\"},{\"id\":\"IdOther\",\"label\":\"title IdOther EN\"}],\"criteriaView\":\"CriteriaModeTest\",\"decimalPlaces\":0}",
                jsonContentResult.Content);
        }

        [Fact]
        public async Task CriteriaStructure_NotFound_404()
        {
            var _mediatorMock = new Mock<IMediatorService>();
            _mediatorMock.Setup(x => x.Send(It.IsAny<NodeIsActiveQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var _loggerMock = new Mock<ILogger<DataSetsController>>();
            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var dataSetsController = new DataSetsController(_loggerMock.Object, _requestContextMock.Object,
                _contexAccessortMock.Object, _mediatorMock.Object);

            // Arrange
            _mediatorMock.Setup(x => x.Send(It.IsAny<StructureCriteriaForDataflowRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((StructureCriteriaForDataflowResponse) null);

            // Act
            var actionResult = await dataSetsController.GetStructure("1", "dataset+id+1.1");

            // Assert
            Assert.IsType<ContentResult>(actionResult);
            var jsonContentResult = actionResult as ContentResult;
            Assert.Equal(404, jsonContentResult.StatusCode);
            Assert.Equal("application/text", jsonContentResult.ContentType);
            Assert.Equal("Dataset not found", jsonContentResult.Content);
        }

        [Fact]
        public async Task CriteriaStructure_NodeNotFound_404()
        {
            var _mediatorMock = new Mock<IMediatorService>();
            _mediatorMock.Setup(x => x.Send(It.IsAny<NodeIsActiveQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var _loggerMock = new Mock<ILogger<DataSetsController>>();
            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var dataSetsController = new DataSetsController(_loggerMock.Object, _requestContextMock.Object,
                _contexAccessortMock.Object, _mediatorMock.Object);

            // Arrange
            _mediatorMock.Setup(x => x.Send(It.IsAny<StructureCriteriaForDataflowRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((StructureCriteriaForDataflowResponse) null);

            // Act
            var actionResult = await dataSetsController.GetStructure("1", "dataset+id+1.1");

            // Assert
            Assert.IsType<ContentResult>(actionResult);
            var jsonContentResult = actionResult as ContentResult;
            Assert.Equal(404, jsonContentResult.StatusCode);
            Assert.Equal("application/text", jsonContentResult.ContentType);
            Assert.Equal("Node not found", jsonContentResult.Content);
        }

        [Fact]
        public async Task CodelistPartial_ItemsFind_200()
        {
            var _mediatorMock = new Mock<IMediatorService>();
            _mediatorMock.Setup(x => x.Send(It.IsAny<NodeIsActiveQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var _loggerMock = new Mock<ILogger<DataSetsController>>();
            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var dataSetsController = new DataSetsController(_loggerMock.Object, _requestContextMock.Object,
                _contexAccessortMock.Object, _mediatorMock.Object);

            // Arrange
            var response = new GetCodelistPartialForDataflowResponse();
            response.ArtefactContainer = new ArtefactContainer();
            response.ArtefactContainer.Criterias = Utility.GenerateCriteria();
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetCodelistPartialForDataflowRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var actionResult = await dataSetsController.GetPartial("1", "dataset+id+1.1", null);

            // Assert
            Assert.IsType<ContentResult>(actionResult);
            var jsonContentResult = actionResult as ContentResult;
            Assert.Equal(200, jsonContentResult.StatusCode);
            Assert.Equal("application/json", jsonContentResult.ContentType);
            Assert.Equal(
                "{\"criteria\":[{\"id\":\"Id1\",\"label\":\"Titolo IT\",\"values\":[{\"id\":\"A\",\"name\":\"A IT\",\"isDefault\":false},{\"id\":\"B\",\"name\":\"B EN\",\"isDefault\":false},{\"id\":\"C\",\"parentId\":\"B\",\"name\":\"C IT\",\"isDefault\":false},{\"id\":\"D\",\"parentId\":\"C\",\"name\":\"D EN\",\"isDefault\":false},{\"id\":\"E\",\"parentId\":\"A\",\"name\":\"E IT\",\"isDefault\":false},{\"id\":\"F\",\"name\":\"F IT\",\"isDefault\":false},{\"id\":\"G\",\"parentId\":\"C\",\"name\":\"G IT\",\"isDefault\":false}]},{\"id\":\"IdTwo\",\"label\":\"title IdTwo FR\",\"values\":[{\"id\":\"A2\",\"name\":\"A2 FR\",\"isDefault\":false},{\"id\":\"B2\",\"name\":\"B2 EN\",\"isDefault\":false},{\"id\":\"C2\",\"parentId\":\"B2\",\"name\":\"C2 IT\",\"isDefault\":false},{\"id\":\"D2\",\"parentId\":\"C2\",\"name\":\"D2 DE\",\"isDefault\":false},{\"id\":\"E2\",\"parentId\":\"A2\",\"name\":\"E ES\",\"isDefault\":false},{\"id\":\"F2\",\"name\":\"F2 IT\",\"isDefault\":false},{\"id\":\"G2\",\"parentId\":\"C2\",\"name\":\"G2 IT\",\"isDefault\":false}]},{\"id\":\"IdOther\",\"label\":\"title IdOther EN\",\"values\":[{\"id\":\"A3\",\"name\":\"A3 FR\",\"isDefault\":false},{\"id\":\"B3\",\"name\":\"B3 EN\",\"isDefault\":false},{\"id\":\"C3\",\"parentId\":\"B3\",\"name\":\"C3 IT\",\"isDefault\":false},{\"id\":\"D3\",\"parentId\":\"C3\",\"name\":\"D3 DE\",\"isDefault\":false},{\"id\":\"E3\",\"parentId\":\"A3\",\"name\":\"E ES\",\"isDefault\":false},{\"id\":\"F3\",\"name\":\"F3 IT\",\"isDefault\":false},{\"id\":\"G3\",\"parentId\":\"C3\",\"name\":\"G3 IT\",\"isDefault\":false}]}],\"decimalPlaces\":0}",
                jsonContentResult.Content);
        }

        [Fact]
        public async Task CodelistPartial_SingleItemFind_200()
        {
            var _mediatorMock = new Mock<IMediatorService>();
            _mediatorMock.Setup(x => x.Send(It.IsAny<NodeIsActiveQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var _loggerMock = new Mock<ILogger<DataSetsController>>();
            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var dataSetsController = new DataSetsController(_loggerMock.Object, _requestContextMock.Object,
                _contexAccessortMock.Object, _mediatorMock.Object);

            // Arrange
            var response = new GetCodelistPartialForDataflowResponse();
            response.ArtefactContainer = new ArtefactContainer();
            response.ArtefactContainer.Criterias = new List<Criteria> {Utility.GenerateCriteria().First()};
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetCodelistPartialForDataflowRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var actionResult = await dataSetsController.GetPartial("1", "dataset+id+1.1", "Id1");

            // Assert
            Assert.IsType<ContentResult>(actionResult);
            var jsonContentResult = actionResult as ContentResult;
            Assert.Equal(200, jsonContentResult.StatusCode);
            Assert.Equal("application/json", jsonContentResult.ContentType);
            Assert.Equal(
                "{\"criteria\":[{\"id\":\"Id1\",\"label\":\"Titolo IT\",\"values\":[{\"id\":\"A\",\"name\":\"A IT\",\"isDefault\":false},{\"id\":\"B\",\"name\":\"B EN\",\"isDefault\":false},{\"id\":\"C\",\"parentId\":\"B\",\"name\":\"C IT\",\"isDefault\":false},{\"id\":\"D\",\"parentId\":\"C\",\"name\":\"D EN\",\"isDefault\":false},{\"id\":\"E\",\"parentId\":\"A\",\"name\":\"E IT\",\"isDefault\":false},{\"id\":\"F\",\"name\":\"F IT\",\"isDefault\":false},{\"id\":\"G\",\"parentId\":\"C\",\"name\":\"G IT\",\"isDefault\":false}]}],\"decimalPlaces\":0}",
                jsonContentResult.Content);
        }

        [Fact]
        public async Task CodelistPartial_ItemsNotFound_404()
        {
            var _mediatorMock = new Mock<IMediatorService>();
            _mediatorMock.Setup(x => x.Send(It.IsAny<NodeIsActiveQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var _loggerMock = new Mock<ILogger<DataSetsController>>();
            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var dataSetsController = new DataSetsController(_loggerMock.Object, _requestContextMock.Object,
                _contexAccessortMock.Object, _mediatorMock.Object);

            // Arrange
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetCodelistPartialForDataflowRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetCodelistPartialForDataflowResponse) null);

            // Act
            var actionResult = await dataSetsController.GetPartial("1", "dataset+id+1.1", null);

            // Assert
            Assert.IsType<ContentResult>(actionResult);
            var jsonContentResult = actionResult as ContentResult;
            Assert.Equal(404, jsonContentResult.StatusCode);
            Assert.Equal("application/text", jsonContentResult.ContentType);
            Assert.Equal("Dataset not found", jsonContentResult.Content);
        }

        [Fact]
        public async Task CodelistPartial_NodeNotFound_400()
        {
            var _mediatorMock = new Mock<IMediatorService>();
            _mediatorMock.Setup(x => x.Send(It.IsAny<NodeIsActiveQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var _loggerMock = new Mock<ILogger<DataSetsController>>();
            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var dataSetsController = new DataSetsController(_loggerMock.Object, _requestContextMock.Object,
                _contexAccessortMock.Object, _mediatorMock.Object);

            // Arrange
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetCodelistPartialForDataflowRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetCodelistPartialForDataflowResponse) null);

            // Act
            var actionResult = await dataSetsController.GetPartial("1", "dataset+id+1.1", null);

            // Assert
            Assert.IsType<ContentResult>(actionResult);
            var jsonContentResult = actionResult as ContentResult;
            Assert.Equal(400, jsonContentResult.StatusCode);
            Assert.Equal("application/text", jsonContentResult.ContentType);
            Assert.Equal("Node not found", jsonContentResult.Content);
        }
    }
}