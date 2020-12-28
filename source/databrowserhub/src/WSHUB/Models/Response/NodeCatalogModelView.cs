using DataBrowser.AC.Utility;
using DataBrowser.AC.Utility.Helpers.ExtMethod;
using EndPointConnector.Models.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WSHUB.Models.Response
{
    public class NodeCatalogModelView
    {
        public List<CategoryGroupModelView> CategoryGroups;
        public Dictionary<string, DatasetModelView> DatasetMap;
        public List<DatasetUncategorizedModelView> DatasetUncategorized;

        public static NodeCatalogModelView ConvertFromDto(NodeCatalogDto nodeCatalogDto, string lang, string nodeCode)
        {
            var nodeCatalogModelView = new NodeCatalogModelView();

            if (nodeCatalogDto == null)
            {
                return null;
            }

            if (nodeCatalogDto.CategoryGroups != null)
            {
                nodeCatalogModelView.CategoryGroups = new List<CategoryGroupModelView>();
                foreach (var itemGroup in nodeCatalogDto.CategoryGroups)
                {
                    var item = convertCategoryGroup(itemGroup, lang, nodeCode);
                    if (item?.Categories != null && item.Categories.Count > 0)
                    {
                        nodeCatalogModelView.CategoryGroups.Add(item);
                    }
                }

                if (nodeCatalogModelView.CategoryGroups.Count == 0)
                {
                    nodeCatalogModelView.CategoryGroups = null;
                }
            }

            if (nodeCatalogDto.DatasetMap != null)
            {
                nodeCatalogModelView.DatasetMap = new Dictionary<string, DatasetModelView>();
                foreach (var itemDataset in nodeCatalogDto.DatasetMap)
                {
                    var dataset = convertDatasetMap(itemDataset.Value, lang);
                    nodeCatalogModelView.DatasetMap.Add(convertIdToViewModel(itemDataset.Key), dataset);
                }

                if (nodeCatalogModelView.DatasetMap.Count == 0)
                {
                    nodeCatalogModelView.DatasetMap = null;
                }
            }

            if (nodeCatalogDto.DatasetUncategorized != null)
            {
                nodeCatalogModelView.DatasetUncategorized = new List<DatasetUncategorizedModelView>();
                foreach (var itemDataset in nodeCatalogDto.DatasetUncategorized)
                {
                    nodeCatalogModelView.DatasetUncategorized.Add(convertDatasetUncategorized(itemDataset, lang));
                }

                if (nodeCatalogModelView.DatasetUncategorized.Count == 0)
                {
                    nodeCatalogModelView.DatasetUncategorized = null;
                }
            }

            return nodeCatalogModelView;
        }

        private static CategoryGroupModelView convertCategoryGroup(CategoryGroupDto categoryGroupDto, string lang, string nodeCode)
        {
            if (categoryGroupDto == null)
            {
                return null;
            }

            var categoryGroupModelView = new CategoryGroupModelView
            {
                Id = convertIdToViewModel(categoryGroupDto.Id),
                Label = categoryGroupDto.Labels.GetTranslateItem(lang),
                Description = categoryGroupDto.Descriptions.GetTranslateItem(lang),
                Extras = categoryGroupDto?.Extras?.Select(i => new ExtraValueModelView
                { Key = i.Key, Type = i.Type, Value = i.Values.GetTranslateItem(lang) })?.ToList(),
                Categories = categoryGroupDto?.Categories?.Select(i => convertCategory(i, lang, nodeCode))
                .Where(i => i != null).ToList()
            };

            return categoryGroupModelView;
        }

        private static CategoryModelView convertCategory(CategoryDto categoryDto, string lang, string nodeCode)
        {
            if (categoryDto == null)
            {
                return null;
            }

            var categoryModelView = new CategoryModelView
            {
                Id = categoryDto.Id,
                Label = categoryDto.Labels.GetTranslateItem(lang),
                Image = getImagesPath(categoryDto.Id, nodeCode),
                Description = categoryDto.Descriptions.GetTranslateItem(lang),
                Extras = categoryDto?.Extras?.Select(i => new ExtraValueModelView
                { Key = i.Key, Type = i.Type, Value = i.Values.GetTranslateItem(lang) })?.ToList(),
                DatasetIdentifiers =
                categoryDto?.DatasetIdentifiers?.Select(i => convertIdToViewModel(i)).ToHashSet(),
                ChildrenCategories = categoryDto?.ChildrenCategories
                ?.Select(i => convertCategory(i, lang, nodeCode)).Where(i => i != null).ToList()
            };

            if ((categoryModelView.ChildrenCategories == null || categoryModelView.ChildrenCategories.Count <= 0) &&
                (categoryModelView.DatasetIdentifiers == null || categoryModelView.DatasetIdentifiers.Count <= 0))
            {
                //remove empty category
                return null;
            }

            return categoryModelView;
        }

        private static DatasetModelView convertDatasetMap(DatasetDto datasetDto, string lang)
        {
            if (datasetDto == null)
            {
                return null;
            }

            var datasetDtoModelView = new DatasetModelView
            {
                Description = datasetDto.Descriptions.GetTranslateItem(lang),
                //datasetDtoModelView.Extras = datasetDto?.Extras?.Select(i => new ExtraValueModelView { Key = i.Key, Type = i.Type, Value = i.Values.GetTranslateItem(lang) })?.ToList();
                Source = datasetDto.Source.GetOnlySpecificTranslateItem(lang),
                Title = datasetDto.Titles.GetTranslateItem(lang),
                ReferenceMetadata = datasetDto.MetadataUrl.GetTranslateItem(lang),
                Keywords = datasetDto.Keywords.GetOnlySpecificTranslateItem(lang)
            };
            var attachedDatas = datasetDto.AttachedDataFiles.GetOnlySpecificTranslateItem(lang);
            if (attachedDatas != null)
            {
                datasetDtoModelView.AttachedDataFiles = new List<AttachedDataFiles>();
                try
                {
                    foreach (var item in attachedDatas)
                    {
                        var itemSplit = item.Split("|");
                        datasetDtoModelView.AttachedDataFiles.Add(new AttachedDataFiles
                            {Format = itemSplit[1], Url = itemSplit[0]});
                    }
                }
                catch (Exception)
                {
                }
            }

            datasetDtoModelView.Note = datasetDto.DefaultNote.GetOnlySpecificTranslateItem(lang);
            datasetDtoModelView.LayoutFilter = datasetDto.LayoutFilter.GetOnlySpecificTranslateItem(lang);
            datasetDtoModelView.CatalogType = datasetDto.DataflowCatalogType.GetOnlySpecificTranslateItem(lang, defaultValue: "normal");

            return datasetDtoModelView;
        }

        private static DatasetUncategorizedModelView convertDatasetUncategorized(DatasetDto datasetDto, string lang)
        {
            if (datasetDto == null)
            {
                return null;
            }

            var datasetDtoModelView = new DatasetUncategorizedModelView
            {
                Identifier = convertIdToViewModel(datasetDto.Identifier),
                Description = datasetDto.Descriptions.GetTranslateItem(lang),
                //datasetDtoModelView.Extras = datasetDto?.Extras?.Select(i => new ExtraValueModelView { Key = i.Key, Type = i.Type, Value = i.Values.GetTranslateItem(lang) })?.ToList();
                Source = datasetDto.Source.GetOnlySpecificTranslateItem(lang),
                Title = datasetDto.Titles.GetTranslateItem(lang),
                ReferenceMetadata = datasetDto.MetadataUrl.GetTranslateItem(lang),
                Keywords = datasetDto.Keywords.GetOnlySpecificTranslateItem(lang)
            };
            var attachedDatas = datasetDto.AttachedDataFiles.GetOnlySpecificTranslateItem(lang);
            if (attachedDatas != null)
            {
                datasetDtoModelView.AttachedDataFiles = new List<AttachedDataFiles>();
                try
                {
                    foreach (var item in attachedDatas)
                    {
                        var itemSplit = item.Split("|");
                        datasetDtoModelView.AttachedDataFiles.Add(new AttachedDataFiles
                            {Format = itemSplit[1], Url = itemSplit[0]});
                    }
                }
                catch (Exception)
                {
                }
            }

            datasetDtoModelView.Note = datasetDto.DefaultNote.GetOnlySpecificTranslateItem(lang);
            datasetDtoModelView.LayoutFilter = datasetDto.LayoutFilter.GetOnlySpecificTranslateItem(lang);
            datasetDtoModelView.CatalogType = datasetDto.DataflowCatalogType.GetTranslateItem(lang, defaultValue: "normal");

            return datasetDtoModelView;
        }

        private static string convertIdToViewModel(string id)
        {
            return RequestAdapter.ConvertDataflowIdToUriFormat(id);
        }

        private static string getImagesPath(string id, string nodeCode)
        {
            var path = DataBrowserDirectory.GetCategoryImageDirPath();
            var pathWithNode = DataBrowserDirectory.GetCategoryImageDirPath() + $"/{nodeCode}";

            var image = $"{pathWithNode}/{id}.png";
            if (File.Exists(image))
            {
                return DataBrowserDirectory.ConvertAbsoluteToRelativePath(image);
            }

            image = $"{pathWithNode}/{RequestAdapter.ConvertDataflowIdToUriFormat(id)}.png";
            if (File.Exists(image))
            {
                return DataBrowserDirectory.ConvertAbsoluteToRelativePath(image);
            }

            return null;
        }
    }

    public class CategoryGroupModelView
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public List<ExtraValueModelView> Extras { get; set; }
        public List<CategoryModelView> Categories { get; set; }
    }

    public class CategoryModelView
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public List<CategoryModelView> ChildrenCategories { get; set; }
        public List<ExtraValueModelView> Extras { get; set; }
        public HashSet<string> DatasetIdentifiers { get; set; }
    }

    public class ExtraValueModelView
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }

    public class DatasetModelView
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public List<ExtraValueModelView> Extras { get; set; }
        public string ReferenceMetadata { get; set; }
        public List<string> Keywords { get; set; }
        public List<AttachedDataFiles> AttachedDataFiles { get; set; }
        public List<string> LayoutFilter { get; set; }
        public string Note { get; set; }
        public string CatalogType { get; set; }
    }

    public class AttachedDataFiles
    {
        public string Url { get; set; }
        public string Format { get; set; }
    }

    public class DatasetUncategorizedModelView : DatasetModelView
    {
        public string Identifier { get; set; }
    }
}