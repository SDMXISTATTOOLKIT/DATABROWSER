import React, {Component, createRef, Fragment} from 'react';
import ChartJs from 'chart.js';
import {withTranslation} from "react-i18next";
import {getFormattedValue} from "../../utils/formatters";
import {
  getDataIdxFromCoordinatesArray,
  getDimensionValueLabelFromJsonStat,
  getDimensionValuesIndexesMap,
  TIME_PERIOD_DIMENSION_KEY
} from "../../utils/jsonStat";
import CustomEmpty from "../custom-empty";
import _ from "lodash";
import {v4 as uuidv4} from 'uuid';
import "chartjs-plugin-zoom"
import {getNthValorizedElementIndexInBooleanArray} from "../../utils/other";
import {CHART_COLORS_ALL_DIMENSION_VALUES_KEY} from "../chart-settings/Colors";
import {LABEL_FORMAT_SELECTOR_LABEL_FORMAT_NAME} from "../label-format-selector/constants";

export const CHART_LEGEND_POSITION_TOP = "top";
export const CHART_LEGEND_POSITION_RIGHT = "right";
export const CHART_LEGEND_POSITION_BOTTOM = "bottom";
export const CHART_LEGEND_POSITION_lEFT = "left";

export const CHART_LEGEND_POSITION_DEFAULT = CHART_LEGEND_POSITION_TOP;

const DEFAULT_LEGEND_POSITION = CHART_LEGEND_POSITION_TOP;
const DEFAULT_STACKED = false;
const DEFAULT_COLORS = {};
const defaultColorArr = [
  "rgba(30, 136, 229, 0.3)",
  "rgba(194, 24, 91, 0.3)",
  "rgba(253, 216, 53, 0.3)",
  "rgba(14, 157, 89, 0.3)",
  "rgba(240, 98, 146, 0.3)",
  "rgba(255, 112, 67, 0.3)",
  "rgba(139, 195, 74, 0.3)",
  "rgba(0, 188, 212, 0.3)",
  "rgba(234, 63, 77, 0.3)",
  "rgba(170, 71, 188, 0.3)",
  "rgba(38, 166, 154, 0.3)",
  "rgba(255, 152, 0, 0.3)"
];

const getChartType = type => {
  switch (type) {
    case "area":
      return "line"
    case "pyramid":
      return "horizontalBar"
    default:
      return type
  }
};

const isBarChart = type => (type === "bar" || type === "horizontalBar" || type === "pyramid");

const isLineChart = type => (type === "line" || type === "area" || type === "radar");

const isBarOrLineChart = type => (isBarChart(type) || isLineChart(type));

