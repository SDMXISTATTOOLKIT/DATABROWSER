using EndPointConnector.Interfaces.Sdmx.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Sister.EndPointConnector.Sdmx.Nsi.Rest.Get;
using Xunit;

namespace Sister.EndPointConnector.Sdmx.UnitTests
{
    public class NsiGetArtefactRestTest
    {
        #region Codelist

        [Fact]
        public void GetSingleArtefact_Codelist_Ok()
        {
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.CodeList, "IdCod", "AgCod", "1.0");

            Assert.Equal("codelist/AgCod/IdCod/1.0/?detail=Full&references=None", result);
        }

        [Fact]
        public void GetSingleArtefact_CodelistAll_Ok()
        {
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.CodeList, "IdCod", "AgCod", "1.0",
                StructureReferenceDetailEnumType.All,
                "");

            Assert.Equal("codelist/AgCod/IdCod/1.0/?detail=Full&references=All", result);
        }

        [Fact]
        public void GetSingleArtefact_CodelistWithChildren_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.CodeList, "IdCod", "AgCod", "1.0",
                StructureReferenceDetailEnumType.Children,
                "");

            Assert.Equal("codelist/AgCod/IdCod/1.0/?detail=Full&references=Children", result);
        }

        [Fact]
        public void GetSingleArtefact_CodelistWithParents_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.CodeList, "IdCod", "AgCod", "1.0",
                StructureReferenceDetailEnumType.Parents,
                "");

            Assert.Equal("codelist/AgCod/IdCod/1.0/?detail=Full&references=Parents", result);
        }

        [Fact]
        public void GetSingleArtefact_CodelistStubWithSupportCompleteStubs_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);


            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.CodeList, "IdCod", "AgCod", "1.0",
                respDetail: "STUB");

            Assert.Equal("codelist/AgCod/IdCod/1.0/?detail=allcompletestubs&references=None", result);
        }

        [Fact]
        public void GetSingleArtefact_CodelistStubNoSupportCompleteStubs_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = false
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            //allstubs
            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.CodeList, "IdCod", "AgCod", "1.0",
                respDetail: "STUB");

            Assert.Equal("codelist/AgCod/IdCod/1.0/?detail=allstubs&references=None", result);
        }

        [Fact]
        public void GetArtefacts_Codelist_Ok()
        {
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.CodeList, null, null, null);

            Assert.Equal("codelist/all/all/all/?detail=allcompletestubs&references=None", result);

            endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = false
            };
            nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.CodeList, null, null, null);

            Assert.Equal("codelist/all/all/all/?detail=allstubs&references=None", result);
        }

        [Fact]
        public void GetArtefacts_CodelistAll_Ok()
        {
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.CodeList, null, null, null,
                StructureReferenceDetailEnumType.All,
                "");

            Assert.Equal("codelist/all/all/all/?detail=Full&references=All", result);
        }

        [Fact]
        public void GetArtefacts_CodelistWithChildren_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.CodeList, null, null, null,
                StructureReferenceDetailEnumType.Children,
                "");

            Assert.Equal("codelist/all/all/all/?detail=Full&references=Children", result);
        }

        [Fact]
        public void GetArtefacts_CodelistWithParents_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.CodeList, null, null, null,
                StructureReferenceDetailEnumType.Parents,
                "");

            Assert.Equal("codelist/all/all/all/?detail=Full&references=Parents", result);
        }

        [Fact]
        public void GetArtefacts_CodelistStubWithSupportCompleteStubs_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);


            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.CodeList, null, null, null,
                respDetail: "STUB");

            Assert.Equal("codelist/all/all/all/?detail=allcompletestubs&references=None", result);
        }

        [Fact]
        public void GetArtefacts_CodelistStubNoSupportCompleteStubs_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = false
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            //allstubs
            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.CodeList, null, null, null,
                respDetail: "STUB");

            Assert.Equal("codelist/all/all/all/?detail=allstubs&references=None", result);
        }

        #endregion

        #region Dsd

        [Fact]
        public void GetSingleArtefact_Dsd_Ok()
        {
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dsd, "IdCod", "AgCod", "1.0");

            Assert.Equal("datastructure/AgCod/IdCod/1.0/?detail=Full&references=None", result);
        }

        [Fact]
        public void GetSingleArtefact_DsdAll_Ok()
        {
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dsd, "IdCod", "AgCod", "1.0",
                StructureReferenceDetailEnumType.All,
                "");

            Assert.Equal("datastructure/AgCod/IdCod/1.0/?detail=Full&references=All", result);
        }

        [Fact]
        public void GetSingleArtefact_DsdWithChildren_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dsd, "IdCod", "AgCod", "1.0",
                StructureReferenceDetailEnumType.Children,
                "");

            Assert.Equal("datastructure/AgCod/IdCod/1.0/?detail=Full&references=Children", result);
        }

        [Fact]
        public void GetSingleArtefact_DsdWithParents_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dsd, "IdCod", "AgCod", "1.0",
                StructureReferenceDetailEnumType.Parents,
                "");

            Assert.Equal("datastructure/AgCod/IdCod/1.0/?detail=Full&references=Parents", result);
        }

        [Fact]
        public void GetSingleArtefact_DsdStubWithSupportCompleteStubs_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);


            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dsd, "IdCod", "AgCod", "1.0",
                respDetail: "STUB");

            Assert.Equal("datastructure/AgCod/IdCod/1.0/?detail=allcompletestubs&references=None", result);
        }

        [Fact]
        public void GetSingleArtefact_DsdStubNoSupportCompleteStubs_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = false
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            //allstubs
            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dsd, "IdCod", "AgCod", "1.0",
                respDetail: "STUB");

            Assert.Equal("datastructure/AgCod/IdCod/1.0/?detail=allstubs&references=None", result);
        }

        [Fact]
        public void GetArtefacts_Dsd_Ok()
        {
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dsd, null, null, null);

            Assert.Equal("datastructure/all/all/all/?detail=Full&references=None", result);

            endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = false
            };
            nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dsd, null, null, null);

            Assert.Equal("datastructure/all/all/all/?detail=Full&references=None", result);
        }

        [Fact]
        public void GetArtefacts_DsdAll_Ok()
        {
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dsd, null, null, null,
                StructureReferenceDetailEnumType.All,
                "");

            Assert.Equal("datastructure/all/all/all/?detail=Full&references=All", result);
        }

        [Fact]
        public void GetArtefacts_DsdWithChildren_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dsd, null, null, null,
                StructureReferenceDetailEnumType.Children,
                "");

            Assert.Equal("datastructure/all/all/all/?detail=Full&references=Children", result);
        }

        [Fact]
        public void GetArtefacts_DsdWithParents_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dsd, null, null, null,
                StructureReferenceDetailEnumType.Parents,
                "");

            Assert.Equal("datastructure/all/all/all/?detail=Full&references=Parents", result);
        }

        [Fact]
        public void GetArtefacts_DsdStubWithSupportCompleteStubs_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);


            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dsd, null, null, null,
                respDetail: "STUB");

            Assert.Equal("datastructure/all/all/all/?detail=allcompletestubs&references=None", result);
        }

        [Fact]
        public void GetArtefacts_DsdStubNoSupportCompleteStubs_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = false
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            //allstubs
            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dsd, null, null, null,
                respDetail: "STUB");

            Assert.Equal("datastructure/all/all/all/?detail=allstubs&references=None", result);
        }

        #endregion

        #region Dataflow

        [Fact]
        public void GetSingleArtefact_Dataflow_Ok()
        {
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dataflow, "IdCod", "AgCod", "1.0");

            Assert.Equal("dataflow/AgCod/IdCod/1.0/?detail=Full&references=None", result);
        }

        [Fact]
        public void GetSingleArtefact_DataflowAll_Ok()
        {
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dataflow, "IdCod", "AgCod", "1.0",
                StructureReferenceDetailEnumType.All,
                "");

            Assert.Equal("dataflow/AgCod/IdCod/1.0/?detail=Full&references=All", result);
        }

        [Fact]
        public void GetSingleArtefact_DataflowWithChildren_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dataflow, "IdCod", "AgCod", "1.0",
                StructureReferenceDetailEnumType.Children,
                "");

            Assert.Equal("dataflow/AgCod/IdCod/1.0/?detail=Full&references=Children", result);
        }

        [Fact]
        public void GetSingleArtefact_DataflowWithParents_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dataflow, "IdCod", "AgCod", "1.0",
                StructureReferenceDetailEnumType.Parents,
                "");

            Assert.Equal("dataflow/AgCod/IdCod/1.0/?detail=Full&references=Parents", result);
        }

        [Fact]
        public void GetSingleArtefact_DataflowStubWithSupportCompleteStubs_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);


            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dataflow, "IdCod", "AgCod", "1.0",
                respDetail: "STUB");

            Assert.Equal("dataflow/AgCod/IdCod/1.0/?detail=allcompletestubs&references=None", result);
        }

        [Fact]
        public void GetSingleArtefact_DataflowStubNoSupportCompleteStubs_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = false
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            //allstubs
            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dataflow, "IdCod", "AgCod", "1.0",
                respDetail: "STUB");

            Assert.Equal("dataflow/AgCod/IdCod/1.0/?detail=allstubs&references=None", result);
        }

        [Fact]
        public void GetArtefacts_Dataflow_Ok()
        {
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dataflow, null, null, null);

            Assert.Equal("dataflow/all/all/all/?detail=Full&references=None", result);

            endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = false
            };
            nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dataflow, null, null, null);

            Assert.Equal("dataflow/all/all/all/?detail=Full&references=None", result);
        }

        [Fact]
        public void GetArtefacts_DataflowAll_Ok()
        {
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dataflow, null, null, null,
                StructureReferenceDetailEnumType.All,
                "");

            Assert.Equal("dataflow/all/all/all/?detail=Full&references=All", result);
        }

        [Fact]
        public void GetArtefacts_DataflowWithChildren_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dataflow, null, null, null,
                StructureReferenceDetailEnumType.Children,
                "");

            Assert.Equal("dataflow/all/all/all/?detail=Full&references=Children", result);
        }

        [Fact]
        public void GetArtefacts_DataflowWithParents_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dataflow, null, null, null,
                StructureReferenceDetailEnumType.Parents,
                "");

            Assert.Equal("dataflow/all/all/all/?detail=Full&references=Parents", result);
        }

        [Fact]
        public void GetArtefacts_DataflowStubWithSupportCompleteStubs_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = true
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);


            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dataflow, null, null, null,
                respDetail: "STUB");

            Assert.Equal("dataflow/all/all/all/?detail=allcompletestubs&references=None", result);
        }

        [Fact]
        public void GetArtefacts_DataflowStubNoSupportCompleteStubs_Ok()
        {
            var mockLogger = Mock.Of<ILoggerFactory>();
            var endPointSdmxConfig = new EndPointSdmxConfig
            {
                SupportAllCompleteStubs = false
            };
            var nsiGetArtefactRest = new NsiGetArtefactRest(NullLoggerFactory.Instance, endPointSdmxConfig);

            //allstubs
            var result = nsiGetArtefactRest.GetArtefact(SdmxStructureEnumType.Dataflow, null, null, null,
                respDetail: "STUB");

            Assert.Equal("dataflow/all/all/all/?detail=allstubs&references=None", result);
        }

        #endregion
    }
}