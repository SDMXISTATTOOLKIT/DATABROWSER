import _ from "lodash";
import {
  LABEL_FORMAT_SELECTOR_LABEL_FORMAT_BOTH,
  LABEL_FORMAT_SELECTOR_LABEL_FORMAT_ID,
  LABEL_FORMAT_SELECTOR_LABEL_FORMAT_NAME
} from "../components/label-format-selector/constants";

export const TIME_PERIOD_DIMENSION_KEY = "TIME_PERIOD";
export const FREQ_DIMENSION_KEY = "FREQ";

const MAX_COL_COUNT = 10000;
const MAX_ROW_COUNT = 10000;

const toUpperCaseFirst = string => (string !== null && string.length > 0)
  ? string[0].toUpperCase() + string.slice(1)
  : string;

export const getDimensionLabelFromJsonStat = (jsonStat, dimension, format) => {
  const name = toUpperCaseFirst(jsonStat.dimension[dimension].label);
  const id = dimension;

  return (name || id);
};

export const getDimensionValueLabelFromJsonStat = (jsonStat, dimension, value, format) => {
  const name = toUpperCaseFirst(jsonStat.dimension[dimension].category.label[value]);
  const id = value;

  if (dimension === TIME_PERIOD_DIMENSION_KEY) {
    return name;
  }

  switch (format) {
    case LABEL_FORMAT_SELECTOR_LABEL_FORMAT_NAME:
      return name;
    case LABEL_FORMAT_SELECTOR_LABEL_FORMAT_ID:
      return id;
    case LABEL_FORMAT_SELECTOR_LABEL_FORMAT_BOTH:
      return `[${id}] ${name}`;
    default:
      return name;
  }
};

export const getDataIdxFromCoordinatesArray = (dimensions, size) => {
  let dataIdx = 0;
  let multiplier = 1;

  // row-major order algorithm
  for (let i = 0; i < dimensions.length; i++) {
    multiplier *= (i > 0) ? size[dimensions.length - i] : 1;
    dataIdx += multiplier * dimensions[dimensions.length - i - 1];
  }

  return dataIdx;
};

export const getCoordinatesArrayFromDataIdx = (dataIdx, size) => {
  const coordinates = new Array(size.length);

  // reverse row-major order algorithm
  let offset = dataIdx;
  for (let i = size.length - 1; i >= 0; i--) {
    const dimensionSize = size[i];
    coordinates[i] = dimensionSize > 0 ? (offset % dimensionSize) : 0;
    offset = dimensionSize > 0 ? Math.floor(offset / dimensionSize) : 0;
  }

  return coordinates;
};

export const getJsonStatTableSize = (jsonStat, layout) => {
  if (jsonStat) {
    let count = 1;

    if (!layout) {
      jsonStat.size.forEach(el => count *= el);

    } else {
      let colCount = 1;
      layout.cols.forEach(col => colCount *= jsonStat.size[jsonStat.id.indexOf(col)]);
      let rowCount = 1;
      layout.rows.forEach(row => rowCount *= jsonStat.size[jsonStat.id.indexOf(row)]);
      layout.sections.forEach(section => rowCount *= jsonStat.size[jsonStat.id.indexOf(section)]);
      count = colCount * rowCount;
    }

    return count;

  } else {
    return 0
  }
};

export const getFilterTreeFromJsonStat = (layout, jsonStat) => {
  const filters = (layout?.primaryDim || []).concat((layout?.secondaryDim || [])).concat(layout?.filters || []);

  let paths = [];
  Object.keys(jsonStat.value).forEach(key => {
    const coordinates = getCoordinatesArrayFromDataIdx(key, jsonStat.size);
    const path = [];
    filters.forEach(dim => {
      const dimValueIdx = coordinates[jsonStat.id.indexOf(dim)];
      path.push(jsonStat.dimension[dim].category.index[dimValueIdx]);
    });
    paths.push(path);
  });

  const tree = {};
  paths.forEach(path => {
    let currentLevel = tree;
    path.forEach(elem => {
      if (!currentLevel[elem]) {
        currentLevel[elem] = {};
      }
      currentLevel = currentLevel[elem];
    });
  });

  return tree
};

