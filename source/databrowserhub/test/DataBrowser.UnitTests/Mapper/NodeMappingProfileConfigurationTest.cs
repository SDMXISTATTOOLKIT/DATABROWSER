using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DataBrowser.AC;
using DataBrowser.AC.Utility;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.UnitTests.HelperTest;
using Xunit;

namespace DataBrowser.UnitTests.Mapper
{
    public class NodeMappingProfileConfigurationTest
    {
        private readonly IMapper _mapper;

        public NodeMappingProfileConfigurationTest()
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile(new MappingProfileConfiguration()); });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void Check_NodeEntityToNodeDto_Ok()
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
                {new ExtraDto {IsPublic = false, Key = "extraKey", Value = "extraValue", ValueType = "valueType"}};

            var node = Node.CreateNode(nodeDto);
            checkDtoNode(node);


            nodeDto = new NodeDto();
            nodeDto.Type = "type";
            nodeDto.Code = "code";
            nodeDto.EndPoint = "endPoint";
            nodeDto.Logo = "logo";
            nodeDto.Order = 4;
            nodeDto.EnableHttpAuth = false;
            nodeDto.AuthHttpUsername = null;
            nodeDto.AuthHttpPassword = null;
            nodeDto.EnableProxy = false;
            nodeDto.UseProxySystem = true;
            nodeDto.ProxyAddress = "proxyaddr";
            nodeDto.ProxyPort = 1234;
            nodeDto.ProxyUsername = "proxyuser";
            nodeDto.ProxyPassword = "proxypass";
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

            node = Node.CreateNode(nodeDto);
            checkDtoNode(node);


            nodeDto = new NodeDto();
            nodeDto.Type = "type";
            nodeDto.Code = "code";
            nodeDto.EndPoint = "endPoint";
            nodeDto.Logo = "logo";
            nodeDto.Order = 4;
            nodeDto.EnableHttpAuth = false;
            nodeDto.AuthHttpUsername = null;
            nodeDto.AuthHttpPassword = null;
            nodeDto.EnableProxy = false;
            nodeDto.UseProxySystem = true;
            nodeDto.ProxyAddress = "proxyaddr";
            nodeDto.ProxyPort = 1234;
            nodeDto.ProxyUsername = "proxyuser";
            nodeDto.ProxyPassword = "proxypass";
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

            node = Node.CreateNode(nodeDto);
            checkDtoNode(node);
        }

        private void checkDtoNode(Node nodeEntity)
        {
            var nodeDto = nodeEntity.ConvertToNodeDto(_mapper);
            EntityNodeData.CheckNodeDtoFromNodeEntity(nodeEntity, nodeDto);
        }

        [Fact]
        public void Check_NodeEntityToNodeDataView_Ok()
        {
            var nodeEntity = EntityNodeData.CreateStandardNode();

            checkNodeDataView(nodeEntity);


            var nodeDto = new NodeDto();
            nodeDto.Type = "type";
            nodeDto.Code = "code";
            nodeDto.EndPoint = "endPoint";
            nodeDto.Logo = "logo";
            nodeDto.Order = 4;
            nodeDto.EnableHttpAuth = false;
            nodeDto.AuthHttpUsername = null;
            nodeDto.AuthHttpPassword = null;
            nodeDto.EnableProxy = false;
            nodeDto.UseProxySystem = true;
            nodeDto.ProxyAddress = "proxyaddr";
            nodeDto.ProxyPort = 1234;
            nodeDto.ProxyUsername = "proxyuser";
            nodeDto.ProxyPassword = "proxypass";
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
            nodeDto.BackgroundMediaURL = "backgroundMediaUrl string";
            nodeEntity = Node.CreateNode(nodeDto);

            checkNodeDataView(nodeEntity);


            nodeDto = new NodeDto();
            nodeDto.Type = "type";
            nodeDto.Code = "code";
            nodeDto.EndPoint = "endPoint";
            nodeDto.Logo = "logo";
            nodeDto.Order = 4;
            nodeDto.EnableHttpAuth = false;
            nodeDto.AuthHttpUsername = null;
            nodeDto.AuthHttpPassword = null;
            nodeDto.EnableProxy = false;
            nodeDto.UseProxySystem = true;
            nodeDto.ProxyAddress = "proxyaddr";
            nodeDto.ProxyPort = 1234;
            nodeDto.ProxyUsername = "proxyuser";
            nodeDto.ProxyPassword = "proxypass";
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

            checkNodeDataView(nodeEntity);
        }

        private void checkNodeDataView(Node nodeEntity)
        {
            var nodeDataView = nodeEntity.ConvertToNodeDataView(_mapper);


            Assert.Equal(nodeDataView.Type, nodeEntity.Type);
            Assert.Equal(nodeDataView.Code, nodeEntity.Code);
            Assert.Equal(nodeDataView.Logo, nodeEntity.Logo);
            Assert.Equal(nodeDataView.Order, nodeEntity.Order);
            Assert.Equal(nodeDataView.BackgroundMediaURL, nodeEntity.BackgroundMediaURL);

            if (nodeEntity.Title == null)
            {
                Assert.True(nodeDataView.Title == null);
            }
            else if (nodeEntity.Title.TransatableItemValues == null)
            {
                Assert.Empty(nodeDataView.Title);
            }
            else
            {
                Assert.Equal(nodeEntity.Title.TransatableItemValues.Count, nodeDataView.Title.Count);

                foreach (var item in nodeEntity.Title.TransatableItemValues)
                {
                    Assert.True(nodeDataView.Title.ContainsKey(item.Language));
                    Assert.Equal(item.Value, nodeDataView.Title[item.Language]);
                }
            }

            if (nodeEntity.Description == null)
            {
                Assert.True(nodeDataView.Description == null || nodeDataView.Description.Count == 0);
            }
            else if (nodeEntity.Description.TransatableItemValues == null)
            {
                Assert.Empty(nodeDataView.Description);
            }
            else
            {
                Assert.Equal(nodeEntity.Description.TransatableItemValues.Count, nodeDataView.Description.Count);

                foreach (var item in nodeEntity.Description.TransatableItemValues)
                {
                    Assert.True(nodeDataView.Description.ContainsKey(item.Language));
                    Assert.Equal(item.Value, nodeDataView.Description[item.Language]);
                }
            }

            if (nodeEntity.Slogan == null)
            {
                Assert.True(nodeDataView.Slogan == null || nodeDataView.Slogan.Count == 0);
            }
            else if (nodeEntity.Slogan.TransatableItemValues == null)
            {
                Assert.Empty(nodeDataView.Slogan);
            }
            else
            {
                Assert.Equal(nodeEntity.Slogan.TransatableItemValues.Count, nodeDataView.Slogan.Count);

                foreach (var item in nodeEntity.Slogan.TransatableItemValues)
                {
                    Assert.True(nodeDataView.Slogan.ContainsKey(item.Language));
                    Assert.Equal(item.Value, nodeDataView.Slogan[item.Language]);
                }
            }

            if (nodeEntity.Extras == null)
            {
                Assert.True(nodeDataView.Extras == null || nodeDataView.Extras.Count == 0);
            }
            else
            {
                Assert.Equal(nodeEntity.Extras.Count, nodeDataView.Extras.Count);

                foreach (var itemExtra in nodeEntity.Extras)
                {
                    Assert.Contains(nodeDataView.Extras, i => i.Key.Equals(itemExtra.Key));

                    var nodeExtraModelView = nodeDataView.Extras.First(i => i.Key.Equals(itemExtra.Key));

                    Assert.Equal(itemExtra.Key, nodeExtraModelView.Key);
                    Assert.Equal(itemExtra.Value, nodeExtraModelView.Value);
                    Assert.Equal(itemExtra.ValueType, nodeExtraModelView.ValueType);
                    Assert.Equal(itemExtra.IsPublic, nodeExtraModelView.IsPublic);

                    if (itemExtra.TransatableItem == null)
                    {
                        Assert.Null(nodeExtraModelView.Transaltes);
                    }
                    else if (itemExtra.TransatableItem.TransatableItemValues == null)
                    {
                        Assert.Null(nodeExtraModelView.Transaltes);
                    }
                    else
                    {
                        Assert.Equal(itemExtra.TransatableItem.TransatableItemValues.Count,
                            nodeExtraModelView.Transaltes.Count);

                        foreach (var itemTransalte in itemExtra.TransatableItem.TransatableItemValues)
                        {
                            Assert.Contains(nodeExtraModelView.Transaltes.Keys, i => i.Equals(itemTransalte.Language));
                            var extraTranslateModelView =
                                nodeExtraModelView.Transaltes.First(i => i.Key.Equals(itemTransalte.Language));

                            Assert.Equal(itemTransalte.Language, extraTranslateModelView.Key);
                            Assert.Equal(itemTransalte.Value, extraTranslateModelView.Value);
                        }
                    }
                }
            }
        }
    }
}