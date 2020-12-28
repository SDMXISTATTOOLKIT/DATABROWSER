using System.Collections.Generic;
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
    public class FilterDashboardTest
    {
        [Fact]
        public async Task PrivateDashboard_AdministratorUser_AuthActive_AllowRead()
        {
            //Arrange  
            var nodeId = 1;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = true
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageTemplate),
                        new Claim(ClaimTypes.Role, UserAndGroup.RoleAdministrator)
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var subject = new FilterDashboard(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = DashboardUtility.CreatePrivateDashboard(1, nodeId);

            var result = subject.CheckReadPermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PrivateDashboard_UnregistredUser_AuthActive_DenyRead()
        {
            //Arrange  
            var nodeId = 1;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = true
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var subject = new FilterDashboard(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = DashboardUtility.CreatePrivateDashboard(1, nodeId);

            var result = subject.CheckReadPermission(viewTemplateDto);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task PrivateDashboard_AdministratorUser_AuthDisable_AllowRead()
        {
            //Arrange  
            var nodeId = 1;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = false
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageTemplate),
                        new Claim(ClaimTypes.Role, UserAndGroup.RoleAdministrator)
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var subject = new FilterDashboard(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = DashboardUtility.CreatePrivateDashboard(1, nodeId);

            var result = subject.CheckReadPermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PrivateDashboard_UnregistredUser_AuthDisable_AllowRead()
        {
            //Arrange  
            var nodeId = 1;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = false
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var subject = new FilterDashboard(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = DashboardUtility.CreatePrivateDashboard(1, 1234);

            var result = subject.CheckReadPermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PublicDashboardHub_AdministratorUser_AuthActive_AllowRead()
        {
            //Arrange  
            var nodeId = 1;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = true
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageTemplate),
                        new Claim(ClaimTypes.Role, UserAndGroup.RoleAdministrator)
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var subject = new FilterDashboard(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = DashboardUtility.CreatePublicDashboard(1, nodeId, 1);

            var result = subject.CheckReadPermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PublicDashboardHub_UnregistredUser_AuthActive_AllowRead()
        {
            //Arrange  
            var nodeId = 1;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = true
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var subject = new FilterDashboard(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = DashboardUtility.CreatePublicDashboard(1, nodeId, 1);

            var result = subject.CheckReadPermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PublicDashboardHub_AdministratorUser_AuthDisable_AllowRead()
        {
            //Arrange  
            var nodeId = 1;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = false
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageTemplate),
                        new Claim(ClaimTypes.Role, UserAndGroup.RoleAdministrator)
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var subject = new FilterDashboard(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = DashboardUtility.CreatePublicDashboard(1, nodeId, 1);

            var result = subject.CheckReadPermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PublicDashboardHub_UnregistredUser_AuthDisable_AllowRead()
        {
            //Arrange  
            var nodeId = 1;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = false
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            var subject = new FilterDashboard(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = DashboardUtility.CreatePublicDashboard(1, nodeId, 1);

            var result = subject.CheckReadPermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PublicDashboardNodes_AdministratorUser_AuthActive_AllowRead()
        {
            //Arrange  
            var nodeId = 1;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = true
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageTemplate),
                        new Claim(ClaimTypes.Role, UserAndGroup.RoleAdministrator)
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var subject = new FilterDashboard(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = DashboardUtility.CreatePublicDashboard(1, nodeId, new List<int> {1});

            var result = subject.CheckReadPermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PublicDashboardNodes_UnregistredUser_AuthActive_AllowRead()
        {
            //Arrange  
            var nodeId = 1;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = true
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            var subject = new FilterDashboard(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = DashboardUtility.CreatePublicDashboard(1, nodeId, new List<int> {1});

            var result = subject.CheckReadPermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PublicDashboardNodes_AdministratorUser_AuthDisable_AllowRead()
        {
            //Arrange  
            var nodeId = 1;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = false
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageTemplate),
                        new Claim(ClaimTypes.Role, UserAndGroup.RoleAdministrator)
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var subject = new FilterDashboard(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = DashboardUtility.CreatePublicDashboard(1, nodeId, new List<int> {1});

            var result = subject.CheckReadPermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PublicDashboardNodes_UnregistredUser_AuthDisable_AllowRead()
        {
            //Arrange  
            var nodeId = 1;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = false
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var subject = new FilterDashboard(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = DashboardUtility.CreatePublicDashboard(1, nodeId, new List<int> {1});

            var result = subject.CheckReadPermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PrivateDashboard_AdministratorUser_AuthActive_AllowWrite()
        {
            //Arrange  
            var nodeId = 1;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = true
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageTemplate),
                        new Claim(ClaimTypes.Role, UserAndGroup.RoleAdministrator)
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var subject = new FilterDashboard(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = DashboardUtility.CreatePrivateDashboard(1, nodeId);

            var result = subject.CheckWritePermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PrivateDashboard_UnregistredUser_AuthActive_DenyWrite()
        {
            //Arrange  
            var nodeId = 1;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = true
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            var subject = new FilterDashboard(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = DashboardUtility.CreatePrivateDashboard(1, nodeId);

            var result = subject.CheckWritePermission(viewTemplateDto);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task PrivateDashboard_AdministratorUser_AuthDisable_AllowWrite()
        {
            //Arrange  
            var nodeId = 1;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = false
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageTemplate),
                        new Claim(ClaimTypes.Role, UserAndGroup.RoleAdministrator)
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var subject = new FilterDashboard(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = DashboardUtility.CreatePrivateDashboard(1, nodeId);

            var result = subject.CheckWritePermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PrivateDashboard_UnregistredUser_AuthDisable_AllowWrite()
        {
            //Arrange  
            var nodeId = 1;

            var authenticationConfig = new AuthenticationConfig
            {
                IsActive = false
            };
            var mockOptionAuthConfig = new Mock<IOptionsSnapshot<AuthenticationConfig>>();
            mockOptionAuthConfig.Setup(m => m.Value).Returns(authenticationConfig);

            var _contexAccessortMock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var subject = new FilterDashboard(_contexAccessortMock.Object, mockOptionAuthConfig.Object,
                _filterNodeMock.Object);

            //Act
            var viewTemplateDto = DashboardUtility.CreatePrivateDashboard(1, nodeId);

            var result = subject.CheckWritePermission(viewTemplateDto);

            //Assert
            Assert.True(result);
        }
    }
}