const getOptions = (chartType, data, decimalSeparator, decimalPlaces, legendPosition, stacked, disableWheelZoom) => {

  let max = 0;
  data.datasets.forEach(dataset => dataset.data.forEach(value => max = Math.max(max, Math.abs(value))));
  max += Math.floor(max / 100 * 20);

  return {
    responsive: true,
    maintainAspectRatio: false,
    scales: (isBarOrLineChart(chartType) && chartType !== "radar")
      ? {
        xAxes: [{
          gridLines: {
            display: chartType !== "bar"
          },
          ticks: {
            beginAtZero: (chartType === "horizontalBar" || chartType === "pyramid"),
            callback: value => {
              if (chartType === "horizontalBar" || chartType === "pyramid") {
                if (chartType === "pyramid" && data.datasets.length === 2 && value < 0) {
                  return getFormattedValue(-value, decimalSeparator, decimalPlaces)
                } else {
                  return getFormattedValue(value, decimalSeparator, decimalPlaces)
                }
              } else {
                return value
              }
            },
            max: chartType === "pyramid" ? max : undefined,
            min: chartType === "pyramid" ? -(max) : undefined,
          },
          stacked: (chartType === "pyramid" || (stacked && isBarOrLineChart(chartType)))
        }],
        yAxes: [{
          gridLines: {
            display: (chartType !== "horizontalBar" && chartType !== "pyramid")
          },
          ticks: {
            beginAtZero: (chartType !== "horizontalBar" && chartType !== "pyramid"),
            callback: value => (chartType !== "horizontalBar" && chartType !== "pyramid")
              ? getFormattedValue(value, decimalSeparator, decimalPlaces)
              : value
          },
          stacked: (chartType === "pyramid" || (stacked && isBarOrLineChart(chartType)))
        }]
      }
      : {},
    legend: {
      display: (!isBarOrLineChart(chartType) || (data.datasets || []).length > 1),
      position: legendPosition
    },
    tooltips: {
      enabled: true,
      mode: "index",
      callbacks: {
        title: (tooltipItem, data) => (data.labels[tooltipItem[0].index] || ''),
        label: (tooltipItem, data) => {
          const getValue = () => {
            let value = dataset.data[tooltipItem.index];
            if (!value) {
              return null;
            }
            value = getFormattedValue(value, decimalSeparator, decimalPlaces);
            if (chartType === "pyramid" && data.datasets.length === 2 && tooltipItem.datasetIndex === 0 && value[0] === "-") {
              return value.slice(1)
            } else {
              return value
            }
          };
          const dataset = data.datasets[tooltipItem.datasetIndex];
          const value = getValue();
          return value ? `${dataset.label}: ${getValue()}` : null
        }
      }
    },
    plugins: disableWheelZoom
      ? {}
      : {
        zoom: {
          pan: {
            enabled: (isBarOrLineChart(chartType) && chartType !== "radar"),
            mode: () => (chartType === "horizontalBar" || chartType === "pyramid") ? "x" : "y"
          },
          zoom: {
            enabled: (isBarOrLineChart(chartType) && chartType !== "radar"),
            mode: () => (chartType === "horizontalBar" || chartType === "pyramid") ? "x" : "y"
          }
        }
      }
  }
};

