using System;
using System.Collections.Generic;
using System.Linq;
using DataBrowser.AC.Utility.Helpers.ExtMethod;
using DataBrowser.Domain.Dtos;
using DataBrowser.Interfaces.Dto.UseCases.Responses;
using EndPointConnector.Models;

namespace WSHUB.Models.Response
{
    public class DatasetCriteriaViewModel
    {
        public List<CriteriaViewMode> Criteria { get; set; }
        public string CriteriaView { get; set; }
        public StructureLayout Layout { get; set; }
        public StructureLayoutChart LayoutChart { get; set; }
        public int DecimalPlaces { get; set; }
        public string DecimalSeparator { get; set; }
        public List<string> GeoIds { get; set; }
        public List<FilterCriteria> Filters { get; set; }

        public List<string> Keywords { get; set; }
        public List<string> AttachedDataFiles { get; }
        public string DefaultPresentation { get; set; }
        public string ReferenceMetadata { get; set; }
        public string EmptyCellPlaceHolder { get; set; }
        public string DefaultNote { get; set; }
        public List<string> TerritorialDimensions { get; set; }
        public string Source { get; set; }
        public long? MaxTableCells { get; set; }
        public int? ObsCount { get; set; }

        public string DefaultView { get; set; }

        public ViewTemplateDto Template { get; set; }
        public ViewTemplateDto View { get; set; }


        public static DatasetCriteriaViewModel ConvertFromStructureDto(
            StructureCriteriaForDataflowResponse criteriaForDataflow, string lang = null)
        {
            var datasetStructureViewModel = new DatasetCriteriaViewModel();
            if (criteriaForDataflow != null)
            {
                datasetStructureViewModel.Criteria = criteriaForDataflow.Criterias?.Select(i => new CriteriaViewMode
                {
                    Id = i.Id, Label = i?.Titles.GetTranslateItem(lang),
                    Extra = i.DataStructureRef?.Id != null
                        ? new Dictionary<string, string> {{"DataStructureRef", i.DataStructureRef.Id}}
                        : null
                })?.ToList();
                datasetStructureViewModel.CriteriaView = criteriaForDataflow.CriteriaMode;
                if (criteriaForDataflow.LayoutRows != null ||
                    criteriaForDataflow.LayoutColumns != null ||
                    criteriaForDataflow.LayoutRowSelections != null)
                    datasetStructureViewModel.Layout = new StructureLayout
                    {
                        Rows = criteriaForDataflow.LayoutRows,
                        Cols = criteriaForDataflow.LayoutColumns,
                        Sections = criteriaForDataflow.LayoutRowSelections
                    };
                if (criteriaForDataflow.LayoutChartPrimaryDim != null ||
                    criteriaForDataflow.LayoutChartSecondaryDim != null ||
                    criteriaForDataflow.LayoutChartFilter != null)
                    datasetStructureViewModel.LayoutChart = new StructureLayoutChart
                    {
                        PrimaryDim = criteriaForDataflow.LayoutChartPrimaryDim,
                        SecondaryDim = criteriaForDataflow.LayoutChartSecondaryDim,
                        Filters = criteriaForDataflow.LayoutChartFilter
                    };

                if (criteriaForDataflow.DefaultCodeSelected != null)
                    datasetStructureViewModel.Filters = criteriaForDataflow.DefaultCodeSelected;


                datasetStructureViewModel.DecimalPlaces = criteriaForDataflow.DecimalNumber;
                datasetStructureViewModel.DecimalSeparator = criteriaForDataflow.DecimalSeparator;
                datasetStructureViewModel.GeoIds = criteriaForDataflow.GeoIds;
                datasetStructureViewModel.EmptyCellPlaceHolder = criteriaForDataflow.EmptyCellPlaceHolder;
                datasetStructureViewModel.TerritorialDimensions = criteriaForDataflow.TerritorialDimensions;
                datasetStructureViewModel.MaxTableCells = criteriaForDataflow.MaxCell;
                datasetStructureViewModel.Template = criteriaForDataflow.Template;
                datasetStructureViewModel.View = criteriaForDataflow.View;
                datasetStructureViewModel.DefaultView = criteriaForDataflow.DefaultView;
            }

            return datasetStructureViewModel;
        }

