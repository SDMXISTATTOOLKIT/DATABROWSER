using DataBrowser.AC.Utility.Helpers.ExtMethod;
using DataBrowser.Domain.Dtos;
using DataBrowser.DomainServices.Interfaces;
using EndPointConnector.Models;
using System.Collections.Generic;
using System.Linq;

namespace DataBrowser.DomainServices
{
    public class DatasetService : IDatasetService
    {
        public Dataset CreateDataset(Dataflow dataflow, Dsd dsdWithData, string lang)
        {
            return CreateDataset(null, null, dataflow, dsdWithData, lang);
        }

        public Dataset CreateDataset(HubDto hub, NodeDto node, Dataflow dataflow, Dsd dsd, string lang)
        {
            var dataset = new Dataset
            {
                CriteriaMode = node?.CriteriaSelectionMode,
                DecimalNumber = node?.DecimalNumber ?? hub?.DecimalNumber ?? 2,
                DecimalSeparator = node?.DecimalSeparator.GetTranslateItem(lang),
                EmptyCellPlaceHolder = node?.EmptyCellDefaultValue ?? hub?.EmptyCellDefaultValue ?? "",
                TerritorialDimensions = node?.LabelDimensionTerritorials,
                MaxCell = hub?.MaxCells ?? 500000,
            };

            var maxObs = dataflow?.MaxObservations?.GetTranslateItem(lang);
            maxObs = maxObs ?? dsd?.MaxObservations?.GetTranslateItem(lang);
            if (maxObs == null)
            {
                maxObs = hub?.MaxObservationsAfterCriteria;
            }
            dataset.MaxObservation = maxObs;

            if (dataflow?.DefaultCodeSelected != null &&
                (dataflow.DefaultCodeSelected.ContainsKey(lang) ||
                 dataflow.DefaultCodeSelected.Count > 0))
                dataset.DefaultCodeSelected =
                    dataflow.DefaultCodeSelected.ContainsKey(lang)
                        ? dataflow.DefaultCodeSelected[lang]
                        : dataflow.DefaultCodeSelected.First().Value;
            else if (dsd.DefaultCodeSelected != null && dsd.DefaultCodeSelected.Count > 0)
                dataset.DefaultCodeSelected =
                    dsd.DefaultCodeSelected.ContainsKey(lang)
                        ? dsd.DefaultCodeSelected[lang]
                        : dsd.DefaultCodeSelected.First().Value;

            if (dataflow?.DecimalNumber != null &&
                (dataflow.DecimalNumber.ContainsKey(lang) || dataflow.DecimalNumber.Count > 0))
            {
                var decimalNum = dataflow.DecimalNumber.ContainsKey(lang)
                    ? dataflow.DecimalNumber[lang]
                    : dataflow.DecimalNumber.First().Value;
                if (decimalNum != null) dataset.DecimalNumber = decimalNum.Value;
            }

            if (dataflow?.MaxCell != null &&
                (dataflow.MaxCell.ContainsKey(lang) || dataflow.MaxCell.Count > 0))
                dataset.MaxCell = dataflow.MaxCell.ContainsKey(lang)
                    ? dataflow.MaxCell[lang]
                    : dataflow.MaxCell.First().Value;
            
            if (dataflow?.NotDisplay != null &&
                (dataflow.NotDisplay.ContainsKey(lang) || dataflow.NotDisplay.Count > 0))
                dataset.NotDisplay = dataflow.NotDisplay.ContainsKey(lang)
                    ? dataflow.NotDisplay[lang]
                    : dataflow.NotDisplay.First().Value;
            else if (dsd?.NotDisplay != null && dsd.NotDisplay.Count > 0)
                dataset.NotDisplay = dsd.NotDisplay.ContainsKey(lang)
                    ? dsd.NotDisplay[lang]
                    : dsd.NotDisplay.First().Value;


            dataset.CriteriaMode = popolateAnnotationValue(dataflow?.CriteriaSelectionMode, lang, dataset.CriteriaMode);
            dataset.DecimalSeparator = popolateAnnotationValue(dataflow?.DecimalSeparator, lang, dataset.DecimalSeparator);
            dataset.EmptyCellPlaceHolder = popolateAnnotationValue(dataflow?.EmptyCellPlaceHolder, lang, dataset.EmptyCellPlaceHolder);
            dataset.DefaultView = popolateAnnotationValue(dataflow?.DefaultView, lang, dataset.DefaultView);

            dataset.LayoutColumns = popolateAnnotationValues(dataflow?.LayoutColumns, dsd.LayoutColumns, lang);
            dataset.LayoutRows = popolateAnnotationValues(dataflow?.LayoutRows, dsd.LayoutRows, lang);
            dataset.LayoutRowSections = popolateAnnotationValues(dataflow?.LayoutRowSections, dsd.LayoutRowSections, lang);
            dataset.GeoIds = popolateAnnotationValues(dataflow?.GeoIds, dsd?.GeoIds, lang);
            dataset.TerritorialDimensions = popolateAnnotationValues(dataflow?.TerritorialDimensions, null, lang, dataset.TerritorialDimensions);
            dataset.LayoutChartPrimaryDim = popolateAnnotationValues(dataflow?.LayoutChartPrimaryDim, dsd?.LayoutChartPrimaryDim, lang);
            dataset.LayoutChartSecondaryDim = popolateAnnotationValues(dataflow?.LayoutChartSecondaryDim, dsd?.LayoutChartSecondaryDim, lang);
            dataset.LayoutChartFilter = popolateAnnotationValues(dataflow?.LayoutChartFilter, dsd?.LayoutChartFilter, lang);

            return dataset;
        }

        private string popolateAnnotationValue(Dictionary<string, string> dataflowAnnotation, string lang, string defaultValue = null)
        {
            if (dataflowAnnotation != null &&
                (dataflowAnnotation.ContainsKey(lang) ||
                 dataflowAnnotation.Count > 0))
                return dataflowAnnotation.ContainsKey(lang)
                        ? dataflowAnnotation[lang]
                        : dataflowAnnotation.First().Value;

            return defaultValue;
        }

        private List<string> popolateAnnotationValues(Dictionary<string, List<string>> dataflowAnnotation, Dictionary<string, List<string>> dsdAnnotaion, string lang, List<string> defaultValue = null)
        {
            if (dataflowAnnotation != null &&
                (dataflowAnnotation.ContainsKey(lang) || dataflowAnnotation.Count > 0))
                return dataflowAnnotation.ContainsKey(lang)
                    ? dataflowAnnotation[lang]
                    : dataflowAnnotation.First().Value;
            else if (dsdAnnotaion != null && dsdAnnotaion.Count > 0)
                return dsdAnnotaion.ContainsKey(lang)
                    ? dsdAnnotaion[lang]
                    : dsdAnnotaion.First().Value;

            return defaultValue;
        }
    }
}
