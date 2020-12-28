using System.Collections.Generic;
using System.Linq;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.Hubs;
using Xunit;

namespace DataBrowser.UnitTests.HelperTest
{
    public static class EntityHubData
    {
        public static HubDto CreateStandardHubDto()
        {
            var hubDto = new HubDto
            {
                LogoURL = "type",
                BackgroundMediaURL = "code",
                SupportedLanguages = new List<string> {"IT", "EN"},
                DefaultLanguage = "IT",
                MaxObservationsAfterCriteria = 500000,
                DecimalSeparator = ",",
                DecimalNumber = 2,
                MaxCells = 2000,
                EmptyCellDefaultValue = "NaN",
                DefaultView = "defView",
                Title = new Dictionary<string, string> {{"IT", "Titolo"}, {"EN", "Title"}},
                Slogan = new Dictionary<string, string> {{"FR", "SLOGANFR"}, {"IT", "SloganIT"}},
                Description = new Dictionary<string, string> {{"SP", "DESCSP"}}
            };
            return hubDto;
        }

        public static Hub CreateStandardHub()
        {
            return Hub.CreateHub(CreateStandardHubDto());
        }

        public static void CheckHubEntityFromHubDto(HubDto hubDto, Hub hub)
        {
            Assert.Equal(hubDto.LogoURL, hub.LogoURL);
            Assert.Equal(hubDto.BackgroundMediaURL, hub.BackgroundMediaURL);
            Assert.Equal(hubDto.DefaultLanguage, hub.DefaultLanguage);
            Assert.Equal(hubDto.MaxObservationsAfterCriteria, hub.MaxObservationsAfterCriteria);
            Assert.Equal(hubDto.DecimalSeparator, hub.DecimalSeparator);
            Assert.Equal(hubDto.DecimalNumber, hub.DecimalNumber);
            Assert.Equal(hubDto.EmptyCellDefaultValue, hub.EmptyCellDefaultValue);
            Assert.Equal(hubDto.DefaultView, hub.DefaultView);

            if (hubDto.SupportedLanguages == null || hubDto.SupportedLanguages.Count == 0)
            {
                Assert.Null(hub.Title);
            }
            else
            {
                var supportedLangEntity = hub.SupportedLanguages.Split(";");
                Assert.Equal(hubDto.SupportedLanguages.Count, supportedLangEntity.Length);
                foreach (var item in hubDto.SupportedLanguages) Assert.Contains(item, hubDto.SupportedLanguages);
            }


            if (hubDto.Title == null || hubDto.Title.Count == 0)
            {
                Assert.Null(hub.Title);
            }
            else
            {
                Assert.Equal(hubDto.Title.Count, hub.Title.TransatableItemValues.Count);
                foreach (var item in hub.Title.TransatableItemValues)
                {
                    Assert.True(hubDto.Title.ContainsKey(item.Language));
                    Assert.Equal(hubDto.Title[item.Language], item.Value);
                }
            }

            if (hubDto.Description == null || hubDto.Description.Count == 0)
            {
                Assert.Null(hub.Description);
            }
            else
            {
                Assert.Equal(hubDto.Description.Count, hub.Description.TransatableItemValues.Count);
                foreach (var item in hub.Description.TransatableItemValues)
                {
                    Assert.True(hubDto.Description.ContainsKey(item.Language));
                    Assert.Equal(hubDto.Description[item.Language], item.Value);
                }
            }

            if (hubDto.Slogan == null || hubDto.Slogan.Count == 0)
            {
                Assert.Null(hub.Slogan);
            }
            else
            {
                Assert.Equal(hubDto.Slogan.Count, hub.Slogan.TransatableItemValues.Count);
                foreach (var item in hub.Slogan.TransatableItemValues)
                {
                    Assert.True(hubDto.Slogan.ContainsKey(item.Language));
                    Assert.Equal(hubDto.Slogan[item.Language], item.Value);
                }
            }
        }

        public static void CheckNodeDtoFromNodeEntity(Hub hub, HubDto hubDto)
        {
            Assert.Equal(hub.LogoURL, hubDto.LogoURL);
            Assert.Equal(hub.BackgroundMediaURL, hubDto.BackgroundMediaURL);
            Assert.Equal(hub.DefaultLanguage, hubDto.DefaultLanguage);
            Assert.Equal(hub.MaxObservationsAfterCriteria, hubDto.MaxObservationsAfterCriteria);
            Assert.Equal(hub.DecimalSeparator, hubDto.DecimalSeparator);
            Assert.Equal(hub.DecimalNumber, hubDto.DecimalNumber);
            Assert.Equal(hub.EmptyCellDefaultValue, hubDto.EmptyCellDefaultValue);
            Assert.Equal(hub.DefaultView, hubDto.DefaultView);


            if (string.IsNullOrWhiteSpace(hub.SupportedLanguages))
            {
                Assert.Null(hubDto.Title);
            }
            else
            {
                var supportedLangEntity = hub.SupportedLanguages.Split(";");
                Assert.Equal(supportedLangEntity.Length, hubDto.SupportedLanguages.Count);
                foreach (var item in supportedLangEntity) Assert.Contains(item, hubDto.SupportedLanguages);
            }

            if (hub.Title == null)
            {
                Assert.Empty(hubDto.Title);
            }
            else
            {
                Assert.Equal(hub.Title.TransatableItemValues.Count, hubDto.Title.Count);
                foreach (var item in hubDto.Title)
                {
                    Assert.Contains(hub.Title.TransatableItemValues, i => i.Language.Equals(item.Key));
                    var entityTransalte = hub.Title.TransatableItemValues.First(i => i.Language.Equals(item.Key));
                    Assert.Equal(entityTransalte.Value, item.Value);
                }
            }

            if (hub.Description == null)
            {
                Assert.Empty(hubDto.Description);
            }
            else
            {
                Assert.Equal(hub.Description.TransatableItemValues.Count, hubDto.Description.Count);
                foreach (var item in hubDto.Description)
                {
                    Assert.Contains(hub.Description.TransatableItemValues, i => i.Language.Equals(item.Key));
                    var entityTransalte = hub.Description.TransatableItemValues.First(i => i.Language.Equals(item.Key));
                    Assert.Equal(entityTransalte.Value, item.Value);
                }
            }

            if (hub.Slogan == null)
            {
                Assert.Empty(hubDto.Slogan);
            }
            else
            {
                Assert.Equal(hub.Slogan.TransatableItemValues.Count, hubDto.Slogan.Count);
                foreach (var item in hubDto.Slogan)
                {
                    Assert.Contains(hub.Slogan.TransatableItemValues, i => i.Language.Equals(item.Key));
                    var entityTransalte = hub.Slogan.TransatableItemValues.First(i => i.Language.Equals(item.Key));
                    Assert.Equal(entityTransalte.Value, item.Value);
                }
            }
        }
    }
}