export const getInitialFiltersValue = (jsonStat, layout, filterTree) => {
  const primaryDim = layout.primaryDim?.[0] || null;
  const secondaryDim = layout.secondaryDim?.[0] || null;
  const filters = layout.filters || [];

  let selectedPrimaryDim = null;
  let selectedSecondaryDim = null;

  if (filters.length === 0) {
    return {}
  }

  let root = filterTree;
  if (primaryDim) {
    selectedPrimaryDim = Object.keys(root).find(primDimKey =>
      layout.primaryDimValues.includes(primDimKey) && (!secondaryDim || Object.keys(root[primDimKey]).find(secDimKey =>
        layout.secondaryDimValues.includes(secDimKey))));
    root = root[selectedPrimaryDim];
    if (secondaryDim) {
      selectedSecondaryDim = Object.keys(root).find(secDimKey =>
        layout.secondaryDimValues.includes(secDimKey));
      root = root[selectedSecondaryDim];
    }
  }

  const filtersValue = {};

  let iter = root;
  layout.filters.forEach(dim => {
    const selectableValues = Object.keys(iter);
    const allValues = [...jsonStat.dimension[dim].category.index];
    if (dim === TIME_PERIOD_DIMENSION_KEY) {
      allValues.reverse();
    }
    const selectedVal = allValues.find(dimVal => selectableValues.includes(dimVal));
    filtersValue[dim] = selectedVal;
    iter = iter[selectedVal];
  });

  return filtersValue;
}

export const getInitialTableLayout = jsonStat => {
  if (!jsonStat) {
    return null;
  }

  let dimensions = [...jsonStat.id];

  let rows = dimensions.filter(dim => dim === TIME_PERIOD_DIMENSION_KEY);
  dimensions = dimensions.filter(dim => !rows.includes(dim));

  let territoryDim = (jsonStat.role?.geo || []).length > 0 ? jsonStat.role.geo[0] : null;

  let cols = territoryDim ? [territoryDim] : [];
  dimensions = dimensions.filter(dim => !cols.includes(dim));

  let filters = dimensions.filter(dim => jsonStat.size[jsonStat.id.indexOf(dim)] === 1);
  dimensions = dimensions.filter(dim => !filters.includes(dim));

  if (dimensions.includes(FREQ_DIMENSION_KEY)) {
    rows = [FREQ_DIMENSION_KEY, ...rows];
    dimensions = dimensions.filter(dim => !rows.includes(dim));
  }

  if (rows.length === 1 && jsonStat.size[jsonStat.id.indexOf(rows[0])] === 1 && cols.length === 1) {
    filters = filters.concat(rows);
    rows = [...cols];
    cols = [];
  }

  let colCount = 1;
  cols.forEach(col => colCount *= jsonStat.size[jsonStat.id.indexOf(col)]);
  let rowCount = 1;
  rows.forEach(row => rowCount *= jsonStat.size[jsonStat.id.indexOf(row)]);

  dimensions.forEach(dim => {
    const newColCount = colCount * jsonStat.size[jsonStat.id.indexOf(dim)];

    if (newColCount > MAX_COL_COUNT) {
      const newRowCount = rowCount * jsonStat.size[jsonStat.id.indexOf(dim)];

      if (newRowCount > MAX_ROW_COUNT) {
        filters.push(dim);

      } else {
        rows = [dim].concat(rows);
        rowCount = newRowCount;
      }

    } else {
      cols.push(dim);
      colCount = newColCount;
    }
  });

  return {
    rows: rows,
    cols: cols,
    sections: [],
    filters: filters
  }
};

export const getInitialChartLayout = jsonStat => {
  if (!jsonStat) {
    return null
  }

  let primaryDim = jsonStat.id.find(dim => dim === TIME_PERIOD_DIMENSION_KEY);
  if (!primaryDim) {
    primaryDim = jsonStat.id[0];
  }
  const filters = jsonStat.id.filter(dim => dim !== primaryDim);

  return {
    primaryDim: [primaryDim],
    primaryDimValues: jsonStat.dimension[primaryDim].category.index,
    secondaryDim: [],
    secondaryDimValues: [],
    filters: filters
  }
};

