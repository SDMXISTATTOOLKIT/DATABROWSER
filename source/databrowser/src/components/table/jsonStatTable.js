import {
  getAttributeObjects,
  getCoordinatesArrayFromDataIdx,
  getDataIdxFromCoordinatesArray,
  getDimensionAttributeMap,
  getDimensionLabelFromJsonStat,
  getDimensionValueLabelFromJsonStat
} from "../../utils/jsonStat";
import {v4 as uuidv4} from 'uuid';
import {getFormattedValue} from "../../utils/formatters";
import _ from "lodash";
import {getCombinationArrays, getNthValorizedElementIndexInBooleanArray} from "../../utils/other";

const JSONSTAT_TABLE_PREVIEW_PLACEHOLDER = "xxx";
const JSONSTAT_TABLE_SECTION_DIMENSIONS_SEPARATOR = '<svg height="6px" width="6px" fill="#00295a"><path d="M 0 0 H 6 V 6 H 0 Z"/></svg>';

const extendArr = (array, n) => {
  let arrays = Array.apply(null, new Array(n / array.length));
  arrays = arrays.map(() => array);
  return [].concat.apply([], arrays);
};

const getObj = (jsonStat, arr, length, isPreview) => {
  const obj = {};
  arr.forEach((el, idx) => {
    const repCount = idx === 0
      ? length / jsonStat.size[jsonStat.id.indexOf(el)]
      : obj[arr[idx - 1]].repCount / jsonStat.size[jsonStat.id.indexOf(el)];

    obj[el] = {
      repCount: repCount,
      ids: [],
      labels: [],
      indexesMap: {}
    };
    if (!isPreview) {
      jsonStat.dimension[el].category.index.forEach((value, idx) => {
        for (let j = 0; j < repCount; j++) {
          obj[el].ids.push(value);
          obj[el].labels.push(jsonStat.dimension[el].category.label[value]);
          obj[el].indexesMap[value] = idx;
        }
      })
    }
  });
  if (!isPreview) {
    arr.forEach(el => {
      if (obj[el].ids.length < length) {
        obj[el].ids = extendArr(obj[el].ids, length);
        obj[el].labels = extendArr(obj[el].labels, length);
      }
    });
  }

  return obj
};

const getValorizedElCount = array => {
  let count = 0;

  array.forEach(el => {
    if (el === true) {
      count++;
    }
  });

  return count;
};

const getJsonStatLayoutObjects = (jsonStat, rows, cols, sections, filters, filtersValue, isPreview) => {

  /** rows handling **/
  let sectionRowCount = 1;
  (rows || []).forEach(row => sectionRowCount *= jsonStat.size[jsonStat.id.indexOf(row)]);
  const rowsObj = (rows || []).length > 0
    ? getObj(jsonStat, rows, sectionRowCount, isPreview)
    : {};

  /** cols handling **/
  let colCount = 1;
  (cols || []).forEach(col => colCount *= jsonStat.size[jsonStat.id.indexOf(col)]);
  const colsObj = (cols || []).length > 0
    ? getObj(jsonStat, cols, colCount, isPreview)
    : {};

  /** sections handling **/
  const sectionArray = [];
  (sections || []).forEach(section => sectionArray.push(jsonStat.dimension[section].category.index));
  const combinations = sectionArray.length > 0
    ? getCombinationArrays(sectionArray)
    : [];

  /** indexes map handling **/
  const indexesMap = {};
  rows.forEach(row => indexesMap[row] = rowsObj[row].indexesMap);
  cols.forEach(col => indexesMap[col] = colsObj[col].indexesMap);
  sections.forEach(section => {
    indexesMap[section] = {};
    jsonStat.dimension[section].category.index.forEach((val, idx) => indexesMap[section][val] = idx);
  });
  filters.forEach(filter => {
    indexesMap[filter] = {};
    jsonStat.dimension[filter].category.index.forEach((val, idx) => indexesMap[filter][val] = idx);
  });

  return {
    sectionRowCount,
    rowsObj,
    colCount,
    colsObj,
    combinations,
    indexesMap
  }
};

