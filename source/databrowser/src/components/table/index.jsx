import React, {Fragment, useCallback, useEffect, useState} from 'react';
import {compose} from "redux";
import {connect} from "react-redux";
import {v4 as uuidv4} from 'uuid';
import withStyles from "@material-ui/core/styles/withStyles";
import {getJsonStatTableHtml} from "./jsonStatTable";
import "./style.css"
import {setDatasetHtmlGeneratingTime} from "../../state/dataset/datasetActions";
import Scrollbars, {SLIDER_WIDTH} from "./Scrollbars";
import {addSpinnerMessage, markSpinnerMessage} from "../../state/spinner/spinnerActions";
import copy from "copy-to-clipboard";
import Snackbar from '@material-ui/core/Snackbar';
import Alert from '@material-ui/lab/Alert';
import Dialog from "@material-ui/core/Dialog";
import DialogTitle from "@material-ui/core/DialogTitle";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import Grid from "@material-ui/core/Grid";
import {useTranslation} from "react-i18next";
import {getTextWidth} from "../../utils/style";
import {LABEL_FORMAT_SELECTOR_LABEL_FORMAT_NAME} from "../label-format-selector/constants";
import CustomEmpty from "../custom-empty";
import {sanitize} from "dompurify";

const $ = window.jQuery;

export const JSONSTAT_TABLE_FONT_SIZE_SM = "s";
export const JSONSTAT_TABLE_FONT_SIZE_MD = "m";
export const JSONSTAT_TABLE_FONT_SIZE_LG = "l";

const COLS_PER_PAGE = 30;
const ROWS_PER_PAGE = 50;

const SLIDER_SAFETY_MARGIN_PERCENTAGE = 20;

const TABLE_RIGHT_PADDING_CELL_WIDTH = 100;

let isFirstRender = false;
let isTimeToUpdate = false;

const styles = theme => ({
  root: {
    width: "100%",
    height: "100%"
  },
  table: {
    width: "100%",
    height: "100%"
  }
});

const mapDispatchToProps = dispatch => ({
  onTimeSet: time => dispatch(setDatasetHtmlGeneratingTime(time)),
  addSpinner: (uuid, message) => dispatch(addSpinnerMessage(uuid, message)),
  removeSpinner: (uuid, isError) => dispatch(markSpinnerMessage(uuid, isError)),
});

const isElementInContainer = (el, container, isVerticalScrollbarVisible, isHorizontalScrollbarVisible, checkOnlyVertical) => {
  const containerRect = container.getBoundingClientRect();
  const elementRect = el.getBoundingClientRect();

  return (
    elementRect.top >= containerRect.top &&
    elementRect.bottom <= (containerRect.bottom - (isHorizontalScrollbarVisible ? SLIDER_WIDTH : 0)) &&
    (checkOnlyVertical === true || (
      elementRect.left >= containerRect.left &&
      elementRect.right <= (containerRect.right - (isVerticalScrollbarVisible ? SLIDER_WIDTH : 0))
    ))
  );
}

const getAttributesValueString = (valueId, valueLabel) => `${(valueLabel || valueId)}${(valueLabel !== valueId) ? ` [${valueId}]` : ''}`