export const getInitialMapLayout = jsonStat => {
  if (!jsonStat) {
    return null
  }

  let territoryDim = (jsonStat.role?.geo || []).length > 0 ? jsonStat.role.geo[0] : null;

  if (!territoryDim) {
    return null
  }

  return {
    territoryDim: territoryDim,
    filters: jsonStat.id.filter(dim => dim !== territoryDim)
  }
};

const getFilteredLayout = (initialLayout, jsonStat) => {
  if (!initialLayout) {
    return null
  }

  const layout = _.cloneDeep(initialLayout);
  const dimArraysKey = ["rows", "cols", "filters", "sections", "primaryDim", "secondaryDim", "territoryDim"];

  if (layout.territoryDim) {
    layout.territoryDim = [layout.territoryDim];
  }

  // removing dimensions from layout not present in jsonStat
  for (let key in layout) {
    if (layout.hasOwnProperty(key)) {
      if (dimArraysKey.includes(key)) {
        layout[key] = (layout[key] || []).filter(dim => jsonStat.id.includes(dim));
      }
    }
  }

  // adding missing dimensions to layout
  jsonStat.id.forEach(dim => {
    let found = false;
    for (let key in layout) {
      if (layout.hasOwnProperty(key)) {
        if (!found && dimArraysKey.includes(key)) {
          found = layout[key].includes(dim);
        }
      }
    }
    if (!found) {
      layout.filters.push(dim);
      if (layout.filtersValue) {
        layout.filtersValue[dim] = jsonStat.dimension[dim].category.index[0];
      }
    }
  });

  // checking selected filtersValue
  if (layout.filtersValue) {
    for (let dim in layout.filtersValue) {
      if (layout.filtersValue.hasOwnProperty(dim)) {
        if (!layout.filters.includes(dim)) {
          layout.filtersValue[dim] = undefined;
        } else if (!jsonStat.dimension[dim].category.index.includes(layout.filtersValue[dim])) {
          layout.filtersValue[dim] = jsonStat.dimension[dim].category.index[0];
        }
      }
    }
  }

  // checking selected primaryDimValues
  if (layout.primaryDim) {
    if (layout.primaryDim.length === 1) {
      const primaryDim = layout?.primaryDim?.[0] || null;
      if (layout.primaryDimValues.length === 0) {
        layout.primaryDimValues = jsonStat.dimension[primaryDim].category.index;
      }
      layout.primaryDimValues = layout.primaryDimValues.filter(value =>
        jsonStat.dimension[primaryDim].category.index.includes(value));
    } else {
      return getInitialChartLayout(jsonStat)
    }
  }

  // checking selected secondaryDimValues
  const secondaryDim = layout?.secondaryDim?.[0] || null;
  if (secondaryDim) {
    if (layout.secondaryDimValues.length === 0) {
      layout.secondaryDimValues = jsonStat.dimension[secondaryDim].category.index;
    }
    layout.secondaryDimValues = layout.secondaryDimValues.filter(value =>
      jsonStat.dimension[secondaryDim].category.index.includes(value));
  }

  if (layout.territoryDim) {
    layout.territoryDim = layout.territoryDim[0];
  }

  return layout
};

export const getFilteredTableLayout = (initialLayout, jsonStat) => {
  if (!initialLayout) {
    return null
  }
  const layout = {
    rows: [],
    cols: [],
    sections: [],
    filters: [],
    ...initialLayout,
  };

  return getFilteredLayout(layout, jsonStat);
};

export const getFilteredMapLayout = (initialLayout, jsonStat) => {
  if (!initialLayout) {
    return null
  }
  const layout = {
    territoryDim: "",
    filters: [],
    ...initialLayout,
  };

  return getFilteredLayout(layout, jsonStat);
};

export const getFilteredChartLayout = (initialLayout, jsonStat) => {
  if (!initialLayout) {
    return null
  }
  const layout = {
    primaryDim: [],
    primaryDimValues: [],
    secondaryDim: [],
    secondaryDimValues: [],
    filters: [],
    ...initialLayout,
  };

  return getFilteredLayout(layout, jsonStat);
};