        public static DatasetCriteriaViewModel ConvertFromDataDto(ArtefactContainer artefactContainer,
            string lang = null)
        {
            var datasetStructureViewModel = new DatasetCriteriaViewModel();
            if (artefactContainer != null && artefactContainer.Criterias != null)
            {
                datasetStructureViewModel.Criteria = new List<CriteriaViewMode>();

                foreach (var itemCriteria in artefactContainer.Criterias)
                {
                    var criteriaViewMode = new CriteriaViewMode
                    {
                        Id = itemCriteria.Id,
                        Label = itemCriteria?.Titles.GetTranslateItem(lang)
                    };
                    datasetStructureViewModel.Criteria.Add(criteriaViewMode);

                    if (itemCriteria.Values == null) continue;

                    var dicItemParents = new Dictionary<string, bool>();

                    criteriaViewMode.Values = new List<CodeViewMode>();
                    foreach (var itemValue in itemCriteria.Values)
                    {
                        string parentId = null;
                        if (!string.IsNullOrWhiteSpace(itemValue.ParentId))
                        {
                            var haveItemParent = false;
                            if (dicItemParents.ContainsKey(itemValue.ParentId))
                            {
                                haveItemParent = dicItemParents[itemValue.ParentId];
                            }
                            else
                            {
                                haveItemParent = itemCriteria.Values.Any(i =>
                                    i.Id.Equals(itemValue.ParentId, StringComparison.InvariantCultureIgnoreCase));
                                dicItemParents.Add(itemValue.ParentId, haveItemParent);
                            }

                            if (haveItemParent) parentId = itemValue.ParentId;
                        }

                        criteriaViewMode.Values.Add(new CodeViewMode
                        {
                            Id = itemValue.Id,
                            ParentId = parentId,
                            Name = itemValue?.Names?.GetTranslateItem(lang),
                            IsDefault = itemValue.IsDefault.GetOnlySpecificTranslateItem(lang),
                            IsSelectable = itemValue.IsSelectable,
                            IsUnSelectable = itemValue.IsUnSelectable
                        });
                    }

                    //Values = itemCriteria?.Values?.Select(k => new CodeViewMode
                    //{
                    //    Id = k.Id,
                    //    ParentId = !string.IsNullOrWhiteSpace(k.ParentId) && hashSet.Any(z => z.Id.Equals(k.ParentId, StringComparison.InvariantCultureIgnoreCase)) ? k.ParentId : null,
                    //    Name = k?.Names?.GetTranslateItem(lang),
                    //    IsDefault = k.IsDefault.GetOnlySpecificTranslateItem(lang)
                    //}).ToList()
                }

                datasetStructureViewModel.CriteriaView = null;
                datasetStructureViewModel.Filters = null;
                datasetStructureViewModel.Layout = null;
                datasetStructureViewModel.DecimalPlaces = 0;
                datasetStructureViewModel.DecimalSeparator = null;
                datasetStructureViewModel.GeoIds = null;
                datasetStructureViewModel.ObsCount = artefactContainer.ObsCount;
            }

            return datasetStructureViewModel;
        }
    }

    public class CriteriaViewMode
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public List<CodeViewMode> Values { get; set; }
        public Dictionary<string, string> Extra { get; set; }
    }

    public class CodeViewMode
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool? IsSelectable { get; set; }
        public bool? IsUnSelectable { get; set; }
    }

    public class StructureLayout
    {
        public List<string> Rows { get; set; }
        public List<string> Cols { get; set; }
        public List<string> Sections { get; set; }
    }

    public class StructureLayoutChart
    {
        public List<string> PrimaryDim { get; set; }
        public List<string> SecondaryDim { get; set; }
        public List<string> Filters { get; set; }
    }

    public class Filters
    {
        
    }
}