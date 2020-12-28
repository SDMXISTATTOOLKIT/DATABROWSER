namespace DataBrowser.UnitTests.UseCases
{
    /*
    public class GetConstraintForDataflowUseCaseTest
    {
        [Fact]
        public async Task HandleTest()
        {
            var logger = Mock.Of<ILogger<GetCriteriaForDataflowUseCase>>();

            var dataflow = new Dataflow
            {
                Id = "DFid+DFAgency+DFVersion",
                DataStructureRef = new ArtefactRef { RefType = ArtefactType.ArtefactEnumType.Dsd, Id = "DSDAgency+DSDId+DSDVersion" }
            };
            var dsd = new Dsd
            {
                Id = "DSDId+DSDVersion+DSDAgency",
            };
            var dimensions = new List<Dimension> {
                                new Dimension { Id = "dim1", Representation = new ArtefactRef { Id = "CodAgency1+CodId1+CodVersion1", RefType = ArtefactType.ArtefactEnumType.CodeList } },
                                new Dimension { Id = "dim2", Representation = new ArtefactRef { Id = "CodAgency2+CodId2+CodVersion2", RefType = ArtefactType.ArtefactEnumType.CodeList } },
                                new Dimension { Id = "dim3", Representation = new ArtefactRef { Id = "CodAgency3+CodId3+CodVersion3", RefType = ArtefactType.ArtefactEnumType.CodeList } },
                                new Dimension { Id = "dim4", Representation = new ArtefactRef { Id = "CodAgency2+CodId2+CodVersion2", RefType = ArtefactType.ArtefactEnumType.CodeList } }
            };
            dsd.Dimensions = dimensions;
            dsd.PrimaryMeasure = null;

            //Dataflow
            var artefactContainer = new ArtefactContainer
            {
                Dataflows = new List<Dataflow> { dataflow },
                Dsds = new List<Dsd> { dsd }
            };
            var endPointConnectorMock = new Mock<IEndPointConnector>();
            endPointConnectorMock.Setup(s => s.GetArtefactAsync(ArtefactType.ArtefactEnumType.Dataflow, It.IsAny<string>(),
                                                                It.IsAny<ArtefactType.ReferenceDetailEnumType>(), It.IsAny<ArtefactType.ResponseDetailEnumType>(),
                                                                It.IsAny<bool>(), It.IsAny<bool>()))
                                .Returns(Task.FromResult(artefactContainer));

            //Codelist
            var codelist = new Codelist
            {
                Id = "CodAgency1+CodId1+CodVersion1",
                Items = new List<Code> { new Code { Id = "IdCode1", Names = new Dictionary<string, string> { { "IT", "desc it" } } } }
            };
            var code1ArtefactContainer = new ArtefactContainer
            {
                Codelists = new List<Codelist> { codelist }
            };
            var codelist2 = new Codelist
            {
                Id = "CodAgency2+CodId2+CodVersion2",
                Items = new List<Code> { new Code { Id = "IdCodeA", Names = new Dictionary<string, string> { { "IT", "desc it" } } }, new Code { Id = "IdCodeB", Names = new Dictionary<string, string> { { "IT", "desc en" } } } }
            };

            var code2ArtefactContainer = new ArtefactContainer
            {
                Codelists = new List<Codelist> { codelist2 }
            };
            var codelist3 = new Codelist
            {
                Id = "CodAgency3+CodId3+CodVersion3",
                Items = new List<Code> { new Code { Id = "IdCode3", Names = new Dictionary<string, string> { { "IT", "desc it 3" } } } }
            };
            var code3ArtefactContainer = new ArtefactContainer
            {
                Codelists = new List<Codelist> { codelist3 }
            };
            endPointConnectorMock.Setup(s => s.GetArtefactAsync(ArtefactType.ArtefactEnumType.CodeList, "CodAgency1+CodId1+CodVersion1",
                                                                It.IsAny<ArtefactType.ReferenceDetailEnumType>(), It.IsAny<ArtefactType.ResponseDetailEnumType>(),
                                                                It.IsAny<bool>(), It.IsAny<bool>()))
                                .Returns(Task.FromResult(code1ArtefactContainer));
            endPointConnectorMock.Setup(s => s.GetArtefactAsync(ArtefactType.ArtefactEnumType.CodeList, "CodAgency2+CodId2+CodVersion2",
                                                                It.IsAny<ArtefactType.ReferenceDetailEnumType>(), It.IsAny<ArtefactType.ResponseDetailEnumType>(),
                                                                It.IsAny<bool>(), It.IsAny<bool>()))
                                .Returns(Task.FromResult(code2ArtefactContainer));
            endPointConnectorMock.Setup(s => s.GetArtefactAsync(ArtefactType.ArtefactEnumType.CodeList, "CodAgency3+CodId3+CodVersion3",
                                                                It.IsAny<ArtefactType.ReferenceDetailEnumType>(), It.IsAny<ArtefactType.ResponseDetailEnumType>(),
                                                                It.IsAny<bool>(), It.IsAny<bool>()))
                                .Returns(Task.FromResult(code3ArtefactContainer));
            endPointConnectorMock.Setup(s => s.GetArtefactAsync(ArtefactType.ArtefactEnumType.CodeList, "CodAgency4+CodId4+CodVersion4",
                                                                It.IsAny<ArtefactType.ReferenceDetailEnumType>(), It.IsAny<ArtefactType.ResponseDetailEnumType>(),
                                                                It.IsAny<bool>(), It.IsAny<bool>()))
                                .Returns(Task.FromResult(code2ArtefactContainer));


            var mockGetArtefactSdmxService = new Mock<IEndPointConnectorFactory>();
            mockGetArtefactSdmxService.Setup(s => s.Create(It.IsAny<EndPointConfig>()))
                .Returns(Task.FromResult(endPointConnectorMock.Object));

            IRequestContext requestContext = Mock.Of<IRequestContext>();

            var nodeService = new Mock<INodeService>();
            var resultNodeService = new NodeDto();
            resultNodeService.CriteriaSelectionMode = "Dynamic";
            nodeService.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(resultNodeService));


            var caseTest = new GetCriteriaForDataflowUseCase(logger, mockGetArtefactSdmxService.Object, requestContext, nodeService.Object);
            var input = new CriteriaForDataflowRequest
            {
                DataflowId = dataflow.Id
            };
            var result = await caseTest.Handle(input, new System.Threading.CancellationToken());

            Assert.Equal(4, result.Criterias.Count);
            Assert.Contains("CodAgency1+CodId1+CodVersion1", result.Criterias.Select(i => i.DataStructureRef.Id).ToList());
            Assert.Contains("CodAgency2+CodId2+CodVersion2", result.Criterias.Select(i => i.DataStructureRef.Id).ToList());
            Assert.Contains("CodAgency3+CodId3+CodVersion3", result.Criterias.Select(i => i.DataStructureRef.Id).ToList());

            Assert.Equal(resultNodeService.CriteriaSelectionMode, result.CriteriaMode);

            Assert.Contains("dim1", result.Criterias.Select(i => i.Id).ToList());
            var codelistConstraints = result.Criterias.First(i => i.Id.Equals("dim1", StringComparison.InvariantCultureIgnoreCase)).DataStructureRef;
            Assert.Equal("CodAgency1+CodId1+CodVersion1", codelistConstraints.Id);

            Assert.Contains("dim2", result.Criterias.Select(i => i.Id).ToList());
            codelistConstraints = result.Criterias.First(i => i.Id.Equals("dim2", StringComparison.InvariantCultureIgnoreCase)).DataStructureRef;
            Assert.Equal("CodAgency2+CodId2+CodVersion2", codelistConstraints.Id);

            Assert.Contains("dim3", result.Criterias.Select(i => i.Id).ToList());
            codelistConstraints = result.Criterias.First(i => i.Id.Equals("dim3", StringComparison.InvariantCultureIgnoreCase)).DataStructureRef;
            Assert.Equal("CodAgency3+CodId3+CodVersion3", codelistConstraints.Id);

            Assert.Contains("dim4", result.Criterias.Select(i => i.Id).ToList());
            codelistConstraints = result.Criterias.First(i => i.Id.Equals("dim4", StringComparison.InvariantCultureIgnoreCase)).DataStructureRef;
            Assert.Equal("CodAgency2+CodId2+CodVersion2", codelistConstraints.Id);
        }
    }
    */
}