export const getUpdatedLayout = (dimension, value, jsonStat, layout, filterTree) => {
  let newLayout = _.cloneDeep(layout);
  const primaryDim = newLayout.primaryDim?.[0] || null;
  const secondaryDim = newLayout.secondaryDim?.[0] || null;

  let tmpLayout = _.cloneDeep(layout);
  if (primaryDim && dimension === primaryDim) {
    tmpLayout.primaryDimValues = value;
  } else if (secondaryDim && dimension === secondaryDim) {
    tmpLayout.secondaryDimValues = value;
  } else {
    tmpLayout.filtersValue[dimension] = value;
  }
  const filters = (tmpLayout?.primaryDim || []).concat((tmpLayout?.secondaryDim || [])).concat(tmpLayout?.filters || []);
  let isLayoutValid = false;
  let iter = filterTree;
  if (primaryDim && !isLayoutValid) {
    (tmpLayout.primaryDimValues || []).forEach(primDimVal => {
      if (secondaryDim && !isLayoutValid) {
        (tmpLayout.secondaryDimValues || []).forEach(secDimVal => {
          if (filterTree[primDimVal][secDimVal] !== null && filterTree[primDimVal][secDimVal] !== undefined) {
            iter = filterTree[primDimVal][secDimVal];
            let isFilterValid = true;
            filters.slice(2).forEach(val => {
              if (isFilterValid && iter[tmpLayout.filtersValue[val]] !== null && iter[tmpLayout.filtersValue[val]] !== undefined) {
                iter = iter[tmpLayout.filtersValue[val]];
              } else {
                isFilterValid = false;
              }
            });
            if (isFilterValid) {
              isLayoutValid = true;
            }
          }
        });
      } else {
        iter = filterTree[primDimVal];
        let isFilterValid = true;
        filters.slice(1).forEach(val => {
          if (isFilterValid && iter[tmpLayout.filtersValue[val]] !== null && iter[tmpLayout.filtersValue[val]] !== undefined) {
            iter = iter[tmpLayout.filtersValue[val]];
          } else {
            isFilterValid = false;
          }
        });
        if (isFilterValid) {
          isLayoutValid = true;
        }
      }
    });
  } else {
    let isFilterValid = true;
    filters.forEach(val => {
      if (isFilterValid && iter[tmpLayout.filtersValue[val]]) {
        iter = iter[tmpLayout.filtersValue[val]];
      } else {
        isFilterValid = false;
      }
    });
    if (isFilterValid) {
      isLayoutValid = true;
    }
  }

  if (isLayoutValid) {
    return tmpLayout

  } else {

    if (primaryDim && dimension === primaryDim) {
      newLayout.primaryDimValues = value;
      if (secondaryDim) {
        const allSecondaryDimValues = getDimensionFilterValues(secondaryDim, jsonStat, newLayout, filterTree);
        newLayout.secondaryDimValues = newLayout.secondaryDimValues.filter(val => allSecondaryDimValues.includes(val));
      }
      newLayout.filtersValue = getInitialFiltersValue(jsonStat, newLayout, filterTree);

    } else if (secondaryDim && dimension === secondaryDim) {
      newLayout.secondaryDimValues = value;
      newLayout.filtersValue = getInitialFiltersValue(jsonStat, newLayout, filterTree);

    } else {
      newLayout.filtersValue[dimension] = value;

      const filters = newLayout.filters || [];
      const selectedIdx = filters.indexOf(dimension);
      if (selectedIdx < (filters.length - 1)) {
        newLayout.filters.slice(selectedIdx + 1).forEach(dim => {
          const selectableValues = getDimensionFilterValues(dim, jsonStat, newLayout, filterTree);
          const allValues = [...jsonStat.dimension[dim].category.index];
          if (dim === TIME_PERIOD_DIMENSION_KEY) {
            allValues.reverse();
          }
          newLayout.filtersValue[dim] = allValues.find(dimVal => selectableValues.includes(dimVal));
        });
      }
    }

    return newLayout
  }
};

