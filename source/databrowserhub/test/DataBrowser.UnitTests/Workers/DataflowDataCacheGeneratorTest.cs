using DataBrowser.AC.Workers;
using DataBrowser.Interfaces;
using Moq;

namespace DataBrowser.UnitTests.Workers
{
    public class DataflowDataCacheGeneratorTest : DataflowDataCacheGenerator
    {
        private readonly Mock<IRequestContext> _requestContextMock;

        public DataflowDataCacheGeneratorTest()
            : base(null, null, null, null, null, null)
        {
            _requestContextMock = new Mock<IRequestContext>();
            _requestContextMock.Setup(x => x.NodeId).Returns(1);
            _requestContextMock.Setup(x => x.NodeCode).Returns("TestCode");
            _requestContextMock.Setup(x => x.IgnoreCache).Returns(true);
            _requestContextMock.Setup(x => x.UserLang).Returns("IT");
            _requestContextMock.Setup(x => x.LoggedUserId).Returns(-1);
        }

        //[Fact]
        //public void GroupCacheByNumberItemCode()
        //{
        //    //groupCacheByNumberItemCode();
        //}
    }
}