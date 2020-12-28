using System;
using System.Collections.Generic;
using System.Linq;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.UnitTests.HelperTest;
using Xunit;

namespace DataBrowser.UnitTests.Entity
{
    public class NodeTest
    {
        [Fact]
        public void CreateVariusNode_WithCorrectData_Ok()
        {
            var nodeDto = new NodeDto
            {
                Type = "type",
                Code = "code",
                EndPoint = "endPoint",
                Logo = "logo",
                Order = 1,
                EnableHttpAuth = true,
                AuthHttpUsername = "user",
                AuthHttpPassword = "pass",
                EnableProxy = true,
                UseProxySystem = true,
                ProxyAddress = "proxyaddr",
                ProxyPort = 8888,
                ProxyUsername = "proxyuser",
                ProxyPassword = "proxypass",
                Title = new Dictionary<string, string> {{"IT", "Titolo"}, {"EN", "Title"}},
                Slogan = new Dictionary<string, string> {{"FR", "SLOGANFR"}, {"IT", "SloganIT"}},
                Description = new Dictionary<string, string> {{"SP", "DESCSP"}},
                BackgroundMediaURL = "images/test.jpg",
                EmptyCellDefaultValue = "NaN",
                DefaultView = "viewDefaultTest",
                ShowDataflowUncategorized = true,
                ShowDataflowNotInProduction = true,
                CriteriaSelectionMode = "CriteriaTest",
                LabelDimensionTerritorials = null,
                LabelDimensionTemporals = null,
                CategorySchemaExcludes = null,
                EndPointFormatSupported = "",
                DecimalNumber = null,
                CatalogNavigationMode = "CatNavTest",
                ShowCategoryLevels = 1,
                Extras = new List<ExtraDto>
                    {new ExtraDto {IsPublic = false, Key = "extraKey", Value = "extraValue", ValueType = "valueType"}},
                TtlCatalog = 2234,
                TtlDataflow = 32345
            };

            var node = Node.CreateNode(nodeDto);
            EntityNodeData.CheckNodeEntityFromNodeDto(nodeDto, node);


            nodeDto = new NodeDto();
            nodeDto.Type = "type";
            nodeDto.Code = "code";
            nodeDto.EndPoint = "endPoint";
            nodeDto.Logo = "logo";
            nodeDto.Order = 4;
            nodeDto.EnableHttpAuth = false;
            nodeDto.AuthHttpUsername = null;
            nodeDto.AuthHttpPassword = null;
            nodeDto.AuthHttpDomain = "domainAuth";
            nodeDto.EnableProxy = false;
            nodeDto.UseProxySystem = true;
            nodeDto.ProxyAddress = "proxyaddr";
            nodeDto.ProxyPort = 1234;
            nodeDto.ProxyUsername = "proxyuser";
            nodeDto.ProxyPassword = "proxypass";
            nodeDto.ShowDataflowUncategorized = false;
            nodeDto.ShowDataflowNotInProduction = true;
            nodeDto.Title = new Dictionary<string, string> {{"IT", "Titolo"}, {"EN", "Title"}, {"FR", "TitleFR"}};
            nodeDto.Slogan = new Dictionary<string, string> {{"FR", "SLOGANFR"}};
            nodeDto.Description = null;
            nodeDto.Extras = new List<ExtraDto>
            {
                new ExtraDto
                {
                    IsPublic = false, Key = "extraKey", Value = "extraValue", ValueType = "valueType",
                    Transaltes = new Dictionary<string, string> {{"IT", "ExtraIT"}, {"EN", "ExtraEN"}}
                }
            };
            nodeDto.BackgroundMediaURL = "test2";
            nodeDto.TtlDataflow = null;
            nodeDto.TtlCatalog = null;

            node = Node.CreateNode(nodeDto);
            EntityNodeData.CheckNodeEntityFromNodeDto(nodeDto, node);


            nodeDto.Type = "type";
            nodeDto.Code = "code";
            nodeDto.EndPoint = "endPoint";
            nodeDto.Logo = "logo";
            nodeDto.Order = 4;
            nodeDto.EnableHttpAuth = false;
            nodeDto.AuthHttpUsername = null;
            nodeDto.AuthHttpPassword = null;
            nodeDto.AuthHttpDomain = "domainAuth2";
            nodeDto.EnableProxy = false;
            nodeDto.UseProxySystem = true;
            nodeDto.ProxyAddress = "proxyaddr";
            nodeDto.ProxyPort = 1234;
            nodeDto.ProxyUsername = "proxyuser";
            nodeDto.ProxyPassword = "proxypass";
            nodeDto.ShowDataflowUncategorized = true;
            nodeDto.ShowDataflowNotInProduction = false;
            nodeDto.Title = new Dictionary<string, string> {{"IT", "Titolo"}, {"EN", "Title"}, {"FR", "TitleFR"}};
            nodeDto.Slogan = new Dictionary<string, string> {{"FR", "SLOGANFR"}};
            nodeDto.Description = null;
            nodeDto.Extras = new List<ExtraDto>
            {
                new ExtraDto
                {
                    IsPublic = false, Key = "extraKey", Value = "extraValue", ValueType = "valueType", Transaltes = null
                }
            };
            nodeDto.BackgroundMediaURL = null;
            nodeDto.TtlDataflow = 10;
            nodeDto.TtlCatalog = 20;

            node = Node.CreateNode(nodeDto);
            EntityNodeData.CheckNodeEntityFromNodeDto(nodeDto, node);
        }