export const getDimensionFilterValues = (dimension, jsonStat, layout, filterTree, withLabels = false) => {
  const primaryDim = layout.primaryDim?.[0] || null;
  const secondaryDim = layout.secondaryDim?.[0] || null;
  const filters = (layout?.primaryDim || []).concat((layout?.secondaryDim || [])).concat(layout?.filters || []);
  const dimensionIdx = filters.indexOf(dimension);
  const prevFilters = filters.slice(0, dimensionIdx);

  let data = {};
  if (dimensionIdx > 0) {
    let iter = filterTree;
    if (primaryDim) {
      (layout.primaryDimValues || []).forEach(primDimVal => {
        if (secondaryDim && dimension !== secondaryDim) {
          (layout.secondaryDimValues || []).forEach(secDimVal => {
            iter = filterTree[primDimVal][secDimVal];
            prevFilters.slice(2).forEach(val => {
              iter = (iter || {})[layout.filtersValue[val]];
            });
            data = {...data, ...iter};
          });
        } else {
          iter = filterTree[primDimVal];
          prevFilters.slice(1).forEach(val => {
            iter = (iter || {})[layout.filtersValue[val]];
          });
          data = {...data, ...iter};
        }
      });
    } else {
      prevFilters.forEach(val => {
        iter = iter[layout.filtersValue[val]];
      });
      data = {...data, ...iter};
    }
  } else {
    data = filterTree;
  }

  const availableValue = Object.keys(data);
  const ret = [];
  jsonStat.dimension[dimension].category.index.forEach(val => {
    if (availableValue.includes(val)) {
      ret.push(withLabels
        ? {
          id: val,
          label: jsonStat.dimension[dimension].category.label[val]
        }
        : val
      );
    }
  });

  return ret;
}

export const getDimensionValuesIndexesMap = jsonStat => {
  const indexesMap = {};

  jsonStat.id.forEach(dim => {
    indexesMap[dim] = {};
    jsonStat.dimension[dim].category.index.forEach((dimValue, idx) => indexesMap[dim][dimValue] = idx);
  });

  return indexesMap;
};

export const getAttributeObjects = (attributeList, attributeObj, hiddenAttributes) => {
  const attributes = [];
  const ids = [];
  let hasAsterisk = false;

  try {
    (attributeList || []).forEach((attr, idx) => {
      if (attr !== null && !(hiddenAttributes || []).includes(attributeObj[idx].id)) {
        const valueId = attributeObj[idx].values[attr].id;
        const valueLabel = attributeObj[idx].values[attr].name;

        attributes.push({
          id: attributeObj[idx].id,
          label: attributeObj[idx].name,
          valueId: valueId,
          valueLabel: valueLabel
        });

        if (valueId !== valueLabel && valueId.length <= 5) {
          ids.push(valueId);
        } else {
          hasAsterisk = true;
        }
      }
    });

    if (hasAsterisk) {
      ids.push("(*)");
    }

    return (attributes.length > 0 && ids.length > 0)
      ? {
        attributes,
        ids
      }
      : null

  } catch (error) {
    return -1
  }
};

export const getDimensionAttributeMap = (jsonStat, hiddenAttributes, getIdsHtmlString) => {
  if (!jsonStat) {
    return null;
  }

  const series = (jsonStat?.extension?.attributes?.series || []);
  const seriesIndexes = (jsonStat?.extension?.attributes?.index?.series || []);
  const dimensionAttributesMap = {};

  jsonStat.id.forEach(dim => dimensionAttributesMap[dim] = {});
  if (series && series.length > 0 && seriesIndexes && seriesIndexes.length > 0) {
    seriesIndexes
      .filter(({coordinates}) => (coordinates || []).filter(c => c !== null).length === 1)
      .forEach(index => {
        const dimIdx = index.coordinates.findIndex(el => el !== null);
        const dim = jsonStat.id[dimIdx];
        const dimValueIdx = index.coordinates.find(el => el !== null);
        const dimValue = jsonStat.dimension[dim].category.index[dimValueIdx];

        const attributeObjects = getAttributeObjects(index.attributes, series, hiddenAttributes);
        if (attributeObjects !== null && attributeObjects !== -1) {
          dimensionAttributesMap[dim][dimValue] = {
            attributes: attributeObjects.attributes,
            ids: attributeObjects.ids,
            htmlString: getIdsHtmlString
              ? getIdsHtmlString(attributeObjects.ids, dim, dimValue)
              : null
          };
        }
      });
  }

  return dimensionAttributesMap;
};