using System.Collections.Generic;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using DataBrowser.UnitTests.HelperTest;
using EndPointConnector.Models;
using WSHUB.Models.Response;
using Xunit;

namespace DataBrowser.UnitTests.ResponseModelView
{
    public class DatasetCriteriaViewModelTest
    {
        [Fact]
        public void ConvertStructure_FromDto_Ok()
        {
            var criteria = Utility.GenerateCriteria();
            var criteriaMode = "TEST";

            var modelView = DatasetCriteriaViewModel.ConvertFromStructureDto(new StructureCriteriaForDataflowResponse
                {Criterias = criteria, CriteriaMode = criteriaMode});

            Assert.NotNull(modelView);
            Assert.Equal("TEST", modelView.CriteriaView);
            Assert.Equal(3, modelView.Criteria.Count);

            Assert.Equal("Id1", modelView.Criteria[0].Id);
            Assert.Null(modelView.Criteria[0].Values);
            Assert.Equal("IdTwo", modelView.Criteria[1].Id);
            Assert.Null(modelView.Criteria[1].Values);
            Assert.Equal("IdOther", modelView.Criteria[2].Id);
            Assert.Null(modelView.Criteria[2].Values);
        }

        [Fact]
        public void ConvertStructure_FromDto_DefaultLang_Ok()
        {
            var criteria = Utility.GenerateCriteria();
            var criteriaMode = "TEST";

            var modelView = DatasetCriteriaViewModel.ConvertFromStructureDto(new StructureCriteriaForDataflowResponse
                {Criterias = criteria, CriteriaMode = criteriaMode});

            Assert.Equal("Titolo IT", modelView.Criteria[0].Label);
            Assert.Equal("title IdTwo FR", modelView.Criteria[1].Label);
            Assert.Equal("title IdOther EN", modelView.Criteria[2].Label);
        }

        [Fact]
        public void ConvertStructure_FromDto_ITLang_Ok()
        {
            var criteria = Utility.GenerateCriteria();
            var criteriaMode = "TEST";

            var modelView = DatasetCriteriaViewModel.ConvertFromStructureDto(
                new StructureCriteriaForDataflowResponse {Criterias = criteria, CriteriaMode = criteriaMode}, "IT");

            Assert.Equal("Titolo IT", modelView.Criteria[0].Label);
            Assert.Equal("title IdTwo FR", modelView.Criteria[1].Label);
            Assert.Equal("title IdOther EN", modelView.Criteria[2].Label);
        }

        [Fact]
        public void ConvertStructure_FromDto_ENLang_Ok()
        {
            var criteria = Utility.GenerateCriteria();
            var criteriaMode = "TEST";

            var modelView = DatasetCriteriaViewModel.ConvertFromStructureDto(
                new StructureCriteriaForDataflowResponse {Criterias = criteria, CriteriaMode = criteriaMode}, "EN");

            Assert.Equal("title EN", modelView.Criteria[0].Label);
            Assert.Equal("title IdTwo FR", modelView.Criteria[1].Label);
            Assert.Equal("title IdOther EN", modelView.Criteria[2].Label);
        }

