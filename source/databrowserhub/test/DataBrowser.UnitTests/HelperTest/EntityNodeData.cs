using System;
using System.Collections.Generic;
using System.Linq;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.Nodes;
using Xunit;

namespace DataBrowser.UnitTests.HelperTest
{
    public static class EntityNodeData
    {
        public static List<Node> Get_Nodes(int numberOfNodes)
        {
            var nodes = new List<Node>();

            for (var i = 0; i < numberOfNodes; i++)
                nodes.Add(
                    Node.CreateNode(new NodeDto {Type = $"type{i}", Code = $"code{i}", EndPoint = $"endpoint{i}"}));

            return nodes;
        }

        public static List<NodeDto> Get_NodesDto(int numberOfNodes)
        {
            var nodes = new List<NodeDto>();

            for (var i = 0; i < numberOfNodes; i++)
                nodes.Add(new NodeDto
                {
                    Type = $"type{i}",
                    Code = $"code{i}",
                    EndPoint = $"endpoint{i}"
                });

            return nodes;
        }


        public static NodeDto CreateStandardNodeDto()
        {
            var nodeDto = new NodeDto();
            nodeDto.Type = "type";
            nodeDto.Code = "code";
            nodeDto.EndPoint = "endPoint";
            nodeDto.Logo = "logo";
            nodeDto.Order = 1;
            nodeDto.EnableHttpAuth = true;
            nodeDto.AuthHttpUsername = "user";
            nodeDto.AuthHttpPassword = "pass";
            nodeDto.EnableProxy = true;
            nodeDto.UseProxySystem = true;
            nodeDto.ProxyAddress = "proxyaddr";
            nodeDto.ProxyPort = 8888;
            nodeDto.ProxyUsername = "proxyuser";
            nodeDto.ProxyPassword = "proxypass";
            nodeDto.Title = new Dictionary<string, string> {{"IT", "Titolo"}, {"EN", "Title"}};
            nodeDto.Slogan = new Dictionary<string, string> {{"FR", "SLOGANFR"}, {"IT", "SloganIT"}};
            nodeDto.Description = new Dictionary<string, string> {{"SP", "DESCSP"}};
            nodeDto.Extras = new List<ExtraDto>
            {
                new ExtraDto
                {
                    IsPublic = false, Key = "extraKey", Value = "extraValue", ValueType = "valueType",
                    Transaltes = new Dictionary<string, string> {{"IT", "ExtraIT"}, {"EN", "ExtraEN"}}
                }
            };
            nodeDto.BackgroundMediaURL = "backgroundMediaUrl string";
            return nodeDto;
        }

        public static Node CreateStandardNode()
        {
            return Node.CreateNode(CreateStandardNodeDto());
        }

