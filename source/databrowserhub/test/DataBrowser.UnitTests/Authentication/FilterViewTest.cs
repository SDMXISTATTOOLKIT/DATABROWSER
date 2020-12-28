using System.Security.Claims;
using System.Threading.Tasks;
using DataBrowser.AuthenticationAuthorization.Filters;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Constants;
using DataBrowser.UnitTests.HelperTest;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DataBrowser.UnitTests.Authentication
{
    public class FilterViewTest
    {
        [Fact]
        public async Task PermissionSingleNode_AuthActive_CurrentUser_Allow()
        {
            //Arrange  
            var nodeId = 1;
            var userId = 10;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = true
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageView),
                        new Claim(ClaimValues.UserId, userId.ToString())
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterView(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = ViewTemplateUtility.CreateView(nodeId, userId);

            var result = subject.CheckPermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PermissionSingleNode_AuthActive_WrongUser_Deny()
        {
            //Arrange  
            var nodeId = 1;
            var userId = 10;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = true
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageView),
                        new Claim(ClaimValues.UserId, userId.ToString())
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterView(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = ViewTemplateUtility.CreateView(nodeId, userId + 100);

            var result = subject.CheckPermission(viewTemplateDto);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task PermissionSingleNode_AuthDisable_WrongUser_Allow()
        {
            //Arrange  
            var nodeId = 1;
            var userId = 10;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = false
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageView),
                        new Claim(ClaimValues.UserId, userId.ToString())
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterView(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = ViewTemplateUtility.CreateView(nodeId, userId + 100);

            var result = subject.CheckPermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PermissionSingleNode_AuthActive_WrongType_Deny()
        {
            //Arrange  
            var nodeId = 1;
            var userId = 10;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = true
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageView),
                        new Claim(ClaimValues.UserId, userId.ToString())
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterView(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = ViewTemplateUtility.CreateTemplate(nodeId);

            var result = subject.CheckPermission(viewTemplateDto);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task PermissionSingleNode_AuthDisable_WrongType_Deny()
        {
            //Arrange  
            var nodeId = 1;
            var userId = 10;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = true
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageView),
                        new Claim(ClaimValues.UserId, userId.ToString())
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterView(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = ViewTemplateUtility.CreateTemplate(nodeId);

            var result = subject.CheckPermission(viewTemplateDto);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task PermissionSingleNode_AuthDisable_CurrentUser_Allow()
        {
            //Arrange  
            var nodeId = 1;
            var userId = 10;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = false
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageView),
                        new Claim(ClaimValues.UserId, userId.ToString())
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterView(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = ViewTemplateUtility.CreateView(nodeId, userId);

            var result = subject.CheckPermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task NoPermissionSingleNode_AuthActive_CurrentUser_Deny()
        {
            //Arrange  
            var nodeId = 1;
            var userId = 10;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = true
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageView),
                        new Claim(ClaimValues.UserId, userId.ToString())
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterView(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = ViewTemplateUtility.CreateView(nodeId + 1, userId);

            var result = subject.CheckPermission(viewTemplateDto);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task NoPermissionSingleNode_AuthDisable_CurrentUser_Allow()
        {
            //Arrange  
            var nodeId = 1;
            var userId = 10;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = false
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageView),
                        new Claim(ClaimValues.UserId, userId.ToString())
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterView(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = ViewTemplateUtility.CreateView(nodeId + 1, userId);

            var result = subject.CheckPermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PermissionCrossNode_AuthActive_CurrentUser_Allow()
        {
            //Arrange  
            var nodeId = 1;
            var userId = 10;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = true
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionCroosNodeType}", PolicyName.ManageView),
                        new Claim(ClaimValues.UserId, userId.ToString())
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterView(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = ViewTemplateUtility.CreateView(nodeId, userId);

            var result = subject.CheckPermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SingleAndCrossPermissionNode_AuthActive_CurrentUser_Allow()
        {
            //Arrange  
            var nodeId = 1;
            var userId = 10;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = true
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionCroosNodeType}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageView),
                        new Claim(ClaimValues.UserId, userId.ToString())
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterView(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = ViewTemplateUtility.CreateView(nodeId, userId);

            var result = subject.CheckPermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task MultiPermissionNode_AuthActive_CurrentUser_Allow()
        {
            //Arrange  
            var nodeId = 1;
            var userId = 10;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = true
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionCroosNodeType}", PolicyName.ManageCache),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 3}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 4}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageTemplate),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", PolicyName.ManageTemplate),
                        new Claim(ClaimValues.UserId, userId.ToString())
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterView(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = ViewTemplateUtility.CreateView(nodeId, userId);

            var result = subject.CheckPermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task MultiPermissionNodeTwo_AuthActive_CurrentUser_Allow()
        {
            //Arrange  
            var nodeId = 1;
            var userId = 10;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = true
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionCroosNodeType}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 3}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 4}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", PolicyName.ManageTemplate),
                        new Claim(ClaimValues.UserId, userId.ToString())
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterView(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = ViewTemplateUtility.CreateView(nodeId, userId);

            var result = subject.CheckPermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }
    }
}