        [Fact]
        public void Convertdata_FromDto_Ok()
        {
            var criteria = Utility.GenerateCriteria();
            criteria.Add(new Criteria
            {
                Id = "Id4",
                Titles = new Dictionary<string, string> {{"IT", "Titolo4 IT"}, {"EN", "title4 EN"}}
            });

            var modelView = DatasetCriteriaViewModel.ConvertFromDataDto(new ArtefactContainer {Criterias = criteria});


            Assert.NotNull(modelView);
            Assert.Null(modelView.CriteriaView);
            Assert.Equal(4, modelView.Criteria.Count);

            Assert.Equal("Id1", modelView.Criteria[0].Id);
            Assert.NotNull(modelView.Criteria[0].Values);
            Assert.Equal(7, modelView.Criteria[0].Values.Count);
            Assert.Equal("A", modelView.Criteria[0].Values[0].Id);
            Assert.Null(modelView.Criteria[0].Values[0].ParentId);
            Assert.Equal("B", modelView.Criteria[0].Values[1].Id);
            Assert.Null(modelView.Criteria[0].Values[1].ParentId);
            Assert.Equal("C", modelView.Criteria[0].Values[2].Id);
            Assert.Equal("B", modelView.Criteria[0].Values[2].ParentId);
            Assert.Equal("D", modelView.Criteria[0].Values[3].Id);
            Assert.Equal("C", modelView.Criteria[0].Values[3].ParentId);
            Assert.Equal("E", modelView.Criteria[0].Values[4].Id);
            Assert.Equal("A", modelView.Criteria[0].Values[4].ParentId);
            Assert.Equal("F", modelView.Criteria[0].Values[5].Id);
            Assert.Null(modelView.Criteria[0].Values[5].ParentId);
            Assert.Equal("G", modelView.Criteria[0].Values[6].Id);
            Assert.Equal("C", modelView.Criteria[0].Values[6].ParentId);

            Assert.Equal("IdTwo", modelView.Criteria[1].Id);
            Assert.NotNull(modelView.Criteria[1].Values);
            Assert.Equal(7, modelView.Criteria[1].Values.Count);
            Assert.Equal("A2", modelView.Criteria[1].Values[0].Id);
            Assert.Null(modelView.Criteria[1].Values[0].ParentId);
            Assert.Equal("B2", modelView.Criteria[1].Values[1].Id);
            Assert.Null(modelView.Criteria[1].Values[1].ParentId);
            Assert.Equal("C2", modelView.Criteria[1].Values[2].Id);
            Assert.Equal("B2", modelView.Criteria[1].Values[2].ParentId);
            Assert.Equal("D2", modelView.Criteria[1].Values[3].Id);
            Assert.Equal("C2", modelView.Criteria[1].Values[3].ParentId);
            Assert.Equal("E2", modelView.Criteria[1].Values[4].Id);
            Assert.Equal("A2", modelView.Criteria[1].Values[4].ParentId);
            Assert.Equal("F2", modelView.Criteria[1].Values[5].Id);
            Assert.Null(modelView.Criteria[1].Values[5].ParentId);
            Assert.Equal("G2", modelView.Criteria[1].Values[6].Id);
            Assert.Equal("C2", modelView.Criteria[1].Values[6].ParentId);

            Assert.Equal("IdOther", modelView.Criteria[2].Id);
            Assert.NotNull(modelView.Criteria[2].Values);
            Assert.Equal(7, modelView.Criteria[2].Values.Count);
            Assert.Equal("A3", modelView.Criteria[2].Values[0].Id);
            Assert.Equal("B3", modelView.Criteria[2].Values[1].Id);
            Assert.Equal("C3", modelView.Criteria[2].Values[2].Id);
            Assert.Equal("D3", modelView.Criteria[2].Values[3].Id);
            Assert.Equal("E3", modelView.Criteria[2].Values[4].Id);
            Assert.Equal("F3", modelView.Criteria[2].Values[5].Id);
            Assert.Equal("G3", modelView.Criteria[2].Values[6].Id);

            Assert.Equal("Id4", modelView.Criteria[3].Id);
            Assert.Null(modelView.Criteria[3].Values);
        }

        [Fact]
        public void Convertdata_FromDto_DefaultLang_Ok()
        {
            var criteria = Utility.GenerateCriteria();
            criteria.Add(new Criteria
            {
                Id = "Id4",
                Titles = new Dictionary<string, string> {{"IT", "Titolo4 IT"}, {"EN", "title4 EN"}}
            });

            var modelView = DatasetCriteriaViewModel.ConvertFromDataDto(new ArtefactContainer {Criterias = criteria});


            Assert.Equal("Titolo IT", modelView.Criteria[0].Label);
            Assert.Equal("A IT", modelView.Criteria[0].Values[0].Name);
            Assert.Equal("B EN", modelView.Criteria[0].Values[1].Name);
            Assert.Equal("D EN", modelView.Criteria[0].Values[3].Name);
            Assert.Equal("E IT", modelView.Criteria[0].Values[4].Name);
            Assert.Equal("F EN", modelView.Criteria[0].Values[5].Name);
            Assert.Equal("title IdTwo FR", modelView.Criteria[1].Label);
            Assert.Equal("A2 FR", modelView.Criteria[1].Values[0].Name);
            Assert.Equal("B2 EN", modelView.Criteria[1].Values[1].Name);
            Assert.Equal("C2 EN", modelView.Criteria[1].Values[2].Name);
            Assert.Equal("F2 EN", modelView.Criteria[1].Values[5].Name);
            Assert.Equal("title IdOther EN", modelView.Criteria[2].Label);
            Assert.Equal("A3 FR", modelView.Criteria[2].Values[0].Name);
            Assert.Equal("B3 EN", modelView.Criteria[2].Values[1].Name);
            Assert.Equal("C3 EN", modelView.Criteria[2].Values[2].Name);
            Assert.Equal("Titolo4 IT", modelView.Criteria[3].Label);
        }