        [Fact]
        public void Node_AddExtraNewKey_Ok()
        {
            var node = EntityNodeData.CreateStandardNode();
            var extraKeyOne = node.Extras.First().Key;

            //New Extra
            var extraKeyNew = "extraKey2";
            var extraValueNew = "extraValue";
            var extraIsPublicNew = false;
            var valueTypeNew = "valueType";
            var extraTranslateNew = new Dictionary<string, string>
                {{"IT", "Titolo2"}, {"EN", "Title2"}, {"FR", "Title2"}};
            node.AddExtra(extraKeyNew, extraValueNew, extraIsPublicNew, valueTypeNew, extraTranslateNew);


            Assert.NotNull(node.Extras);
            Assert.Equal(2, node.Extras.Count);
            var keys = node.Extras.Select(i => i.Key).ToList();
            Assert.Contains(extraKeyOne, keys);
            Assert.Contains(extraKeyNew, keys);
            foreach (var itemExtra in node.Extras)
            {
                if (!itemExtra.Key.Equals(extraKeyNew, StringComparison.InvariantCultureIgnoreCase)) continue;
                if (extraTranslateNew == null)
                {
                    Assert.Null(itemExtra.TransatableItem);
                }
                else
                {
                    Assert.NotNull(itemExtra.TransatableItem);
                    Assert.Equal(extraTranslateNew.Count, itemExtra.TransatableItem.TransatableItemValues.Count);
                    foreach (var item in itemExtra.TransatableItem.TransatableItemValues)
                    {
                        Assert.True(extraTranslateNew.ContainsKey(item.Language));
                        Assert.Equal(extraTranslateNew[item.Language], item.Value);
                    }
                }
            }


            //New Extra
            var extraKeyNew2 = "extraKey3";
            var extraValueNew2 = "extraValue";
            var extraIsPublicNew2 = true;
            var valueTypeNew2 = "valueType";
            Dictionary<string, string> extraTranslateNew2 = null;
            node.AddExtra(extraKeyNew2, extraValueNew2, extraIsPublicNew2, valueTypeNew2, extraTranslateNew2);

            Assert.NotNull(node.Extras);
            Assert.Equal(3, node.Extras.Count);
            keys = node.Extras.Select(i => i.Key).ToList();
            Assert.Contains(extraKeyOne, keys);
            Assert.Contains(extraKeyNew, keys);
            Assert.Contains(extraKeyNew2, keys);
            foreach (var itemExtra in node.Extras)
            {
                if (!itemExtra.Key.Equals(extraKeyNew2, StringComparison.InvariantCultureIgnoreCase)) continue;
                if (extraTranslateNew2 == null)
                {
                    Assert.Null(itemExtra.TransatableItem);
                }
                else
                {
                    Assert.NotNull(itemExtra.TransatableItem);
                    Assert.Equal(extraTranslateNew2.Count, itemExtra.TransatableItem.TransatableItemValues.Count);
                    foreach (var item in itemExtra.TransatableItem.TransatableItemValues)
                    {
                        Assert.True(extraTranslateNew2.ContainsKey(item.Language));
                        Assert.Equal(extraTranslateNew2[item.Language], item.Value);
                    }
                }
            }
        }