function JsonStatTable(props) {
  const {
    classes,
    jsonStat,
    layout,
    labelFormat = LABEL_FORMAT_SELECTOR_LABEL_FORMAT_NAME,
    fontSize,
    isFullscreen,
    isPreview = false,
    removeEmptyLines = !isPreview,
    isPaginated = !isPreview,
    isStickyHeader = true,
    decimalSeparator,
    decimalPlaces,
    emptyChar,
    hiddenAttributes,
    addSpinner,
    removeSpinner,
    hideSpinner = false,
    disableWheelZoom = false,
    onTimeSet
  } = props;

  const {t} = useTranslation();

  const [uuid] = useState(uuidv4());
  const [spinnerUuid] = useState(uuidv4());

  const [tableSupportStructures, setTableSupportStructures] = useState(null);
  const [htmlTable, setHtmlTable] = useState("");

  const [row, setRow] = useState(0);
  const [col, setCol] = useState(0);

  const [visibleRowCount, setVisibleRowCount] = useState(1);
  const [visibleColCount, setVisibleColCount] = useState(1);

  const [tableRightPadding, setTableRightPadding] = useState(0);

  const [isHorizontalScrollbarVisible, setHorizontalScrollbarVisibility] = useState(null);
  const [isVerticalScrollbarVisible, setVerticalScrollbarVisibility] = useState(null);

  const [attributes, setAttributes] = useState(null);
  const [isCopiedVisible, setCopiedVisibility] = useState(false);

  const [worker] = useState(() => new Worker("./workers/getJsonStatTableSupportStructuresWorker.js"));

  useEffect(() => {
    return () => {
      if (worker) {
        worker.terminate();
      }
    }
  }, [worker]);

  const handleStyle = useCallback(
    (isFirstRender, isResizing) => {
      if (!isPreview && tableSupportStructures) {

        if (!isResizing) {

          /** attribute's tooltip handling **/
          const $tooltip = $(`.jsonstat-table__${uuid} #jsonstat-table__tooltip`);
          $(`.jsonstat-table__${uuid} .ca .ct`)
            .hover(
              function () {
                const $elem = $(this).get(0);
                const rect = $elem.getBoundingClientRect();

                const attributes = $elem.className.includes("ctd")
                  ? tableSupportStructures.observationAttributesMap[$elem.id].attributes
                  : tableSupportStructures.dimensionAttributesMap[$elem.id.split(",")[0]][$elem.id.split(",")[1]].attributes;

                const ATTRIBUTE_HEIGHT = 18;

                attributes.forEach(({id, label, valueId, valueLabel}) => {
                  $tooltip.append($(`<li class="cttt"><b>${(label || id)}</b>: ${getAttributesValueString(valueId, valueLabel)}</li>`))
                })

                const left = rect.x < (window.innerWidth / 2)
                  ? rect.left
                  : rect.left - $tooltip.innerWidth();

                $tooltip.css({
                  visibility: "visible",
                  top: rect.top - (ATTRIBUTE_HEIGHT * attributes.length) - 30,
                  left: left
                });
              },
              function () {
                $tooltip.empty().css({visibility: "hidden"});
              }
            )
            .click(
              function () {
                const $elem = $(this).get(0);

                const attributes = $elem.className.includes("ctd")
                  ? tableSupportStructures.observationAttributesMap[$elem.id].attributes
                  : tableSupportStructures.dimensionAttributesMap[$elem.id.split(",")[0]][$elem.id.split(",")[1]].attributes;

                setAttributes(attributes);
              }
            );

          /** header's height handling **/
          const cell = $(`.jsonstat-table__${uuid} th.c:first`);
          if ((cell.css("minWidth") || "") === "" || (cell.css("minWidth") || "") === "0px") {

            const cellFontSize = cell.css("fontSize")
            const cellPaddingLeft = Number(cell.css("paddingLeft").split("px")[0])
            const cellPaddingRight = Number(cell.css("paddingRight").split("px")[0])

            const getTextWidthEl = $('<span>').css({
              visibility: 'hidden',
              position: 'absolute',
              fontSize: cellFontSize
            }).appendTo('body').get(0);

            $(`.jsonstat-table__${uuid} table thead th`).each(function () {
              const minWidth = getTextWidth($(this).text(), getTextWidthEl);
              $(this).css({minWidth: ((minWidth / 2) + cellPaddingLeft + cellPaddingRight + 24)})
            })

            $(getTextWidthEl).remove();
          }
        }

        if (!isPaginated) {

          const stickyRowCount = (layout.cols.length + 1);
          const stickyColCount = layout.rows.length;

          if (isStickyHeader) {

            /** header **/
            const tops = [0];
            const $headerRows = $(`.jsonstat-table__${uuid} thead tr`);
            for (let i = 0; i < $headerRows.length; i++) {
              tops.push(tops[i] + $headerRows[i].getBoundingClientRect().height);
            }
            for (let i = 0; i < (tops.length - 1); i++) {
              $(`.jsonstat-table__${uuid} thead tr:nth-child(${i + 1}) th`).css("top", tops[i]);
            }

            /** columns **/
            const lefts = [0];
            const $firstBodyRowThs = $(`.jsonstat-table__${uuid} tbody tr:not(.rs):first th`);
            for (let i = 0; i < $firstBodyRowThs.length; i++) {
              lefts.push(lefts[i] + $firstBodyRowThs[i].getBoundingClientRect().width);
            }
            for (let i = 0; i < (lefts.length - 1); i++) {
              // the class "clX" is assigned during the html generation to the x-th column for each row, if this column have to be sticky
              $(`.jsonstat-table__${uuid} th.cl${i}`).css("left", lefts[i]);
            }

            /** sections **/
            $(`.jsonstat-table__${uuid} .cs`).css({top: tops[tops.length - 1], left: 0});

          } else {

            if (!isFirstRender) {

              /** header **/
              for (let i = 0; i < stickyRowCount; i++) {
                $(`.jsonstat-table__${uuid} thead tr:nth-child(${i + 1}) th`).css("top", "auto");
              }

              /** columns **/
              for (let i = 0; i < stickyColCount; i++) {
                // the class "clX" is assigned during the html generation to the x-th column for each row, if this column have to be sticky
                $(`.jsonstat-table__${uuid} th.cl${i}`).css("left", "auto");
              }

              /** sections **/
              $(`.jsonstat-table__${uuid} .cs`).css({top: "auto", left: "auto"});
            }
          }
        }

        removeSpinner(spinnerUuid);
      }
    },
    [uuid, tableSupportStructures, layout, isPreview, isStickyHeader, isPaginated, removeSpinner, spinnerUuid]
  );

  const handleScrollbar = useCallback(
    updateVisibleRowAndColCount => {
      let $tableContainer = $(`.jsonstat-table__${uuid}`);

      if ($tableContainer && tableSupportStructures) {

        let isVerticalScrollbarVisible = null;
        let isHorizontalScrollbarVisible = null;

        const {rowCount, colCount} = tableSupportStructures;
        const $table = $(`.jsonstat-table__${uuid} table`);

        setVerticalScrollbarVisibility(prevIsVerticalScrollbarVisible => {
          isVerticalScrollbarVisible = (row !== null && row !== undefined && row !== 0)
            ? prevIsVerticalScrollbarVisible
            : ((rowCount > ROWS_PER_PAGE) || (rowCount <= ROWS_PER_PAGE && $table.height() > ($tableContainer.height() - 40))); // 40 pixel for horizontal scrollbar
          return isVerticalScrollbarVisible;
        });

        setHorizontalScrollbarVisibility(prevIsHorizontalScrollbarVisible => {
          isHorizontalScrollbarVisible = (col !== null && col !== undefined && col !== 0)
            ? prevIsHorizontalScrollbarVisible
            : ((colCount > COLS_PER_PAGE) || (colCount <= COLS_PER_PAGE && ($table.width() - tableRightPadding) > ($tableContainer.width() - 40))); // 40 pixel for vertical scrollbar
          return isHorizontalScrollbarVisible;
        });

        let visibleColCount = 0;
        $(`.jsonstat-table__${uuid} tbody tr:not(.rs):first td`).each((idx, el) => {
          if (isElementInContainer(el, $tableContainer[0], isVerticalScrollbarVisible, isHorizontalScrollbarVisible)) {
            visibleColCount++;
          }
        });
        if (updateVisibleRowAndColCount) {
          setVisibleColCount(visibleColCount);
        }

        let visibleRowCount = 0;
        $(`.jsonstat-table__${uuid} tbody tr:not(.rs)`).each((idx, el) => {
          if (isElementInContainer($(el).children("td")[0], $tableContainer[0], isVerticalScrollbarVisible, isHorizontalScrollbarVisible)) {
            visibleRowCount++;
          }
        });
        $(`.jsonstat-table__${uuid} tbody tr.rs`).each((idx, el) => {
          if (isElementInContainer(el, $tableContainer[0], isVerticalScrollbarVisible, isHorizontalScrollbarVisible, true)) {
            visibleRowCount++;
          }
        });
        if (updateVisibleRowAndColCount) {
          setVisibleRowCount(visibleRowCount);
        }
      }
    },
    [uuid, tableSupportStructures, tableRightPadding, row, col]
  );

  useEffect(() => {
    if (jsonStat && layout) {
      isTimeToUpdate = false;
      setTableSupportStructures(null);
      setHtmlTable("");

      if (!hideSpinner && !isPreview) {
        addSpinner(spinnerUuid, t("components.table.spinners.rendering"));
      }

      worker.onmessage = event => {
        isTimeToUpdate = true;
        setTableSupportStructures(event.data);
        if (event.data.hasAttributeObservationError) {
          window.error.show(t("components.table.error.observationAttribute"));
        }
      };
      worker.onerror = () => {
        setTableSupportStructures(null);
        removeSpinner(spinnerUuid, true);
      }
      worker.postMessage({
        jsonStat,
        layout,
        isPreview,
        removeEmptyLines,
        hiddenAttributes
      });

      setRow(0);
      setCol(0);
      isFirstRender = true;
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [jsonStat, layout, isPreview, removeEmptyLines, hideSpinner, addSpinner, removeSpinner, spinnerUuid, t]); // TODO: include "hiddenAttributes" in deps array

  useEffect(() => {
    if (isTimeToUpdate && tableSupportStructures) {
      const func = () => {
        handleStyle(false, true);
        handleScrollbar(true);
      }
      window.addEventListener("resize", func);
      return () => window.removeEventListener("resize", func);
    }
  }, [tableSupportStructures, handleStyle, handleScrollbar]);

  useEffect(() => {
    if (isTimeToUpdate && tableSupportStructures) {
      const {
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
        dimensionAttributesMap,
        observationAttributesMap
      } = tableSupportStructures;

      const rowEnd = (row + ROWS_PER_PAGE) >= rowCount ? rowCount : (row + ROWS_PER_PAGE);
      const colEnd = (col + COLS_PER_PAGE) >= colCount ? colCount : (col + COLS_PER_PAGE);

      const tableRightPadding = (COLS_PER_PAGE - (colEnd - col)) * TABLE_RIGHT_PADDING_CELL_WIDTH;
      setTableRightPadding(tableRightPadding);

      setHtmlTable(getJsonStatTableHtml(
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
        (fontSize || JSONSTAT_TABLE_FONT_SIZE_MD),
        decimalSeparator,
        decimalPlaces,
        sanitize(emptyChar),
        isPreview,
        isPaginated
          ? {
            rowStart: row,
            rowEnd: rowEnd,
            colStart: col,
            colEnd: colEnd,
            tableRightPadding: !isPreview ? tableRightPadding : 1
          }
          : null,
        dimensionAttributesMap,
        observationAttributesMap,
        onTimeSet
      ));

      $(`.jsonstat-table__${uuid}`).scrollTop(0).scrollLeft(0);
    }
  }, [uuid, tableSupportStructures, jsonStat, labelFormat, fontSize, isFullscreen, decimalSeparator, decimalPlaces, emptyChar, isPreview, isPaginated, onTimeSet, row, col]);

  useEffect(() => {
    if (isTimeToUpdate && htmlTable && htmlTable.length > 0) {
      handleStyle(isFirstRender, false);
      handleScrollbar(isFirstRender);

      isFirstRender = false;
    }
  }, [htmlTable, handleStyle, handleScrollbar]);

  return (
    <Fragment>
      {tableSupportStructures
        ? (
          <div className={classes.root}>
            {isPaginated
              ? (
                <Scrollbars
                  verticalValue={row}
                  verticalMaxValue={tableSupportStructures.rowCount}
                  verticalTicks={tableSupportStructures.rowCount - (visibleRowCount - Math.floor(visibleRowCount / 100 * SLIDER_SAFETY_MARGIN_PERCENTAGE))}
                  onVerticalScroll={setRow}
                  isVerticalScrollbarVisible={isVerticalScrollbarVisible}
                  horizontalValue={col}
                  horizontalMaxValue={tableSupportStructures.colCount}
                  horizontalTicks={tableSupportStructures.colCount - (visibleColCount - Math.floor(visibleColCount / 100 * SLIDER_SAFETY_MARGIN_PERCENTAGE))}
                  onHorizontalScroll={setCol}
                  isHorizontalScrollbarVisible={isHorizontalScrollbarVisible}
                  disableWheelZoom={disableWheelZoom}
                >
                  <div
                    className={`${classes.table} jsonstat-table jsonstat-table__${uuid}`}
                    style={{overflow: "hidden"}}
                    dangerouslySetInnerHTML={{__html: htmlTable}}
                  />
                </Scrollbars>
              )
              : (
                <div
                  className={`${classes.table} jsonstat-table jsonstat-table__${uuid}`}
                  style={{overflow: "auto"}}
                  dangerouslySetInnerHTML={{__html: htmlTable}}
                />
              )
            }
          </div>
        )
        : (
          <CustomEmpty text={t("components.table.initializing") + "..."}/>
        )
      }
      <Snackbar
        open={isCopiedVisible}
        autoHideDuration={2000}
        onClose={() => setCopiedVisibility(prevVal => !prevVal)}
      >
        <Alert severity="success" onClose={() => setCopiedVisibility(prevVal => !prevVal)}>
          {t("components.table.alerts.attributeCopied")}
        </Alert>
      </Snackbar>
      <Dialog
        open={attributes !== null}
        onClose={() => setAttributes(null)}
      >
        <DialogTitle>
          {t("components.table.dialogs.observationAttributes.title")}
        </DialogTitle>
        <DialogContent style={{width: 480}}>
          <Grid container spacing={2}>
            {(attributes || []).map(({id, label, valueId, valueLabel}, idx) =>
              <Grid item key={idx} xs={12}>
                <b>{label || id}</b>{`: ${getAttributesValueString(valueId, valueLabel)}`}
              </Grid>
            )}
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setAttributes(null)}>
            {t("commons.confirm.close")}
          </Button>
          <Button
            color="primary"
            autoFocus
            onClick={() => {
              copy(attributes
                .map(({id, label, valueId, valueLabel}) => `${(label || id)}: ${getAttributesValueString(valueId, valueLabel)}`)
                .join(", ")
              );
              setCopiedVisibility(prevVal => !prevVal)();
            }}
          >
            {t("commons.confirm.copy")}
          </Button>
        </DialogActions>
      </Dialog>
    </Fragment>
  )
}

export default compose(
  connect(null, mapDispatchToProps),
  withStyles(styles)
)(JsonStatTable);