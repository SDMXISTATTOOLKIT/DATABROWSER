using System.Collections.Generic;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.Hubs;
using DataBrowser.UnitTests.HelperTest;
using Xunit;

namespace DataBrowser.UnitTests.Entity
{
    public class HubTest
    {
        [Fact]
        public void CreateVariusNode_WithCorrectData_Ok()
        {
            var hubDto = EntityHubData.CreateStandardHubDto();
            var hub = EntityHubData.CreateStandardHub();

            EntityHubData.CheckHubEntityFromHubDto(hubDto, hub);


            hubDto.LogoURL = "LogoURL2";
            hubDto.SupportedLanguages = new List<string> {"IT", "DE", "FR"};
            hubDto.DefaultLanguage = "DefaultLanguage2";
            hubDto.MaxObservationsAfterCriteria = 4999;
            hubDto.DecimalSeparator = ".";
            hubDto.DecimalNumber = 0;
            hubDto.MaxCells = 3000;
            hubDto.EmptyCellDefaultValue = "NULL";
            hubDto.DefaultView = "newView";
            hubDto.Title = new Dictionary<string, string> {{"IT", "Titolo"}, {"EN", "Title"}, {"FR", "TitleFR"}};
            hubDto.Slogan = new Dictionary<string, string> {{"FR", "SLOGANFR"}};
            hubDto.Description = null;
            hubDto.BackgroundMediaURL = null;
            hubDto.Extras = "{\"pageSize\":20, \"pageNumber\": 10}";

            hub = Hub.CreateHub(hubDto);
            EntityHubData.CheckHubEntityFromHubDto(hubDto, hub);
        }

        [Fact]
        public void EditVariusNode_WithCorrectData_Ok()
        {
            var hub = EntityHubData.CreateStandardHub();

            var hubDto = new HubDto();
            hubDto.LogoURL = "LogoURL2";
            hubDto.SupportedLanguages = new List<string> {"IT", "DE", "FR"};
            hubDto.DefaultLanguage = "DefaultLanguage2";
            hubDto.MaxObservationsAfterCriteria = 4999;
            hubDto.DecimalSeparator = ".";
            hubDto.DecimalNumber = 0;
            hubDto.MaxCells = 1000;
            hubDto.EmptyCellDefaultValue = "NULL";
            hubDto.DefaultView = "newView";
            hubDto.Title = new Dictionary<string, string> {{"IT", "Titolo"}, {"EN", "Title"}, {"FR", "TitleFR"}};
            hubDto.Slogan = new Dictionary<string, string> {{"FR", "SLOGANFR"}};
            hubDto.Description = null;
            hubDto.BackgroundMediaURL = null;
            hubDto.Extras = "{\"pageSize2\":20, \"pageNumber2\": 10}";

            hub.EditHub(hubDto);
            EntityHubData.CheckHubEntityFromHubDto(hubDto, hub);


            hubDto = new HubDto();
            hubDto.LogoURL = "LogoURL2";
            hubDto.SupportedLanguages = new List<string> {"IT", "FR"};
            hubDto.DefaultLanguage = "DefaultLanguage3";
            hubDto.MaxObservationsAfterCriteria = 9999;
            hubDto.DecimalSeparator = "A";
            hubDto.DecimalNumber = 10;
            hubDto.MaxCells = 9000;
            hubDto.EmptyCellDefaultValue = "TEST";
            hubDto.DefaultView = "newView2";
            hubDto.Title = new Dictionary<string, string> {{"IT", "Titolo"}, {"FR", "TitleFR"}};
            hubDto.Slogan = new Dictionary<string, string> {{"FR", "SLOGANFR"}, {"IT", "SLOGANIT"}};
            hubDto.Description = null;
            hubDto.BackgroundMediaURL = "test2";
            hubDto.Extras = null;

            hub.EditHub(hubDto);
            EntityHubData.CheckHubEntityFromHubDto(hubDto, hub);


            hubDto = new HubDto();
            hubDto.LogoURL = "LogoURL3";
            hubDto.SupportedLanguages = new List<string> {"IT", "DE", "EN"};
            hubDto.DefaultLanguage = "DefaultLanguage4";
            hubDto.MaxObservationsAfterCriteria = 9999;
            hubDto.DecimalSeparator = "B";
            hubDto.DecimalNumber = 5;
            hubDto.MaxCells = 8000;
            hubDto.EmptyCellDefaultValue = "TEST3";
            hubDto.DefaultView = "newView4";
            hubDto.Title = new Dictionary<string, string> {{"IT", "Titolo"}, {"EN", "Title"}, {"FR", "TitleFR"}};
            hubDto.Slogan = new Dictionary<string, string> {{"FR", "SLOGANFR"}};
            hubDto.Description = null;
            hubDto.BackgroundMediaURL = "test4";
            hubDto.Extras = "{\"pageSize3\":30, \"pageNumber2\": 33}";

            hub.EditHub(hubDto);
            EntityHubData.CheckHubEntityFromHubDto(hubDto, hub);
        }

