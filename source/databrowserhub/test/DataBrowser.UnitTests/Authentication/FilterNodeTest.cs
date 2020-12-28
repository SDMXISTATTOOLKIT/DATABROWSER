using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DataBrowser.AuthenticationAuthorization.Filters;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DataBrowser.UnitTests.Authentication
{
    public class FilterNodeTest
    {
        [Theory]
        [InlineData("ManageCache")]
        [InlineData("ManageConfig")]
        [InlineData("ManageView")]
        [InlineData("ManageTemplate")]
        public async Task PermissionSingleNode_AuthActive_CurrentUser_Allow(string strPermissionType)
        {
            //Arrange  
            var permissionType = getEnumPermission(strPermissionType);
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
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", permissionType.ToString())
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, permissionType, nodeId, null);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("ManageCache")]
        [InlineData("ManageConfig")]
        [InlineData("ManageView")]
        [InlineData("ManageTemplate")]
        public async Task PermissionSingleNode_AuthDisable_CurrentUser_Allow(string strPermissionType)
        {
            //Arrange  
            var permissionType = getEnumPermission(strPermissionType);
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
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", permissionType.ToString())
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, permissionType, nodeId, null);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("ManageCache")]
        [InlineData("ManageConfig")]
        [InlineData("ManageView")]
        [InlineData("ManageTemplate")]
        public async Task NoPermissionSingleNode_AuthActive_CurrentUser_Deny(string strPermissionType)
        {
            //Arrange  
            var permissionType = getEnumPermission(strPermissionType);
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
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", permissionType.ToString())
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, permissionType, nodeId + 1, null);

            //Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("ManageCache")]
        [InlineData("ManageConfig")]
        [InlineData("ManageView")]
        [InlineData("ManageTemplate")]
        public async Task NoPermissionSingleNode_AuthDisable_CurrentUser_Allow(string strPermissionType)
        {
            //Arrange  
            var permissionType = getEnumPermission(strPermissionType);
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
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", permissionType.ToString())
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, permissionType, nodeId + 1, null);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("ManageCache")]
        [InlineData("ManageConfig")]
        [InlineData("ManageView")]
        [InlineData("ManageTemplate")]
        public async Task PermissionCrossNode_AuthActive_CurrentUser_Allow(string strPermissionType)
        {
            //Arrange  
            var permissionType = getEnumPermission(strPermissionType);
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
                        new Claim($"{PermissionType.PermissionCroosNodeType}", permissionType.ToString())
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, permissionType, nodeId, null);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("ManageCache")]
        [InlineData("ManageConfig")]
        [InlineData("ManageView")]
        [InlineData("ManageTemplate")]
        public async Task SingleCrossPermissionNode_AuthActive_CurrentUser_Allow(string strPermissionType)
        {
            //Arrange  
            var permissionType = getEnumPermission(strPermissionType);
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
                        new Claim($"{PermissionType.PermissionCroosNodeType}", permissionType.ToString()),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", permissionType.ToString())
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, permissionType, nodeId, null);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Template_MultiPermissionNode_AuthActive_CurrentUser_Allow()
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
                        new Claim($"{PermissionType.PermissionCroosNodeType}", PolicyName.ManageCache),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageTemplate),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", PolicyName.ManageTemplate),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", PolicyName.ManageTemplate),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 3}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 4}", PolicyName.ManageTemplate),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", PolicyName.ManageTemplate),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", PolicyName.ManageView)
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, PermissionType.NodePermission.ManageTemplate, nodeId, null);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Template_MultiPermissionNodeTwo_AuthActive_CurrentUser_Allow()
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
                        new Claim($"{PermissionType.PermissionCroosNodeType}", PolicyName.ManageTemplate),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", PolicyName.ManageTemplate),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", PolicyName.ManageTemplate),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 3}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 4}", PolicyName.ManageTemplate),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", PolicyName.ManageTemplate),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", PolicyName.ManageView)
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, PermissionType.NodePermission.ManageTemplate, nodeId, null);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task View_MultiPermissionNode_AuthActive_CurrentUser_Allow()
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
                        new Claim($"{PermissionType.PermissionCroosNodeType}", PolicyName.ManageCache),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 3}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 4}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", PolicyName.ManageTemplate)
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, PermissionType.NodePermission.ManageView, nodeId, null);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task View_MultiPermissionNodeTwo_AuthActive_CurrentUser_Allow()
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
                        new Claim($"{PermissionType.PermissionCroosNodeType}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 3}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 4}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", PolicyName.ManageTemplate)
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, PermissionType.NodePermission.ManageView, nodeId, null);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Config_MultiPermissionNode_AuthActive_CurrentUser_Allow()
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
                        new Claim($"{PermissionType.PermissionCroosNodeType}", PolicyName.ManageCache),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 3}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 4}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", PolicyName.ManageView),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", PolicyName.ManageTemplate)
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, PermissionType.NodePermission.ManageConfig, nodeId, null);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Config_MultiPermissionNodeTwo_AuthActive_CurrentUser_Allow()
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
                        new Claim($"{PermissionType.PermissionCroosNodeType}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 3}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 4}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", PolicyName.ManageConfig),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageTemplate")
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, PermissionType.NodePermission.ManageView, nodeId, null);

            //Assert
            Assert.True(result);
        }


        [Fact]
        public async Task Cache_MultiPermissionNode_AuthActive_CurrentUser_Allow()
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
                        new Claim($"{PermissionType.PermissionCroosNodeType}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 3}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 4}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageTemplate")
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, PermissionType.NodePermission.ManageCache, nodeId, null);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Cache_MultiPermissionNodeTwo_AuthActive_CurrentUser_Allow()
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
                        new Claim($"{PermissionType.PermissionCroosNodeType}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 3}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 4}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageTemplate")
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, PermissionType.NodePermission.ManageCache, nodeId, null);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("ManageCache")]
        [InlineData("ManageConfig")]
        [InlineData("ManageView")]
        [InlineData("ManageTemplate")]
        public async Task PermissionSingleNode_AuthActive_SpecifictUser_Allow(string strPermissionType)
        {
            //Arrange  
            var permissionType = getEnumPermission(strPermissionType);
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
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 100}", "ManageTemplate")
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);


            var specificUser = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", permissionType.ToString())
                    },
                    "Basic")
            );

            //Act
            var result = executeCheckPermission(subject, permissionType, nodeId, specificUser);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("ManageCache")]
        [InlineData("ManageConfig")]
        [InlineData("ManageView")]
        [InlineData("ManageTemplate")]
        public async Task PermissionSingleNode_AuthDisable_SpecificUser_Allow(string strPermissionType)
        {
            //Arrange  
            var permissionType = getEnumPermission(strPermissionType);
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
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 100}", "ManageTemplate")
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var specificUser = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", permissionType.ToString())
                    },
                    "Basic")
            );

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, permissionType, nodeId, specificUser);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("ManageCache")]
        [InlineData("ManageConfig")]
        [InlineData("ManageView")]
        [InlineData("ManageTemplate")]
        public async Task NoPermissionSingleNode_AuthActive_SpecificUser_Deny(string strPermissionType)
        {
            //Arrange  
            var permissionType = getEnumPermission(strPermissionType);
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
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", permissionType.ToString())
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            var specificUser = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", permissionType.ToString())
                    },
                    "Basic")
            );

            //Act
            var result = executeCheckPermission(subject, permissionType, nodeId + 1, specificUser);

            //Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("ManageCache")]
        [InlineData("ManageConfig")]
        [InlineData("ManageView")]
        [InlineData("ManageTemplate")]
        public async Task NoPermissionSingleNode_AuthDisable_SpecificUser_Allow(string strPermissionType)
        {
            //Arrange  
            var permissionType = getEnumPermission(strPermissionType);
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
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 100}", "ManageTemplate")
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var specificUser = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", permissionType.ToString())
                    },
                    "Basic")
            );

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, permissionType, nodeId + 1, specificUser);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("ManageCache")]
        [InlineData("ManageConfig")]
        [InlineData("ManageView")]
        [InlineData("ManageTemplate")]
        public async Task PermissionCrossNode_AuthActive_SpecificUser_Allow(string strPermissionType)
        {
            //Arrange  
            var permissionType = getEnumPermission(strPermissionType);
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
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 100}", "ManageTemplate")
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var specificUser = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionCroosNodeType}", permissionType.ToString())
                    },
                    "Basic")
            );

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, permissionType, nodeId, specificUser);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("ManageCache")]
        [InlineData("ManageConfig")]
        [InlineData("ManageView")]
        [InlineData("ManageTemplate")]
        public async Task SingleCrossPermissionNode_AuthActive_SpecificUser_Allow(string strPermissionType)
        {
            //Arrange  
            var permissionType = getEnumPermission(strPermissionType);
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
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 100}", "ManageTemplate")
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var specificUser = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionCroosNodeType}", permissionType.ToString()),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", permissionType.ToString())
                    },
                    "Basic")
            );

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, permissionType, nodeId, specificUser);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Template_MultiPermissionNode_AuthActive_SpecificUser_Allow()
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
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 100}", "ManageTemplate")
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var specificUser = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionCroosNodeType}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", "ManageTemplate"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageTemplate"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageTemplate"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 3}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 4}", "ManageTemplate"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageTemplate"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageView")
                    },
                    "Basic")
            );

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, PermissionType.NodePermission.ManageTemplate, nodeId,
                specificUser);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Template_MultiPermissionNodeTwo_AuthActive_SpecificUser_Allow()
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
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 100}", "ManageTemplate")
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var specificUser = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionCroosNodeType}", "ManageTemplate"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageTemplate"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageTemplate"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 3}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 4}", "ManageTemplate"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageTemplate"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageView")
                    },
                    "Basic")
            );

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, PermissionType.NodePermission.ManageTemplate, nodeId,
                specificUser);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task View_MultiPermissionNode_AuthActive_SpecificUser_Allow()
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
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 100}", "ManageView")
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var specificUser = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionCroosNodeType}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 3}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 4}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageTemplate")
                    },
                    "Basic")
            );

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result =
                executeCheckPermission(subject, PermissionType.NodePermission.ManageView, nodeId, specificUser);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task View_MultiPermissionNodeTwo_AuthActive_SpecificUser_Allow()
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
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 100}", "ManageView")
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var specificUser = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionCroosNodeType}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 3}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 4}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageTemplate")
                    },
                    "Basic")
            );

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result =
                executeCheckPermission(subject, PermissionType.NodePermission.ManageView, nodeId, specificUser);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Config_MultiPermissionNode_AuthActive_SpecificUser_Allow()
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
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 100}", "ManageConfig")
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var specificUser = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionCroosNodeType}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 3}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 4}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageTemplate")
                    },
                    "Basic")
            );

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result = executeCheckPermission(subject, PermissionType.NodePermission.ManageConfig, nodeId,
                specificUser);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Config_MultiPermissionNodeTwo_AuthActive_SpecificUser_Allow()
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
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 100}", "ManageConfig")
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var specificUser = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionCroosNodeType}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 3}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 4}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageTemplate")
                    },
                    "Basic")
            );

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result =
                executeCheckPermission(subject, PermissionType.NodePermission.ManageView, nodeId, specificUser);

            //Assert
            Assert.True(result);
        }


        [Fact]
        public async Task Cache_MultiPermissionNode_AuthActive_SpecificUser_Allow()
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
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 100}", "ManageCache")
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var specificUser = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionCroosNodeType}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 3}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 4}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageTemplate")
                    },
                    "Basic")
            );

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result =
                executeCheckPermission(subject, PermissionType.NodePermission.ManageCache, nodeId, specificUser);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Cache_MultiPermissionNodeTwo_AuthActive_SpecificUser_Allow()
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
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 100}", "ManageCache")
                    },
                    "Basic")
            );
            _contexAccessortMock.Setup(x => x.HttpContext).Returns(context);

            var specificUser = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim($"{PermissionType.PermissionCroosNodeType}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 3}", "ManageConfig"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 4}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId}", "ManageView"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 1}", "ManageCache"),
                        new Claim($"{PermissionType.PermissionSingleNodeType}{nodeId + 2}", "ManageTemplate")
                    },
                    "Basic")
            );

            var subject = new FilterNode(_contexAccessortMock.Object, mockOptionAuthConfig.Object);

            //Act
            var result =
                executeCheckPermission(subject, PermissionType.NodePermission.ManageCache, nodeId, specificUser);

            //Assert
            Assert.True(result);
        }

        private PermissionType.NodePermission getEnumPermission(string name)
        {
            switch (name.ToUpperInvariant())
            {
                case "MANAGECACHE":
                    return PermissionType.NodePermission.ManageCache;
                case "MANAGECONFIG":
                    return PermissionType.NodePermission.ManageConfig;
                case "MANAGETEMPLATE":
                    return PermissionType.NodePermission.ManageTemplate;
                case "MANAGEVIEW":
                    return PermissionType.NodePermission.ManageView;
            }

            throw new ArgumentOutOfRangeException();
        }

        private bool executeCheckPermission(FilterNode filterNode, PermissionType.NodePermission nodePermission,
            int nodeId, ClaimsPrincipal specificUser)
        {
            switch (nodePermission)
            {
                case PermissionType.NodePermission.ManageCache:
                    return filterNode.CheckPermissionNodeManageCache(nodeId, specificUser);
                case PermissionType.NodePermission.ManageConfig:
                    return filterNode.CheckPermissionNodeManageConfig(nodeId, specificUser);
                case PermissionType.NodePermission.ManageTemplate:
                    return filterNode.CheckPermissionNodeManageTemplate(nodeId, specificUser);
                case PermissionType.NodePermission.ManageView:
                    return filterNode.CheckPermissionNodeManageView(nodeId, specificUser);
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}