        [Fact]
        public void Convertdata_FromDto_ITLang_Ok()
        {
            var criteria = Utility.GenerateCriteria();
            criteria.Add(new Criteria
            {
                Id = "Id4",
                Titles = new Dictionary<string, string> {{"IT", "Titolo4 IT"}, {"EN", "title4 EN"}}
            });

            var modelView =
                DatasetCriteriaViewModel.ConvertFromDataDto(new ArtefactContainer {Criterias = criteria}, "IT");


            Assert.Equal("Titolo IT", modelView.Criteria[0].Label);
            Assert.Equal("A IT", modelView.Criteria[0].Values[0].Name);
            Assert.Equal("B EN", modelView.Criteria[0].Values[1].Name);
            Assert.Equal("D EN", modelView.Criteria[0].Values[3].Name);
            Assert.Equal("E IT", modelView.Criteria[0].Values[4].Name);
            Assert.Equal("F IT", modelView.Criteria[0].Values[5].Name);
            Assert.Equal("title IdTwo FR", modelView.Criteria[1].Label);
            Assert.Equal("A2 FR", modelView.Criteria[1].Values[0].Name);
            Assert.Equal("B2 EN", modelView.Criteria[1].Values[1].Name);
            Assert.Equal("C2 IT", modelView.Criteria[1].Values[2].Name);
            Assert.Equal("F2 IT", modelView.Criteria[1].Values[5].Name);
            Assert.Equal("title IdOther EN", modelView.Criteria[2].Label);
            Assert.Equal("A3 FR", modelView.Criteria[2].Values[0].Name);
            Assert.Equal("B3 EN", modelView.Criteria[2].Values[1].Name);
            Assert.Equal("C3 IT", modelView.Criteria[2].Values[2].Name);
            Assert.Equal("Titolo4 IT", modelView.Criteria[3].Label);
        }

        [Fact]
        public void Convertdata_FromDto_ENLang_Ok()
        {
            var criteria = Utility.GenerateCriteria();
            criteria.Add(new Criteria
            {
                Id = "Id4",
                Titles = new Dictionary<string, string> {{"IT", "Titolo4 IT"}, {"EN", "title4 EN"}}
            });

            var modelView =
                DatasetCriteriaViewModel.ConvertFromDataDto(new ArtefactContainer {Criterias = criteria}, "EN");


            Assert.Equal("title EN", modelView.Criteria[0].Label);
            Assert.Equal("A EN", modelView.Criteria[0].Values[0].Name);
            Assert.Equal("B EN", modelView.Criteria[0].Values[1].Name);
            Assert.Equal("D EN", modelView.Criteria[0].Values[3].Name);
            Assert.Equal("E IT", modelView.Criteria[0].Values[4].Name);
            Assert.Equal("F EN", modelView.Criteria[0].Values[5].Name);
            Assert.Equal("title IdTwo FR", modelView.Criteria[1].Label);
            Assert.Equal("A2 EN", modelView.Criteria[1].Values[0].Name);
            Assert.Equal("B2 EN", modelView.Criteria[1].Values[1].Name);
            Assert.Equal("D2 DE", modelView.Criteria[1].Values[3].Name);
            Assert.Equal("F2 EN", modelView.Criteria[1].Values[5].Name);
            Assert.Equal("title IdOther EN", modelView.Criteria[2].Label);
            Assert.Equal("A3 EN", modelView.Criteria[2].Values[0].Name);
            Assert.Equal("B3 EN", modelView.Criteria[2].Values[1].Name);
            Assert.Equal("C3 EN", modelView.Criteria[2].Values[2].Name);
            Assert.Equal("title4 EN", modelView.Criteria[3].Label);
        }
    }
}