        [Fact]
        public void EditTitle_Ok()
        {
            var hub = EntityHubData.CreateStandardHub();

            var titleNew = new Dictionary<string, string> {{"IT", "Titolo"}, {"EN", "Title"}, {"FR", "Title"}};
            hub.SetTitleTransaltion(titleNew);

            titleNew = new Dictionary<string, string> {{"IT", "Titolo"}, {"FR", "TitleFR"}, {"DE", "Title DE"}};
            hub.SetTitleTransaltion(titleNew);


            Assert.Equal(titleNew.Count, hub.Title.TransatableItemValues.Count);
            foreach (var item in hub.Title.TransatableItemValues)
            {
                Assert.True(titleNew.ContainsKey(item.Language));
                Assert.Equal(titleNew[item.Language], item.Value);
            }
        }

        [Fact]
        public void RemoveTitle_Ok()
        {
            var hub = EntityHubData.CreateStandardHub();

            hub.SetTitleTransaltion(null);
            Assert.Null(hub.Title);
        }

        [Fact]
        public void SetNewDescription_Ok()
        {
            var hub = EntityHubData.CreateStandardHub();

            var descriptionNew = new Dictionary<string, string> {{"EN", "ENdesc"}, {"IT", "ITdesc"}};
            hub.SetDescriptionTransaltion(descriptionNew);
            Assert.Equal(descriptionNew.Count, hub.Description.TransatableItemValues.Count);
            foreach (var item in hub.Description.TransatableItemValues)
            {
                Assert.True(descriptionNew.ContainsKey(item.Language));
                Assert.Equal(descriptionNew[item.Language], item.Value);
            }
        }

        [Fact]
        public void RemoveDescription_Ok()
        {
            var hub = EntityHubData.CreateStandardHub();

            hub.SetDescriptionTransaltion(null);
            Assert.Null(hub.Description);
        }

        [Fact]
        public void SetNewSlogan_Ok()
        {
            var hub = EntityHubData.CreateStandardHub();

            var sloganNew = new Dictionary<string, string> {{"FR", "SLOGANFR2"}, {"IT", "SloganIT2"}};
            hub.SetSloganTransaltion(sloganNew);
            Assert.Equal(sloganNew.Count, hub.Slogan.TransatableItemValues.Count);
            foreach (var item in hub.Slogan.TransatableItemValues)
            {
                Assert.True(sloganNew.ContainsKey(item.Language));
                Assert.Equal(sloganNew[item.Language], item.Value);
            }
        }

        [Fact]
        public void RemoveSlogan_Ok()
        {
            var hub = EntityHubData.CreateStandardHub();

            hub.SetSloganTransaltion(null);
            Assert.Null(hub.Slogan);
        }
    }
}