const getCellAttributeIdsElem = (ids, htmlElemId, isDataCell) => {
  let idElems = "";
  const astElem = ' <span class="ct-a">(*)</span> ';

  if (!ids || ids.length === 0) {
    return "";

  } else {
    if (ids.length === 1 && ids[0].length <= 2) {
      idElems = ` <span class="${ids[0] === "(*)" ? 'ct-a' : 'ct-c'}">${ids[0]}</span> `;
    } else {
      idElems += astElem;
    }
  }

  return ` <span id="${htmlElemId}" class="ct ${isDataCell ? 'ctd' : 'ctsh'}">${idElems}</span> `
};

export const getJsonStatTableSupportStructures = (jsonStat, layout, isPreview, removeEmptyLines, hiddenAttributes) => {
  const {
    rows,
    cols,
    filters,
    filtersValue,
    sections
  } = layout;

  const {
    sectionRowCount,
    rowsObj,
    colCount,
    colsObj,
    combinations,
    indexesMap
  } = getJsonStatLayoutObjects(jsonStat, rows, cols, sections, filters, filtersValue, isPreview);

  /** empty rows & cols handling **/

  let valorizedRowsPerSection = (combinations && combinations.length > 0)
    ? combinations.map(() => [])
    : [[]];
  let valorizedCols = [];

  if (removeEmptyLines) {
    valorizedRowsPerSection = valorizedRowsPerSection.map(() => new Array(sectionRowCount).fill(false));
    valorizedCols = new Array(colCount).fill(false);

    const filterValueArray = jsonStat.id.map(dim => {
      if (filtersValue[dim]) {
        return indexesMap[dim][filtersValue[dim]]
      } else {
        return null
      }
    });
    Object.keys(jsonStat.value).forEach(key => {
      const coordinates = getCoordinatesArrayFromDataIdx(key, jsonStat.size);

      let sectionIdx = 0;
      if (sections.length > 0) {
        const sectionValueArray = [];
        coordinates.forEach((val, idx) => {
          const dim = jsonStat.id[idx];
          if (sections.includes(dim)) {
            sectionValueArray.push(jsonStat.dimension[dim].category.index[val]);
          }
        });
        sectionIdx = combinations.findIndex(combination => combination.join("+") === sectionValueArray.join("+"));
      }

      if (filterValueArray.filter((elem, idx) => elem !== null && elem !== coordinates[idx]).length === 0) {
        let y = 0;
        rows.forEach(row => {
          const val = coordinates[jsonStat.id.indexOf(row)];
          y += val * rowsObj[row].repCount;
        });

        let x = 0;
        cols.forEach(col => {
          const val = coordinates[jsonStat.id.indexOf(col)];
          x += val * colsObj[col].repCount;
        });

        valorizedRowsPerSection[sectionIdx][y] = true;
        valorizedCols[x] = true;
      }
    });

  } else {
    valorizedRowsPerSection = valorizedRowsPerSection.map(() => new Array(sectionRowCount).fill(true));
    valorizedCols = new Array(colCount).fill(true);
  }

  let valorizedRows;
  if (Array.prototype.flat) {
    valorizedRows = valorizedRowsPerSection.flat();
  } else {
    valorizedRows = [];
    valorizedRowsPerSection.forEach(section => valorizedRows = valorizedRows.concat(section));
  }

  /** dimension attributes handling **/
  const dimensionAttributesMap = getDimensionAttributeMap(
    jsonStat,
    hiddenAttributes,
    (ids, dim, dimValue) => getCellAttributeIdsElem(ids, `${dim},${dimValue}`, false)
  );

  /** observation attributes handling **/
  const observations = (jsonStat?.extension?.attributes?.observation || []);
  const observationIndexes = (jsonStat?.extension?.attributes?.index?.observation || {});
  const observationAttributesMap = _.cloneDeep(observationIndexes);

  let hasAttributeObservationError = false;
  for (let key in observationAttributesMap) {
    if (observationAttributesMap.hasOwnProperty(key)) {
      const attributeObjects = getAttributeObjects(observationAttributesMap[key], observations, hiddenAttributes);
      if (attributeObjects !== null && attributeObjects !== -1) {
        observationAttributesMap[key] = {
          attributes: attributeObjects.attributes,
          ids: attributeObjects.ids,
          htmlString: getCellAttributeIdsElem(attributeObjects.ids, key, true)
        };
      } else if (attributeObjects === -1) {
        hasAttributeObservationError = true;
      }
    }
  }

  return {
    rows,
    cols,
    sections,
    filters,
    filtersValue,
    colCount: getValorizedElCount(valorizedCols),
    rowCount: getValorizedElCount(valorizedRows),
    colsObj,
    rowsObj,
    combinations,
    indexesMap,
    valorizedCols,
    valorizedRows,
    valorizedRowsPerSection,
    sectionsLength: valorizedRowsPerSection[0].length,
    dimensionAttributesMap,
    observationAttributesMap,
    hasAttributeObservationError
  }
};