        public static void CheckNodeEntityFromNodeDto(NodeDto nodeDto, Node node)
        {
            Assert.Equal(nodeDto.Type, node.Type);
            Assert.Equal(nodeDto.Code, node.Code);
            Assert.Equal(nodeDto.EndPoint, node.EndPoint);
            Assert.Equal(nodeDto.Logo, node.Logo);
            Assert.Equal(nodeDto.Order, node.Order);
            Assert.Equal(nodeDto.EnableHttpAuth, node.EnableHttpAuth);
            Assert.Equal(nodeDto.AuthHttpUsername, node.AuthHttpUsername);
            Assert.Equal(nodeDto.AuthHttpPassword, node.AuthHttpPassword);
            Assert.Equal(nodeDto.EnableProxy, node.EnableProxy);
            Assert.Equal(nodeDto.UseProxySystem, node.UseProxySystem);
            Assert.Equal(nodeDto.ProxyAddress, node.ProxyAddress);
            Assert.Equal(nodeDto.ProxyPort, node.ProxyPort);
            Assert.Equal(nodeDto.ProxyUsername, node.ProxyUsername);
            Assert.Equal(nodeDto.ProxyPassword, node.ProxyPassword);
            Assert.Equal(nodeDto.CriteriaSelectionMode, node.CriteriaSelectionMode);
            Assert.Equal(nodeDto.DefaultView, node.DefaultView);
            Assert.Equal(nodeDto.EmptyCellDefaultValue, node.EmptyCellDefaultValue);
            Assert.Equal(nodeDto.Logo, node.Logo);
            Assert.Equal(nodeDto.ShowCategoryLevels, node.ShowCategoryLevels);
            Assert.Equal(nodeDto.CatalogNavigationMode, node.CatalogNavigationMode);
            Assert.Equal(nodeDto.BackgroundMediaURL, node.BackgroundMediaURL);
            Assert.Equal(nodeDto.EndPointFormatSupported, node.EndPointFormatSupported);
            Assert.Equal(nodeDto.TtlDataflow, node.TtlDataflow);
            Assert.Equal(nodeDto.TtlCatalog, node.TtlCatalog);

            if (nodeDto.Title == null || nodeDto.Title.Count == 0)
            {
                Assert.Null(node.Title);
            }
            else
            {
                Assert.Equal(nodeDto.Title.Count, node.Title.TransatableItemValues.Count);
                foreach (var item in node.Title.TransatableItemValues)
                {
                    Assert.True(nodeDto.Title.ContainsKey(item.Language));
                    Assert.Equal(nodeDto.Title[item.Language], item.Value);
                }
            }

            if (nodeDto.Description == null || nodeDto.Description.Count == 0)
            {
                Assert.Null(node.Description);
            }
            else
            {
                Assert.Equal(nodeDto.Description.Count, node.Description.TransatableItemValues.Count);
                foreach (var item in node.Description.TransatableItemValues)
                {
                    Assert.True(nodeDto.Description.ContainsKey(item.Language));
                    Assert.Equal(nodeDto.Description[item.Language], item.Value);
                }
            }

            if (nodeDto.Slogan == null || nodeDto.Slogan.Count == 0)
            {
                Assert.Null(node.Slogan);
            }
            else
            {
                Assert.Equal(nodeDto.Slogan.Count, node.Slogan.TransatableItemValues.Count);
                foreach (var item in node.Slogan.TransatableItemValues)
                {
                    Assert.True(nodeDto.Slogan.ContainsKey(item.Language));
                    Assert.Equal(nodeDto.Slogan[item.Language], item.Value);
                }
            }

            if (nodeDto.Extras == null || nodeDto.Extras.Count == 0)
            {
                Assert.Null(node.Extras);
            }
            else
            {
                Assert.NotNull(node.Extras);
                foreach (var itemDtoExtra in nodeDto.Extras)
                foreach (var itemExtra in node.Extras)
                {
                    if (!itemDtoExtra.Key.Equals(itemExtra.Key, StringComparison.InvariantCultureIgnoreCase)) continue;
                    Assert.Equal(itemDtoExtra.Key, itemExtra.Key);
                    Assert.Equal(itemDtoExtra.Value, itemExtra.Value);
                    Assert.Equal(itemDtoExtra.ValueType, itemExtra.ValueType);
                    Assert.Equal(itemDtoExtra.IsPublic, itemExtra.IsPublic);
                    if (itemDtoExtra.Transaltes == null)
                    {
                        Assert.Null(itemExtra.TransatableItem);
                    }
                    else
                    {
                        Assert.NotNull(itemExtra.TransatableItem);
                        Assert.Equal(itemDtoExtra.Transaltes.Count,
                            itemExtra.TransatableItem.TransatableItemValues.Count);
                        foreach (var item in itemExtra.TransatableItem.TransatableItemValues)
                        {
                            Assert.True(itemDtoExtra.Transaltes.ContainsKey(item.Language));
                            Assert.Equal(itemDtoExtra.Transaltes[item.Language], item.Value);
                        }
                    }
                }
            }
        }