const getData = (chartType, jsonStat, layout, removeEmptyValues, labelFormat, colors, stacked) => {

  const {
    primaryDim: primaryDimArr,
    primaryDimValues: originalPrimaryDimValues,
    secondaryDim: secondaryDimArr,
    secondaryDimValues: originalSecondaryDimValues,
    filtersValue
  } = layout;

  const primaryDim = primaryDimArr[0];
  const primaryDimValues = primaryDim
    ? jsonStat.dimension[primaryDim].category.index.filter(dimVal => originalPrimaryDimValues.includes(dimVal))
    : originalPrimaryDimValues;
  const secondaryDim = secondaryDimArr[0];
  const secondaryDimValues = secondaryDim
    ? jsonStat.dimension[secondaryDim].category.index.filter(dimVal => originalSecondaryDimValues.includes(dimVal))
    : originalSecondaryDimValues;

  const indexesMap = getDimensionValuesIndexesMap(jsonStat);

  const valorizedDataMap = {};
  secondaryDimValues.forEach(secondaryDimValue => {
    valorizedDataMap[secondaryDimValue] = new Array(primaryDimValues.length).fill(false);
  });
  const valorizedDataArr = new Array(primaryDimValues.length).fill(false);

  const datasets = [];

  if (secondaryDimValues && secondaryDimValues.length > 0) {
    secondaryDimValues.forEach((secondaryDimValue, secDimIdx) => {
      const data = [];
      primaryDimValues.forEach((primaryDimValue, primDimIdx) => {
        const dimValueArray = jsonStat.id.map(dim => {
          if (dim === primaryDim) {
            return primaryDimValue;
          } else if (dim === secondaryDim) {
            return secondaryDimValue;
          } else {
            return filtersValue[dim];
          }
        });
        const valueIdx = getDataIdxFromCoordinatesArray(
          dimValueArray.map((value, idx) => indexesMap[jsonStat.id[idx]][value]),
          jsonStat.size
        );
        const value = jsonStat.value[valueIdx];
        if (value !== null && value !== undefined) {
          if (chartType === "pyramid" && secondaryDimValues.length === 2 && secDimIdx === 0) {
            data.push(-value);
          } else {
            data.push(value);
          }
          valorizedDataMap[secondaryDimValue][primDimIdx] = true;
        } else {
          data.push(null);
        }
      });
      datasets.push({
        label: getDimensionValueLabelFromJsonStat(jsonStat, secondaryDim, secondaryDimValue, labelFormat),
        data: data
      });
    });

  } else {
    const data = [];
    primaryDimValues.forEach((primaryDimValue, primDimIdx) => {
      const dimValueArray = jsonStat.id.map(dim => {
        if (dim === primaryDim) {
          return primaryDimValue;
        } else {
          return filtersValue[dim];
        }
      });
      const valueIdx = getDataIdxFromCoordinatesArray(
        dimValueArray.map((value, idx) => indexesMap[jsonStat.id[idx]][value]),
        jsonStat.size
      );
      const value = jsonStat.value[valueIdx];
      if (value !== null && value !== undefined) {
        data.push(value);
        valorizedDataArr[primDimIdx] = true;
      } else {
        data.push(null);
      }
    });
    datasets.push({
      label: "",
      data: data
    });
  }

  const labels = [];

  const valorizedValues = (secondaryDimValues && secondaryDimValues.length > 0)
    ? new Array(primaryDimValues.length)
    : [...valorizedDataArr];

  if (removeEmptyValues) {
    if (secondaryDimValues && secondaryDimValues.length > 0) {
      primaryDimValues.forEach((_, idx) => {
        let found = false;
        secondaryDimValues.forEach(secDim => {
          if (!found && valorizedDataMap[secDim][idx] === true) {
            found = true;
          }
        });
        valorizedValues[idx] = found;
      });
    }

    datasets.forEach(dataset => dataset.data = dataset.data.filter((_, idx) => valorizedValues[idx]));

    primaryDimValues.forEach((value, idx) => {
      if (valorizedValues[idx]) {
        labels.push(getDimensionValueLabelFromJsonStat(jsonStat, primaryDim, value, labelFormat));
      }
    });

  } else {
    primaryDimValues.forEach(value => {
      labels.push(getDimensionValueLabelFromJsonStat(jsonStat, primaryDim, value, labelFormat));
    });
  }

  const getPrimaryDimValueColors = datasetIdx => {
    return datasets[datasetIdx].data.map((_, valIdx) => {
      const idx = removeEmptyValues ? getNthValorizedElementIndexInBooleanArray(valorizedValues, valIdx) : valIdx;

      return colors?.[primaryDim]?.[primaryDimValues[idx]] ||
        colors?.[secondaryDim]?.[secondaryDimValues[datasetIdx]] ||
        (!secondaryDim && colors?.[primaryDim]?.[CHART_COLORS_ALL_DIMENSION_VALUES_KEY]) ||
        defaultColorArr[datasetIdx % defaultColorArr.length];
    });
  }

  const getDatasetColors = datasetIdx => {
    if (isBarChart(chartType)) {
      return getPrimaryDimValueColors(datasetIdx);

    } else if (isLineChart(chartType)) {
      return (secondaryDim && colors?.[secondaryDim]?.[secondaryDimValues[datasetIdx]]) ||
        (!secondaryDim && colors?.[primaryDim]?.[CHART_COLORS_ALL_DIMENSION_VALUES_KEY]) ||
        defaultColorArr[datasetIdx % defaultColorArr.length];

    } else {
      return datasets[datasetIdx].data.map((_, valIdx) => {
        const idx = removeEmptyValues ? getNthValorizedElementIndexInBooleanArray(valorizedValues, valIdx) : valIdx;

        return colors?.[primaryDim]?.[primaryDimValues[idx]] ||
          defaultColorArr[valIdx % defaultColorArr.length];
      });
    }
  };

  const getFill = datasetIdx => {
    let fill = false;
    if (chartType === "area") {
      fill = stacked
        ? datasetIdx === 0 ? "origin" : "-1"
        : "origin";
    } else if (chartType === "radar") {
      fill = "origin";
    }
    return fill
  }


  return {
    labels: labels,
    datasets: datasets.map((dataset, datasetIdx) => ({
      ...dataset,
      backgroundColor: getDatasetColors(datasetIdx),
      borderColor: getDatasetColors(datasetIdx),
      borderWidth: 2,
      lineTension: chartType === "area" ? 0 : undefined,
      fill: getFill(datasetIdx),
      pointBackgroundColor: getPrimaryDimValueColors(datasetIdx),
      pointRadius: 4,
      pointHoverRadius: 4,
      pointBorderColor: "white",
      pointBorderWidth: 1,
      pointHoverBorderWidth: 1
    }))
  };
};