const getTableRightPadding = padding => padding
  ? `<td class="c c-rb" style="min-width: ${padding}px;">`
  : "";

export const getJsonStatTableHtml = (
  jsonStat,
  rows,
  cols,
  sections,
  filters,
  filtersValue,
  colCount,
  rowCount,
  colsObj,
  rowsObj,
  combinations,
  indexesMap,
  valorizedCols,
  valorizedRows,
  valorizedRowsPerSection,
  sectionsLength,
  labelFormat,
  fontSize,
  decimalSeparator,
  decimalPlaces,
  emptyChar,
  isPreview,
  paginationParams,
  dimensionAttributesMap,
  observationAttributesMap,
  onTimeSet
) => {

  /** pagination **/

  const origColStart = paginationParams ? paginationParams.colStart : 0;
  const origColEnd = paginationParams ? paginationParams.colEnd : colCount;

  const rowStart = paginationParams
    ? getNthValorizedElementIndexInBooleanArray(valorizedRows, paginationParams.rowStart)
    : getNthValorizedElementIndexInBooleanArray(valorizedRows, 0);

  const rowEnd = paginationParams
    ? getNthValorizedElementIndexInBooleanArray(valorizedRows, paginationParams.rowEnd)
    : getValorizedElCount(valorizedRows);

  const colStart = paginationParams
    ? getNthValorizedElementIndexInBooleanArray(valorizedCols, paginationParams.colStart)
    : getNthValorizedElementIndexInBooleanArray(valorizedCols, 0);

  const colEnd = paginationParams
    ? getNthValorizedElementIndexInBooleanArray(valorizedCols, paginationParams.colEnd)
    : getValorizedElCount(valorizedCols);

  const t0 = performance.now();

  /** HTML generating **/

  let table = `<table id="${uuidv4()}">`;

  /** table head **/

  table += '<thead>';

  cols.forEach((col, idx) => {
    table += `<tr data-row-key="h-${idx}">`;
    table += `<th class="c cf${fontSize} ch cl0" colspan="${rows.length}">${getDimensionLabelFromJsonStat(jsonStat, col, labelFormat)}</th>`;

    if (!isPreview) {

      let c = colStart;
      let colVisited = origColStart;
      while (colVisited < origColEnd) {

        const cc = c;
        let fullColSpan = colsObj[col].ids.slice(c, c + colsObj[col].repCount).filter(el => el === colsObj[col].ids[cc]).length;

        let colSpan = valorizedCols.slice(c, c + fullColSpan).filter(el => el === true).length;

        if (colVisited + colSpan > origColEnd) {
          colSpan = origColEnd - colVisited;
        }

        if (colSpan > 0) {

          const htmlString = (dimensionAttributesMap?.[col]?.[colsObj[col].ids[c]]?.htmlString || "");

          table += (
            `<th class="c cf${fontSize} csh ${htmlString.length > 0 ? 'ca' : ''}" colspan="${colSpan}">` +
            getDimensionValueLabelFromJsonStat(jsonStat, col, colsObj[col].ids[c], labelFormat) +
            htmlString +
            `</th>`
          );
        }

        c += fullColSpan;
        colVisited += colSpan;
      }

    } else {
      for (let c = 0; c < 3; c++) {
        table += `<th class="c cf${fontSize} csh" colspan="1">${JSONSTAT_TABLE_PREVIEW_PLACEHOLDER}</th>`;
      }
    }

    table += getTableRightPadding((paginationParams?.tableRightPadding || 1));

    table += '</tr>';
  });

  if (rows.length > 0) {
    table += `<tr data-row-key="hh">`;
    rows.forEach((row, idx) => table += `<th class="c cf${fontSize} ch cl${idx}">${getDimensionLabelFromJsonStat(jsonStat, row, labelFormat)}</th>`);
    if (!isPreview) {
      table += `<th class="c cf${fontSize} csh" colspan="${origColEnd - origColStart}"/>`;
    } else {
      table += `<th class="c cf${fontSize} csh" colspan="${cols.length > 0 ? 3 : 1}"/>`;
    }
    table += getTableRightPadding((paginationParams?.tableRightPadding || 1));
    table += '</tr>';
  }

  table += '</thead>';

  /** table body **/

  table += '<tbody id="body">';

  if (!isPreview) {

    const sectionsStarts = [0];
    valorizedRowsPerSection.forEach((valorizedRows, idx) => sectionsStarts.push(sectionsStarts[idx] + valorizedRows.length));

    const subHeaderHandled = {};
    rows.forEach(row => subHeaderHandled[row] = -1);

    let currentSectionIdx = 0;
    for (let i = 0; i < sectionsStarts.length; i++) {
      if (rowStart >= sectionsStarts[i] && (i === sectionsStarts.length - 1 || rowStart < sectionsStarts[i + 1])) {
        currentSectionIdx = i;
      }
    }

    const getSectionRow = currentSectionIdx => {
      let sectionRow = `<tr data-row-key="s-${currentSectionIdx}" class="rs">`;
      let sectionLabel = "";
      const currentSectionIdxClone = currentSectionIdx;
      sections.forEach((section, idx) => {

        const htmlString = (dimensionAttributesMap?.[section]?.[combinations[currentSectionIdxClone][idx]]?.htmlString || "");

        sectionLabel += (
          `<span class="${htmlString.length > 0 ? 'ca' : ''}" style="display: inline-block;">` +
          `<span class="cs-d">${getDimensionLabelFromJsonStat(jsonStat, section, labelFormat)}:</span> ${getDimensionValueLabelFromJsonStat(jsonStat, section, combinations[currentSectionIdxClone][idx], labelFormat)}` +
          htmlString +
          '</span>'
        );
        sectionLabel += idx < (sections.length - 1) ? `<span style="display: inline-block; margin: 0 8px">${JSONSTAT_TABLE_SECTION_DIMENSIONS_SEPARATOR}</span>` : '';
      });

      sectionRow += `<th class="c cf${fontSize} cs" colspan="${origColEnd - origColStart + (rows.length || 1)}">${sectionLabel}</th>`;

      sectionRow += getTableRightPadding((paginationParams?.tableRightPadding || 1));

      sectionRow += '</tr>';

      return sectionRow;
    }

    if (sections && sections.length > 0) {
      table += getSectionRow(currentSectionIdx);
    }

    for (let r = rowStart; r < rowEnd; r++) {

      if (sections && sections.length > 0) {
        if (r > rowStart && sectionsStarts.indexOf(r) !== -1) {
          currentSectionIdx = sectionsStarts.indexOf(r);
          table += getSectionRow(currentSectionIdx);
        }
      }

      if (valorizedRows[r] === true) {

        table += `<tr data-row-key="r-${r}">`;

        let subHeader = "";

        if (rows.length > 0) {
          for (let rr = 0; rr < rows.length; rr++) {

            const htmlString = (dimensionAttributesMap?.[rows[rr]]?.[rowsObj[rows[rr]].ids[r % sectionsLength]]?.htmlString || "");

            if (r > subHeaderHandled[rows[rr]]) {

              let rowSpan = valorizedRows.slice(r, r + (rowsObj[rows[rr]].repCount - (r % rowsObj[rows[rr]].repCount))).filter(el => el === true).length;
              if (r + rowSpan > rowEnd) {
                rowSpan = rowEnd - r;
              }

              subHeaderHandled[rows[rr]] = r + (rowsObj[rows[rr]].repCount - (r % rowsObj[rows[rr]].repCount)) - 1;

              if (rowSpan > 0) {
                subHeader += (
                  `<th class="c cf${fontSize} csh cl${rr} ${htmlString.length > 0 ? 'ca' : ''}" rowspan="${rowSpan}">` +
                  getDimensionValueLabelFromJsonStat(jsonStat, rows[rr], rowsObj[rows[rr]].ids[r % sectionsLength], labelFormat) +
                  htmlString +
                  `</th>`
                );
              }
            }
          }

        } else {
          subHeader += `<th class="c cf${fontSize} csh cl0"/>`
        }

        table += subHeader;

        const getDataCell = (c, currentSectionIdx) => {

          const dataObj = {
            ...filtersValue
          };
          rows.forEach(row => dataObj[row] = rowsObj[row].ids[r % sectionsLength]);
          cols.forEach(col => dataObj[col] = colsObj[col].ids[c]);
          sections.forEach((section, idx) => dataObj[section] = combinations[currentSectionIdx][idx]);

          const dataIndexArr = jsonStat.id.map(dim => indexesMap[dim][dataObj[dim]]);

          const dataIdx = getDataIdxFromCoordinatesArray(dataIndexArr, jsonStat.size);
          const value = jsonStat.value[dataIdx];

          const htmlString = (observationAttributesMap[dataIdx]?.htmlString || "");

          return (
            `<td id="${dataIdx}" class="c cf${fontSize} ${htmlString.length > 0 ? 'ca' : ''}">` +
            htmlString +
            getFormattedValue(value, decimalSeparator, decimalPlaces, emptyChar) +
            `</td>`
          );
        }

        if (valorizedRows[r] === true) {
          for (let c = colStart; c < colEnd; c++) {
            if (valorizedCols[c] === true) {
              table += getDataCell(c, currentSectionIdx);
            }
          }

        } else {
          for (let c = colStart; c < colEnd; c++) {
            table += getDataCell(c, currentSectionIdx);
          }
        }

        table += getTableRightPadding((paginationParams?.tableRightPadding || 1));

        table += '</tr>';
      }

    }

    table += `<tr><td class="c c-bb" colspan="${rows.length + (origColEnd - origColStart + (rows.length > 0 ? 0 : 1))}"></td></tr>`;

  } else {

    if (sections && sections.length > 0) {
      table += `<tr data-row-key="s-0" class="rs">`;
      let sectionLabel = "";
      sections.forEach((section, idx) => {
        sectionLabel += `<span style="display: inline-block;"><span class="cs-d">${getDimensionLabelFromJsonStat(jsonStat, section, labelFormat)}:</span> ${JSONSTAT_TABLE_PREVIEW_PLACEHOLDER}</span>`;
        sectionLabel += idx < (sections.length - 1) ? `<span style="display: inline-block; margin: 0 8px">${JSONSTAT_TABLE_SECTION_DIMENSIONS_SEPARATOR}</span>` : '';
      });
      table += `<th class="c cf${fontSize} cs" colspan="${(rows.length || 1) + (cols.length > 0 ? 3 : 1)}">${sectionLabel}</th>`;
      table += getTableRightPadding(1);
      table += '</tr>';
    }
    for (let r = 0; r < (rows.length > 0 ? 3 : 1); r++) {
      table += `<tr data-row-key="r-${r}">`;
      if (rows.length > 0) {
        for (let rr = 0; rr < rows.length; rr++) {
          table += `<th class="c cf${fontSize} csh cl${rr}">xxx</th>`;
        }
      } else {
        table += `<th class="c cf${fontSize} csh cl0">&nbsp;</th>`
      }
      for (let c = 0; c < (cols.length > 0 ? 3 : 1); c++) {
        table += `<td class="c cf${fontSize}"/>`;
      }
      table += getTableRightPadding(1);
      table += '</tr>';
    }
    table += `<tr><td class="c c-bb" colspan="${(rows.length || 1) + (cols.length > 0 ? 3 : 1)}"></td></tr>`;
  }

  table += '</tbody>';

  table += '</table>';

  table += '<div id="jsonstat-table__tooltip" class="ctt"/>'

  const t1 = performance.now();
  if (!isPreview) {
    onTimeSet(Math.round((t1 - t0) * 100) / 100);
  }

  return table;
};