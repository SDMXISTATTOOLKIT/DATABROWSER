using System.Security.Claims;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Query.Nodes;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DataBrowser.UnitTests.Query.Nodes
{
    public class NodesHandlerUtilityTest
    {
        private readonly Mock<IRequestContext> _requestContextMock;

        public NodesHandlerUtilityTest()
        {
            _requestContextMock = new Mock<IRequestContext>();
            _requestContextMock.Setup(x => x.NodeId).Returns(1);
            _requestContextMock.Setup(x => x.NodeCode).Returns("TestCode");
            _requestContextMock.Setup(x => x.IgnoreCache).Returns(true);
            _requestContextMock.Setup(x => x.UserLang).Returns("IT");
            _requestContextMock.Setup(x => x.LoggedUserId).Returns(1);
        }

        [Fact]
        public void CheckNodePermissionConfigOrCache_HaveConfig_Allow()
        {
            var _loggerMock = new Mock<ILogger>();
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageCache(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock
                .Setup(x => x.CheckPermissionNodeManageTemplate(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);

            var testNodeQueryBase = new TestNodeQueryBase();
            testNodeQueryBase.FilterByPermissionNodeCache = true;
            testNodeQueryBase.FilterByPermissionNodeConfig = true;
            testNodeQueryBase.FilterIsInAnd = false;
            var result = NodesHandlerUtility.CheckPermissionNode(testNodeQueryBase, _requestContextMock.Object.NodeId,
                _filterNodeMock.Object, _loggerMock.Object);

            Assert.True(result);
        }

        [Fact]
        public void CheckNodePermissionConfigOrCache_HaveCache_Allow()
        {
            var _loggerMock = new Mock<ILogger>();
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageCache(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock
                .Setup(x => x.CheckPermissionNodeManageTemplate(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);

            var testNodeQueryBase = new TestNodeQueryBase();
            testNodeQueryBase.FilterByPermissionNodeCache = true;
            testNodeQueryBase.FilterByPermissionNodeConfig = true;
            testNodeQueryBase.FilterIsInAnd = false;
            var result = NodesHandlerUtility.CheckPermissionNode(testNodeQueryBase, _requestContextMock.Object.NodeId,
                _filterNodeMock.Object, _loggerMock.Object);

            Assert.True(result);
        }

        [Fact]
        public void CheckNodePermissionConfigOrCache_Any_Deny()
        {
            var _loggerMock = new Mock<ILogger>();
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageCache(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock
                .Setup(x => x.CheckPermissionNodeManageTemplate(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var testNodeQueryBase = new TestNodeQueryBase();
            testNodeQueryBase.FilterByPermissionNodeCache = true;
            testNodeQueryBase.FilterByPermissionNodeConfig = true;
            testNodeQueryBase.FilterIsInAnd = false;
            var result = NodesHandlerUtility.CheckPermissionNode(testNodeQueryBase, _requestContextMock.Object.NodeId,
                _filterNodeMock.Object, _loggerMock.Object);

            Assert.False(result);
        }

        [Fact]
        public void CheckNodePermissionConfigAndCache_HaveCacheConfig_Allow()
        {
            var _loggerMock = new Mock<ILogger>();
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageCache(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock
                .Setup(x => x.CheckPermissionNodeManageTemplate(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);

            var testNodeQueryBase = new TestNodeQueryBase();
            testNodeQueryBase.FilterByPermissionNodeCache = true;
            testNodeQueryBase.FilterByPermissionNodeConfig = true;
            testNodeQueryBase.FilterIsInAnd = true;
            var result = NodesHandlerUtility.CheckPermissionNode(testNodeQueryBase, _requestContextMock.Object.NodeId,
                _filterNodeMock.Object, _loggerMock.Object);

            Assert.True(result);
        }

        [Fact]
        public void CheckNodePermissionConfigAndCache_HaveCache_Deny()
        {
            var _loggerMock = new Mock<ILogger>();
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageCache(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock
                .Setup(x => x.CheckPermissionNodeManageTemplate(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);

            var testNodeQueryBase = new TestNodeQueryBase();
            testNodeQueryBase.FilterByPermissionNodeCache = true;
            testNodeQueryBase.FilterByPermissionNodeConfig = true;
            testNodeQueryBase.FilterIsInAnd = true;
            var result = NodesHandlerUtility.CheckPermissionNode(testNodeQueryBase, _requestContextMock.Object.NodeId,
                _filterNodeMock.Object, _loggerMock.Object);

            Assert.False(result);
        }

        [Fact]
        public void CheckNodePermissionAll_HaveAll_Allow()
        {
            var _loggerMock = new Mock<ILogger>();
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageCache(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock
                .Setup(x => x.CheckPermissionNodeManageTemplate(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var testNodeQueryBase = new TestNodeQueryBase();
            testNodeQueryBase.FilterByPermissionNodeCache = true;
            testNodeQueryBase.FilterByPermissionNodeConfig = true;
            testNodeQueryBase.FilterByPermissionNodeTemplate = true;
            testNodeQueryBase.FilterByPermissionNodeView = true;
            testNodeQueryBase.FilterIsInAnd = true;
            var result = NodesHandlerUtility.CheckPermissionNode(testNodeQueryBase, _requestContextMock.Object.NodeId,
                _filterNodeMock.Object, _loggerMock.Object);

            Assert.True(result);
        }

        [Fact]
        public void CheckNodePermissionAll_HaveNotAll_Deny()
        {
            var _loggerMock = new Mock<ILogger>();
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageCache(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock
                .Setup(x => x.CheckPermissionNodeManageTemplate(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);

            var testNodeQueryBase = new TestNodeQueryBase();
            testNodeQueryBase.FilterByPermissionNodeCache = true;
            testNodeQueryBase.FilterByPermissionNodeConfig = true;
            testNodeQueryBase.FilterByPermissionNodeTemplate = true;
            testNodeQueryBase.FilterByPermissionNodeView = true;
            testNodeQueryBase.FilterIsInAnd = true;
            var result = NodesHandlerUtility.CheckPermissionNode(testNodeQueryBase, _requestContextMock.Object.NodeId,
                _filterNodeMock.Object, _loggerMock.Object);

            Assert.False(result);
        }

        [Fact]
        public void CheckNodePermissionHaveOneOrMore_HaveOnlyOne_Allow()
        {
            var _loggerMock = new Mock<ILogger>();
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageCache(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock
                .Setup(x => x.CheckPermissionNodeManageTemplate(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var testNodeQueryBase = new TestNodeQueryBase();
            testNodeQueryBase.FilterByPermissionNodeCache = true;
            testNodeQueryBase.FilterByPermissionNodeConfig = true;
            testNodeQueryBase.FilterByPermissionNodeTemplate = true;
            testNodeQueryBase.FilterByPermissionNodeView = true;
            testNodeQueryBase.FilterIsInAnd = false;
            var result = NodesHandlerUtility.CheckPermissionNode(testNodeQueryBase, _requestContextMock.Object.NodeId,
                _filterNodeMock.Object, _loggerMock.Object);

            Assert.True(result);
        }

        [Fact]
        public void CheckNodePermissionHaveOneOrMore_HaveTwo_Allow()
        {
            var _loggerMock = new Mock<ILogger>();
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageCache(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock
                .Setup(x => x.CheckPermissionNodeManageTemplate(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var testNodeQueryBase = new TestNodeQueryBase();
            testNodeQueryBase.FilterByPermissionNodeCache = true;
            testNodeQueryBase.FilterByPermissionNodeConfig = true;
            testNodeQueryBase.FilterByPermissionNodeTemplate = true;
            testNodeQueryBase.FilterByPermissionNodeView = true;
            testNodeQueryBase.FilterIsInAnd = false;
            var result = NodesHandlerUtility.CheckPermissionNode(testNodeQueryBase, _requestContextMock.Object.NodeId,
                _filterNodeMock.Object, _loggerMock.Object);

            Assert.True(result);
        }

        [Fact]
        public void CheckNodePermissionConfig_HaveConfig_Allow()
        {
            var _loggerMock = new Mock<ILogger>();
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageCache(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock
                .Setup(x => x.CheckPermissionNodeManageTemplate(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);

            var testNodeQueryBase = new TestNodeQueryBase();
            testNodeQueryBase.FilterByPermissionNodeCache = false;
            testNodeQueryBase.FilterByPermissionNodeConfig = true;
            testNodeQueryBase.FilterByPermissionNodeTemplate = false;
            testNodeQueryBase.FilterByPermissionNodeView = false;
            testNodeQueryBase.FilterIsInAnd = true;
            var result = NodesHandlerUtility.CheckPermissionNode(testNodeQueryBase, _requestContextMock.Object.NodeId,
                _filterNodeMock.Object, _loggerMock.Object);

            Assert.True(result);
        }

        [Fact]
        public void CheckNodePermissionConfig_HaventConfig_Deny()
        {
            var _loggerMock = new Mock<ILogger>();
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageCache(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock
                .Setup(x => x.CheckPermissionNodeManageTemplate(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var testNodeQueryBase = new TestNodeQueryBase();
            testNodeQueryBase.FilterByPermissionNodeCache = false;
            testNodeQueryBase.FilterByPermissionNodeConfig = true;
            testNodeQueryBase.FilterByPermissionNodeTemplate = false;
            testNodeQueryBase.FilterByPermissionNodeView = false;
            testNodeQueryBase.FilterIsInAnd = true;
            var result = NodesHandlerUtility.CheckPermissionNode(testNodeQueryBase, _requestContextMock.Object.NodeId,
                _filterNodeMock.Object, _loggerMock.Object);

            Assert.False(result);
        }

        [Fact]
        public void CheckNodePermissionCache_HaveCache_Allow()
        {
            var _loggerMock = new Mock<ILogger>();
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageCache(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock
                .Setup(x => x.CheckPermissionNodeManageTemplate(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);

            var testNodeQueryBase = new TestNodeQueryBase();
            testNodeQueryBase.FilterByPermissionNodeCache = true;
            testNodeQueryBase.FilterByPermissionNodeConfig = false;
            testNodeQueryBase.FilterByPermissionNodeTemplate = false;
            testNodeQueryBase.FilterByPermissionNodeView = false;
            testNodeQueryBase.FilterIsInAnd = true;
            var result = NodesHandlerUtility.CheckPermissionNode(testNodeQueryBase, _requestContextMock.Object.NodeId,
                _filterNodeMock.Object, _loggerMock.Object);

            Assert.True(result);
        }

        [Fact]
        public void CheckNodePermissionCache_HaventCache_Deny()
        {
            var _loggerMock = new Mock<ILogger>();
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageCache(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock
                .Setup(x => x.CheckPermissionNodeManageTemplate(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var testNodeQueryBase = new TestNodeQueryBase();
            testNodeQueryBase.FilterByPermissionNodeCache = true;
            testNodeQueryBase.FilterByPermissionNodeConfig = false;
            testNodeQueryBase.FilterByPermissionNodeTemplate = false;
            testNodeQueryBase.FilterByPermissionNodeView = false;
            testNodeQueryBase.FilterIsInAnd = true;
            var result = NodesHandlerUtility.CheckPermissionNode(testNodeQueryBase, _requestContextMock.Object.NodeId,
                _filterNodeMock.Object, _loggerMock.Object);

            Assert.False(result);
        }

        [Fact]
        public void CheckNodePermissionTemplate_HaveTemplate_Allow()
        {
            var _loggerMock = new Mock<ILogger>();
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageCache(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock
                .Setup(x => x.CheckPermissionNodeManageTemplate(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);

            var testNodeQueryBase = new TestNodeQueryBase();
            testNodeQueryBase.FilterByPermissionNodeCache = false;
            testNodeQueryBase.FilterByPermissionNodeConfig = false;
            testNodeQueryBase.FilterByPermissionNodeTemplate = true;
            testNodeQueryBase.FilterByPermissionNodeView = false;
            testNodeQueryBase.FilterIsInAnd = true;
            var result = NodesHandlerUtility.CheckPermissionNode(testNodeQueryBase, _requestContextMock.Object.NodeId,
                _filterNodeMock.Object, _loggerMock.Object);

            Assert.True(result);
        }

        [Fact]
        public void CheckNodePermissionTemplate_HaventTemplate_Deny()
        {
            var _loggerMock = new Mock<ILogger>();
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageCache(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock
                .Setup(x => x.CheckPermissionNodeManageTemplate(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var testNodeQueryBase = new TestNodeQueryBase();
            testNodeQueryBase.FilterByPermissionNodeCache = false;
            testNodeQueryBase.FilterByPermissionNodeConfig = false;
            testNodeQueryBase.FilterByPermissionNodeTemplate = true;
            testNodeQueryBase.FilterByPermissionNodeView = false;
            testNodeQueryBase.FilterIsInAnd = true;
            var result = NodesHandlerUtility.CheckPermissionNode(testNodeQueryBase, _requestContextMock.Object.NodeId,
                _filterNodeMock.Object, _loggerMock.Object);

            Assert.False(result);
        }

        [Fact]
        public void CheckNodePermissionView_HaveView_Allow()
        {
            var _loggerMock = new Mock<ILogger>();
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageCache(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock
                .Setup(x => x.CheckPermissionNodeManageTemplate(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var testNodeQueryBase = new TestNodeQueryBase();
            testNodeQueryBase.FilterByPermissionNodeCache = false;
            testNodeQueryBase.FilterByPermissionNodeConfig = false;
            testNodeQueryBase.FilterByPermissionNodeTemplate = false;
            testNodeQueryBase.FilterByPermissionNodeView = true;
            testNodeQueryBase.FilterIsInAnd = true;
            var result = NodesHandlerUtility.CheckPermissionNode(testNodeQueryBase, _requestContextMock.Object.NodeId,
                _filterNodeMock.Object, _loggerMock.Object);

            Assert.True(result);
        }

        [Fact]
        public void CheckNodePermissionView_HaventView_Deny()
        {
            var _loggerMock = new Mock<ILogger>();
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageCache(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock
                .Setup(x => x.CheckPermissionNodeManageTemplate(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(true);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);

            var testNodeQueryBase = new TestNodeQueryBase();
            testNodeQueryBase.FilterByPermissionNodeCache = false;
            testNodeQueryBase.FilterByPermissionNodeConfig = false;
            testNodeQueryBase.FilterByPermissionNodeTemplate = false;
            testNodeQueryBase.FilterByPermissionNodeView = true;
            testNodeQueryBase.FilterIsInAnd = true;
            var result = NodesHandlerUtility.CheckPermissionNode(testNodeQueryBase, _requestContextMock.Object.NodeId,
                _filterNodeMock.Object, _loggerMock.Object);

            Assert.False(result);
        }

        [Fact]
        public void CheckFalseAll_HaventPermissionWithAnd_Allow()
        {
            var _loggerMock = new Mock<ILogger>();
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageCache(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock
                .Setup(x => x.CheckPermissionNodeManageTemplate(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);

            var testNodeQueryBase = new TestNodeQueryBase();
            testNodeQueryBase.FilterByPermissionNodeCache = false;
            testNodeQueryBase.FilterByPermissionNodeConfig = false;
            testNodeQueryBase.FilterByPermissionNodeTemplate = false;
            testNodeQueryBase.FilterByPermissionNodeView = false;
            testNodeQueryBase.FilterIsInAnd = true;
            var result = NodesHandlerUtility.CheckPermissionNode(testNodeQueryBase, _requestContextMock.Object.NodeId,
                _filterNodeMock.Object, _loggerMock.Object);

            Assert.True(result);
        }

        [Fact]
        public void CheckFalseAll_HaventPermissionWithOr_Allow()
        {
            var _loggerMock = new Mock<ILogger>();
            var _filterNodeMock = new Mock<IFilterNode>();
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageCache(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageConfig(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock
                .Setup(x => x.CheckPermissionNodeManageTemplate(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);
            _filterNodeMock.Setup(x => x.CheckPermissionNodeManageView(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
                .Returns(false);

            var testNodeQueryBase = new TestNodeQueryBase();
            testNodeQueryBase.FilterByPermissionNodeCache = false;
            testNodeQueryBase.FilterByPermissionNodeConfig = false;
            testNodeQueryBase.FilterByPermissionNodeTemplate = false;
            testNodeQueryBase.FilterByPermissionNodeView = false;
            testNodeQueryBase.FilterIsInAnd = false;
            var result = NodesHandlerUtility.CheckPermissionNode(testNodeQueryBase, _requestContextMock.Object.NodeId,
                _filterNodeMock.Object, _loggerMock.Object);

            Assert.True(result);
        }


        internal class TestNodeQueryBase : NodeQueryBase
        {
        }
    }
}