        [Fact]
        public void Node_RemoveExtraNotFoundKey_ReturnFalseOk()
        {
            var node = EntityNodeData.CreateStandardNode();


            var extraKeyNew = "extraKey2";
            var extraValueNew = "extraValue";
            var extraIsPublicNew = false;
            var valueTypeNew = "valueType";
            var extraTranslateNew = new Dictionary<string, string>
                {{"IT", "Titolo2"}, {"EN", "Title2"}, {"FR", "Title2"}};
            node.AddExtra(extraKeyNew, extraValueNew, extraIsPublicNew, valueTypeNew, extraTranslateNew);
            Assert.Equal(2, node.Extras.Count);

            var extraKeyNew2 = "extraKey3";
            var extraValueNew2 = "extraValue";
            var extraIsPublicNew2 = true;
            var valueTypeNew2 = "valueType";
            Dictionary<string, string> extraTranslateNew2 = null;
            node.AddExtra(extraKeyNew2, extraValueNew2, extraIsPublicNew2, valueTypeNew2, extraTranslateNew2);
            Assert.Equal(3, node.Extras.Count);

            var result = node.RemoveExtra("extraKeyNotFound");

            Assert.False(result);
            Assert.Equal(3, node.Extras.Count);
        }

        [Fact]
        public void Node_RemoveExtra_Ok()
        {
            var node = EntityNodeData.CreateStandardNode();


            var extraKeyNew = "extraKey2";
            var extraValueNew = "extraValue";
            var extraIsPublicNew = false;
            var valueTypeNew = "valueType";
            var extraTranslateNew = new Dictionary<string, string>
                {{"IT", "Titolo2"}, {"EN", "Title2"}, {"FR", "Title2"}};
            node.AddExtra(extraKeyNew, extraValueNew, extraIsPublicNew, valueTypeNew, extraTranslateNew);

            var extraKeyNew2 = "extraKey3";
            var extraValueNew2 = "extraValue";
            var extraIsPublicNew2 = true;
            var valueTypeNew2 = "valueType";
            Dictionary<string, string> extraTranslateNew2 = null;
            node.AddExtra(extraKeyNew2, extraValueNew2, extraIsPublicNew2, valueTypeNew2, extraTranslateNew2);


            var result = node.RemoveExtra("extraKey3");

            Assert.True(result);
            Assert.Equal(2, node.Extras.Count);

            result = node.RemoveExtra("extraKey2");

            Assert.True(result);
            Assert.Equal(1, node.Extras.Count);

            result = node.RemoveExtra("extraKey");

            Assert.True(result);
            Assert.Equal(0, node.Extras.Count);

            Assert.NotNull(node.Extras);
        }

