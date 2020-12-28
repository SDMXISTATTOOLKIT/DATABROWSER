using System.Security.Claims;
using System.Threading.Tasks;
using DataBrowser.AuthenticationAuthorization.Policy;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DataBrowser.UnitTests.Authentication
{
    public class AllowManageConfigPolicyTest
    {
        private readonly Mock<IRequestContext> _requestContextMock;

        public AllowManageConfigPolicyTest()
        {
            _requestContextMock = new Mock<IRequestContext>();
            _requestContextMock.Setup(x => x.NodeId).Returns(1);
            _requestContextMock.Setup(x => x.NodeCode).Returns("TestCode");
            _requestContextMock.Setup(x => x.IgnoreCache).Returns(true);
            _requestContextMock.Setup(x => x.UserLang).Returns("IT");
            _requestContextMock.Setup(x => x.LoggedUserId).Returns(1);
        }

        [Fact]
        public async Task FilterNodeTrue_AuthActive_Allow()
        {
            //Arrange  
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = true
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);


            var requirements = new[] {new AllowManageConfigPolicy()};
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim("Permission_SingleNode_1", "ManageConfig")
                    },
                    "Basic")
            );

            var context = new AuthorizationHandlerContext(requirements, user, null);
            var subject = new AllowManageConfigHandler(_requestContextMock.Object, _filterNodeMock.Object,
                mockOptionAuthConfig.Object);

            //Act
            await subject.HandleAsync(context);

            //Assert
            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task FilterNodeTrue_AuthDisable_Allow()
        {
            //Arrange  

            var nodeId = 1;
            var _requestContextMock = new Mock<IRequestContext>();
            _requestContextMock.Setup(x => x.NodeId).Returns(nodeId);
            _requestContextMock.Setup(x => x.NodeCode).Returns("TestCode");
            _requestContextMock.Setup(x => x.IgnoreCache).Returns(true);
            _requestContextMock.Setup(x => x.UserLang).Returns("IT");
            _requestContextMock.Setup(x => x.LoggedUserId).Returns(1);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = false
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);


            var requirements = new[] {new AllowManageConfigPolicy()};
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim("Permission_SingleNode_1", "ManageConfig")
                    },
                    "Basic")
            );

            var context = new AuthorizationHandlerContext(requirements, user, null);
            var subject = new AllowManageConfigHandler(_requestContextMock.Object, _filterNodeMock.Object,
                mockOptionAuthConfig.Object);

            //Act
            await subject.HandleAsync(context);

            //Assert
            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task FilterNodeFalse_AuthDisable_Allow()
        {
            //Arrange  

            var nodeId = 1;
            var _requestContextMock = new Mock<IRequestContext>();
            _requestContextMock.Setup(x => x.NodeId).Returns(nodeId);
            _requestContextMock.Setup(x => x.NodeCode).Returns("TestCode");
            _requestContextMock.Setup(x => x.IgnoreCache).Returns(true);
            _requestContextMock.Setup(x => x.UserLang).Returns("IT");
            _requestContextMock.Setup(x => x.LoggedUserId).Returns(1);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = false
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);


            var requirements = new[] {new AllowManageConfigPolicy()};
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim("Permission_SingleNode_1", "ManageConfig")
                    },
                    "Basic")
            );

            var context = new AuthorizationHandlerContext(requirements, user, null);
            var subject = new AllowManageConfigHandler(_requestContextMock.Object, _filterNodeMock.Object,
                mockOptionAuthConfig.Object);

            //Act
            await subject.HandleAsync(context);

            //Assert
            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task FilterNodeFalse_AuthActive_Deny()
        {
            //Arrange  

            var nodeId = 1;
            var _requestContextMock = new Mock<IRequestContext>();
            _requestContextMock.Setup(x => x.NodeId).Returns(nodeId);
            _requestContextMock.Setup(x => x.NodeCode).Returns("TestCode");
            _requestContextMock.Setup(x => x.IgnoreCache).Returns(true);
            _requestContextMock.Setup(x => x.UserLang).Returns("IT");
            _requestContextMock.Setup(x => x.LoggedUserId).Returns(1);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = true
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);


            var requirements = new[] {new AllowManageConfigPolicy()};
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim("Permission_SingleNode_2", "ManageConfig")
                    },
                    "Basic")
            );

            var context = new AuthorizationHandlerContext(requirements, user, null);
            var subject = new AllowManageConfigHandler(_requestContextMock.Object, _filterNodeMock.Object,
                mockOptionAuthConfig.Object);

            //Act
            await subject.HandleAsync(context);

            //Assert
            Assert.False(context.HasSucceeded);
        }
    }
}