        public static void CheckNodeDtoFromNodeEntity(Node node, NodeDto nodeDto)
        {
            Assert.Equal(node.Type, nodeDto.Type);
            Assert.Equal(node.Code, nodeDto.Code);
            Assert.Equal(node.EndPoint, nodeDto.EndPoint);
            Assert.Equal(node.Logo, nodeDto.Logo);
            Assert.Equal(node.Order, nodeDto.Order);
            Assert.Equal(node.EnableHttpAuth, nodeDto.EnableHttpAuth);
            Assert.Equal(node.AuthHttpUsername, nodeDto.AuthHttpUsername);
            Assert.Equal(node.AuthHttpPassword, nodeDto.AuthHttpPassword);
            Assert.Equal(node.AuthHttpDomain, nodeDto.AuthHttpDomain);
            Assert.Equal(node.EnableProxy, nodeDto.EnableProxy);
            Assert.Equal(node.UseProxySystem, nodeDto.UseProxySystem);
            Assert.Equal(node.ProxyAddress, nodeDto.ProxyAddress);
            Assert.Equal(node.ProxyPort, nodeDto.ProxyPort);
            Assert.Equal(node.ProxyUsername, nodeDto.ProxyUsername);
            Assert.Equal(node.ProxyPassword, nodeDto.ProxyPassword);
            Assert.Equal(node.CriteriaSelectionMode, nodeDto.CriteriaSelectionMode);
            Assert.Equal(node.DefaultView, nodeDto.DefaultView);
            Assert.Equal(node.EmptyCellDefaultValue, nodeDto.EmptyCellDefaultValue);
            Assert.Equal(node.Logo, nodeDto.Logo);
            Assert.Equal(node.ShowCategoryLevels, nodeDto.ShowCategoryLevels);
            Assert.Equal(node.CatalogNavigationMode, nodeDto.CatalogNavigationMode);
            Assert.Equal(node.BackgroundMediaURL, nodeDto.BackgroundMediaURL);
            Assert.Equal(node.EndPointFormatSupported, nodeDto.EndPointFormatSupported);
            Assert.Equal(node.TtlDataflow, nodeDto.TtlDataflow);
            Assert.Equal(node.TtlCatalog, nodeDto.TtlCatalog);

            if (node.Title == null)
            {
                Assert.Empty(nodeDto.Title);
            }
            else
            {
                Assert.Equal(node.Title.TransatableItemValues.Count, nodeDto.Title.Count);
                foreach (var item in nodeDto.Title)
                {
                    Assert.Contains(node.Title.TransatableItemValues, i => i.Language.Equals(item.Key));
                    var entityTransalte = node.Title.TransatableItemValues.First(i => i.Language.Equals(item.Key));
                    Assert.Equal(entityTransalte.Value, item.Value);
                }
            }

            if (node.Description == null)
            {
                Assert.Empty(nodeDto.Description);
            }
            else
            {
                Assert.Equal(node.Description.TransatableItemValues.Count, nodeDto.Description.Count);
                foreach (var item in nodeDto.Description)
                {
                    Assert.Contains(node.Description.TransatableItemValues, i => i.Language.Equals(item.Key));
                    var entityTransalte =
                        node.Description.TransatableItemValues.First(i => i.Language.Equals(item.Key));
                    Assert.Equal(entityTransalte.Value, item.Value);
                }
            }

            if (node.Slogan == null)
            {
                Assert.Empty(nodeDto.Slogan);
            }
            else
            {
                Assert.Equal(node.Slogan.TransatableItemValues.Count, nodeDto.Slogan.Count);
                foreach (var item in nodeDto.Slogan)
                {
                    Assert.Contains(node.Slogan.TransatableItemValues, i => i.Language.Equals(item.Key));
                    var entityTransalte = node.Slogan.TransatableItemValues.First(i => i.Language.Equals(item.Key));
                    Assert.Equal(entityTransalte.Value, item.Value);
                }
            }


            if (node.Extras == null || node.Extras.Count == 0)
            {
                Assert.Empty(nodeDto.Extras);
            }
            else
            {
                Assert.NotNull(nodeDto.Extras);
                Assert.Equal(node.Extras.Count, nodeDto.Extras.Count);
                foreach (var item in node.Extras)
                {
                    Assert.Contains(nodeDto.Extras, i => i.Key.Equals(item.Key));
                    var extraDto = nodeDto.Extras.First(i => i.Key.Equals(item.Key));
                    Assert.Equal(item.Value, extraDto.Value);
                    Assert.Equal(item.IsPublic, extraDto.IsPublic);
                    Assert.Equal(item.Key, extraDto.Key);
                    Assert.Equal(item.ValueType, extraDto.ValueType);

                    if (item.TransatableItem == null || item.TransatableItem.TransatableItemValues == null ||
                        item.TransatableItem.TransatableItemValues.Count == 0)
                    {
                        Assert.Null(extraDto.Transaltes);
                    }
                    else
                    {
                        Assert.NotNull(extraDto.Transaltes);
                        Assert.Equal(item.TransatableItem.TransatableItemValues.Count, extraDto.Transaltes.Count);
                        foreach (var itemTranslate in item.TransatableItem.TransatableItemValues)
                        {
                            Assert.Contains(extraDto.Transaltes, i => i.Key.Equals(itemTranslate.Language));
                            var translateDto = extraDto.Transaltes.First(i => i.Key.Equals(itemTranslate.Language));
                            Assert.Equal(itemTranslate.Value, translateDto.Value);
                            Assert.Equal(itemTranslate.Language, translateDto.Key);
                        }
                    }
                }
            }
        }
    }
}