        [Fact]
        public void Node_AddExtraExistKey_OverwriteOk()
        {
            var node = EntityNodeData.CreateStandardNode();
            var extraKeyOne = node.Extras.First().Key;

            var extraKeyNew = "extraKey";
            var extraValueNew = "extraValue";
            var extraIsPublicNew = false;
            var valueTypeNew = "valueType";
            var extraTranslateNew = new Dictionary<string, string>
                {{"IT", "Titolo2"}, {"EN", "Title2"}, {"FR", "Title2"}};
            node.AddExtra(extraKeyNew, extraValueNew, extraIsPublicNew, valueTypeNew, extraTranslateNew);

            Assert.NotNull(node.Extras);
            Assert.Equal(1, node.Extras.Count);
            var keys = node.Extras.Select(i => i.Key).ToList();
            Assert.Contains(extraKeyOne, keys);
            Assert.Contains(extraKeyNew, keys);
            foreach (var itemExtra in node.Extras)
            {
                if (!itemExtra.Key.Equals(extraKeyNew, StringComparison.InvariantCultureIgnoreCase)) continue;
                if (extraTranslateNew == null)
                {
                    Assert.Null(itemExtra.TransatableItem);
                }
                else
                {
                    Assert.NotNull(itemExtra.TransatableItem);
                    Assert.Equal(extraTranslateNew.Count, itemExtra.TransatableItem.TransatableItemValues.Count);
                    foreach (var item in itemExtra.TransatableItem.TransatableItemValues)
                    {
                        Assert.True(extraTranslateNew.ContainsKey(item.Language));
                        Assert.Equal(extraTranslateNew[item.Language], item.Value);
                    }
                }
            }
        }

        [Fact]
        public void Node_SetNewTitle_Ok()
        {
            var node = EntityNodeData.CreateStandardNode();


            var titleNew = new Dictionary<string, string> {{"IT", "Titolo"}, {"EN", "Title"}, {"FR", "Title"}};
            node.SetTitleTransaltion(titleNew);
            Assert.Equal(titleNew.Count, node.Title.TransatableItemValues.Count);
            foreach (var item in node.Title.TransatableItemValues)
            {
                Assert.True(titleNew.ContainsKey(item.Language));
                Assert.Equal(titleNew[item.Language], item.Value);
            }
        }

        [Fact]
        public void Node_EditTitle_Ok()
        {
            var node = EntityNodeData.CreateStandardNode();

            var titleNew = new Dictionary<string, string> {{"IT", "Titolo"}, {"EN", "Title"}, {"FR", "Title"}};
            node.SetTitleTransaltion(titleNew);

            titleNew = new Dictionary<string, string> {{"IT", "Titolo"}, {"FR", "TitleFR"}, {"DE", "Title DE"}};
            node.SetTitleTransaltion(titleNew);


            Assert.Equal(titleNew.Count, node.Title.TransatableItemValues.Count);
            foreach (var item in node.Title.TransatableItemValues)
            {
                Assert.True(titleNew.ContainsKey(item.Language));
                Assert.Equal(titleNew[item.Language], item.Value);
            }
        }

        [Fact]
        public void Node_RemoveTitle_Ok()
        {
            var node = EntityNodeData.CreateStandardNode();

            node.SetTitleTransaltion(null);
            Assert.Null(node.Title);
        }

        [Fact]
        public void Node_SetNewDescription_Ok()
        {
            var node = EntityNodeData.CreateStandardNode();

            var descriptionNew = new Dictionary<string, string> {{"EN", "ENdesc"}, {"IT", "ITdesc"}};
            node.SetDescriptionTransaltion(descriptionNew);
            Assert.Equal(descriptionNew.Count, node.Description.TransatableItemValues.Count);
            foreach (var item in node.Description.TransatableItemValues)
            {
                Assert.True(descriptionNew.ContainsKey(item.Language));
                Assert.Equal(descriptionNew[item.Language], item.Value);
            }
        }

        [Fact]
        public void Node_RemoveDescription_Ok()
        {
            var node = EntityNodeData.CreateStandardNode();

            node.SetDescriptionTransaltion(null);
            Assert.Null(node.Description);
        }