const getDefaultRemoveEmptyValues = (type, {primaryDim}) => !((type === "line" || type === "area") && primaryDim[0] === TIME_PERIOD_DIMENSION_KEY);

class Chart extends Component {

  constructor(props) {
    super(props);
    this.chartRef = createRef();
  }

  componentDidMount() {
    const {
      jsonStat,
      type,
      layout,
      removeEmptyValues = getDefaultRemoveEmptyValues(type, layout),
      labelFormat = LABEL_FORMAT_SELECTOR_LABEL_FORMAT_NAME,
      decimalSeparator,
      decimalPlaces,
      legendPosition = DEFAULT_LEGEND_POSITION,
      stacked = DEFAULT_STACKED,
      colors = DEFAULT_COLORS,
      disableWheelZoom = false
    } = this.props;

    ChartJs.defaults.global.defaultFontFamily = "'Roboto', 'Helvetica', 'Arial', sans-serif";

    const data = getData(type, jsonStat, layout, removeEmptyValues, labelFormat, colors, stacked);
    this.myChart = new ChartJs(this.chartRef.current, {
      type: getChartType(type),
      options: getOptions(type, data, decimalSeparator, decimalPlaces, legendPosition, stacked, disableWheelZoom),
      data: data,
      plugins: [
        {
          beforeDraw: function (chart) {
            const ctx = chart.chart.ctx;
            ctx.fillStyle = "white";
            ctx.fillRect(0, 0, chart.chart.width, chart.chart.height);
          }
        }
      ]
    });
  }

  componentDidUpdate(prevProps) {
    const {
      jsonStat,
      type,
      layout,
      removeEmptyValues = getDefaultRemoveEmptyValues(type, layout),
      labelFormat = LABEL_FORMAT_SELECTOR_LABEL_FORMAT_NAME,
      decimalSeparator,
      decimalPlaces,
      legendPosition = DEFAULT_LEGEND_POSITION,
      stacked = DEFAULT_STACKED,
      colors = DEFAULT_COLORS,
      disableWheelZoom = false
    } = this.props;

    if (
      prevProps.jsonStat !== jsonStat ||
      prevProps.type !== type ||
      !_.isEqual(prevProps.layout, layout) ||
      (prevProps.labelFormat !== null && prevProps.labelFormat !== undefined && prevProps.labelFormat !== labelFormat) ||
      (prevProps.decimalSeparator !== null && prevProps.decimalSeparator !== undefined && prevProps.decimalSeparator !== decimalSeparator) ||
      (prevProps.decimalPlaces !== null && prevProps.decimalPlaces !== undefined && prevProps.decimalPlaces !== decimalPlaces) ||
      (prevProps.legendPosition !== null && prevProps.legendPosition !== undefined && prevProps.legendPosition !== legendPosition) ||
      (prevProps.stacked !== null && prevProps.stacked !== undefined && prevProps.stacked !== stacked) ||
      (prevProps.colors !== null && prevProps.colors !== undefined && !_.isEqual(prevProps.colors, colors))
    ) {
      const data = getData(type, jsonStat, layout, removeEmptyValues, labelFormat, colors, stacked);
      this.myChart.config.type = getChartType(type);
      this.myChart.options = getOptions(type, data, decimalSeparator, decimalPlaces, legendPosition, stacked, disableWheelZoom);
      this.myChart.data = data;
      this.myChart.update();
    }
  }

  render() {
    const {
      t,
      chartId = `cart__${uuidv4()}`,
      type,
      layout
    } = this.props;

    const hideChart = (type === "pyramid" && ((layout.secondaryDim || []).length !== 1 || (layout.secondaryDimValues || []).length !== 2));

    return (
      <Fragment>
        {hideChart && (
          <CustomEmpty text={t("components.chart.pyramid.invalidSecondaryDimension")}/>
        )}
        <canvas
          id={chartId}
          ref={this.chartRef}
          style={{
            display: hideChart ? "none" : "block"
          }}
        />
      </Fragment>
    );
  }
}

export default withTranslation()(Chart);