        [Fact]
        public void Node_SetNewSlogan_Ok()
        {
            var node = EntityNodeData.CreateStandardNode();

            var sloganNew = new Dictionary<string, string> {{"FR", "SLOGANFR2"}, {"IT", "SloganIT2"}};
            node.SetSloganTransaltion(sloganNew);
            Assert.Equal(sloganNew.Count, node.Slogan.TransatableItemValues.Count);
            foreach (var item in node.Slogan.TransatableItemValues)
            {
                Assert.True(sloganNew.ContainsKey(item.Language));
                Assert.Equal(sloganNew[item.Language], item.Value);
            }
        }

        [Fact]
        public void Node_RemoveSlogan_Ok()
        {
            var node = EntityNodeData.CreateStandardNode();

            node.SetSloganTransaltion(null);
            Assert.Null(node.Slogan);
        }

        [Fact]
        public void EditVariusNode_WithCorrectData_Ok()
        {
            var nodeDto = new NodeDto
            {
                Type = "type",
                Code = "code",
                EndPoint = "endPoint",
                Logo = "logo",
                Order = 1,
                EnableHttpAuth = true,
                AuthHttpUsername = "user",
                AuthHttpPassword = "pass",
                EnableProxy = true,
                UseProxySystem = true,
                ProxyAddress = "proxyaddr",
                ProxyPort = 8888,
                ProxyUsername = "proxyuser",
                ProxyPassword = "proxypass",
                Title = new Dictionary<string, string> {{"IT", "Titolo"}, {"EN", "Title"}},
                Slogan = new Dictionary<string, string> {{"FR", "SLOGANFR"}, {"IT", "SloganIT"}},
                Description = new Dictionary<string, string> {{"SP", "DESCSP"}},
                BackgroundMediaURL = "images/test.jpg",
                EmptyCellDefaultValue = "NaN",
                DefaultView = "viewDefaultTest",
                ShowDataflowUncategorized = true,
                ShowDataflowNotInProduction = true,
                CriteriaSelectionMode = "CriteriaTest",
                LabelDimensionTerritorials = null,
                LabelDimensionTemporals = null,
                CategorySchemaExcludes = null,
                EndPointFormatSupported = "",
                DecimalNumber = null,
                CatalogNavigationMode = "CatNavTest",
                ShowCategoryLevels = 1,
                Extras = new List<ExtraDto>
                    {new ExtraDto {IsPublic = false, Key = "extraKey", Value = "extraValue", ValueType = "valueType"}},
                TtlCatalog = 2234,
                TtlDataflow = 32345
            };

            var node = Node.CreateNode(nodeDto);
            node.EditNode(nodeDto);
            EntityNodeData.CheckNodeEntityFromNodeDto(nodeDto, node);


            nodeDto = new NodeDto();
            nodeDto.Type = "type";
            nodeDto.Code = "code";
            nodeDto.EndPoint = "endPoint";
            nodeDto.Logo = "logo";
            nodeDto.Order = 4;
            nodeDto.EnableHttpAuth = false;
            nodeDto.AuthHttpUsername = null;
            nodeDto.AuthHttpPassword = null;
            nodeDto.AuthHttpDomain = "domainAuth3";
            nodeDto.EnableProxy = false;
            nodeDto.UseProxySystem = true;
            nodeDto.ProxyAddress = "proxyaddr";
            nodeDto.ProxyPort = 1234;
            nodeDto.ProxyUsername = "proxyuser";
            nodeDto.ProxyPassword = "proxypass";
            nodeDto.ShowDataflowUncategorized = false;
            nodeDto.ShowDataflowNotInProduction = false;
            nodeDto.Title = new Dictionary<string, string> {{"IT", "Titolo"}, {"FR", "TitleFR"}};
            nodeDto.Slogan = new Dictionary<string, string> {{"FR", "SLOGANFR"}, {"IT", "SLOGANIT"}};
            nodeDto.Description = null;
            nodeDto.Extras = new List<ExtraDto>
            {
                new ExtraDto
                {
                    IsPublic = false, Key = "extraKey2", Value = "extraValue2", ValueType = "valueType2",
                    Transaltes = new Dictionary<string, string> {{"IT", "ExtraIT2"}, {"FR", "ExtraFR"}}
                }
            };
            nodeDto.BackgroundMediaURL = "test2";
            nodeDto.TtlDataflow = 123456789;
            nodeDto.TtlCatalog = 187654321;

            node = Node.CreateNode(nodeDto);
            node.EditNode(nodeDto);
            EntityNodeData.CheckNodeEntityFromNodeDto(nodeDto, node);


            nodeDto.Type = "type";
            nodeDto.Code = "code";
            nodeDto.EndPoint = "endPoint";
            nodeDto.Logo = "logo";
            nodeDto.Order = 4;
            nodeDto.EnableHttpAuth = false;
            nodeDto.AuthHttpUsername = "AuthUser2";
            nodeDto.AuthHttpPassword = "AuthPass2";
            nodeDto.AuthHttpDomain = "domainAuth4";
            nodeDto.EnableProxy = false;
            nodeDto.UseProxySystem = true;
            nodeDto.ProxyAddress = "proxyaddr2";
            nodeDto.ProxyPort = 123;
            nodeDto.ProxyUsername = "proxyuser2";
            nodeDto.ProxyPassword = "proxypass2";
            nodeDto.EndPointFormatSupported = "FormatSupported";
            nodeDto.EmptyCellDefaultValue = "defaultvaluecell";
            nodeDto.ShowDataflowUncategorized = false;
            nodeDto.ShowDataflowNotInProduction = true;
            nodeDto.DecimalNumber = 2;
            nodeDto.DecimalSeparator = new Dictionary<string, string> {{"IT", "1"}, {"EN", "3"}};
            nodeDto.CategorySchemaExcludes = new List<string> {"Cat1", "Cat2"};
            nodeDto.Title = new Dictionary<string, string> {{"IT", "Titolo"}, {"EN", "Title"}, {"FR", "TitleFR"}};
            nodeDto.Slogan = new Dictionary<string, string> {{"FR", "SLOGANFR"}};
            nodeDto.Description = null;
            nodeDto.Extras = new List<ExtraDto>
            {
                new ExtraDto
                {
                    IsPublic = false, Key = "extraKey", Value = "extraValue", ValueType = "valueType", Transaltes = null
                }
            };
            nodeDto.BackgroundMediaURL = null;
            nodeDto.TtlDataflow = null;
            nodeDto.TtlCatalog = null;

            node = Node.CreateNode(nodeDto);
            node.EditNode(nodeDto);
            EntityNodeData.CheckNodeEntityFromNodeDto(nodeDto, node);
        }

        [Fact]
        public void Node_SetDecimalSeparetor_Ok()
        {
            var node = EntityNodeData.CreateStandardNode();

            var decimalSeparatorNew = new Dictionary<string, string> {{"EN", "ENdesc"}, {"IT", "ITdesc"}};
            node.SetDecimalSeparetorTransaltion(decimalSeparatorNew);
            Assert.Equal(decimalSeparatorNew.Count, node.DecimalSeparator.TransatableItemValues.Count);
            foreach (var item in node.DecimalSeparator.TransatableItemValues)
            {
                Assert.True(decimalSeparatorNew.ContainsKey(item.Language));
                Assert.Equal(decimalSeparatorNew[item.Language], item.Value);
            }
        }

        [Fact]
        public void Node_RemoveDecimalSeparetor_Ok()
        {
            var node = EntityNodeData.CreateStandardNode();

            node.SetDecimalSeparetorTransaltion(null);
            Assert.Null(node.DecimalSeparator);
        }
    }
}