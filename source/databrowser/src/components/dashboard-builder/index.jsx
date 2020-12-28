import React, {Fragment, useState} from "react";
import {compose} from "redux";
import {connect} from "react-redux";
import withStyles from "@material-ui/core/styles/withStyles";
import _ from "lodash";
import Grid from "@material-ui/core/Grid";
import FormControl from "@material-ui/core/FormControl";
import TextField from "@material-ui/core/TextField";
import Button from "@material-ui/core/Button";
import IconButton from "@material-ui/core/IconButton";
import AddIcon from '@material-ui/icons/Add';
import CloseIcon from '@material-ui/icons/Close';
import DeleteIcon from '@material-ui/icons/Delete';
import EditIcon from '@material-ui/icons/Edit';
import Dialog from "@material-ui/core/Dialog";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import DialogTitle from "@material-ui/core/DialogTitle";
import Tooltip from "@material-ui/core/Tooltip";
import InfiniteScrollTable from "../infinite-scroll-table";
import Selector from "../selector";
import {
  DASHBOARD_ELEM_ENABLE_FILTERS_KEY,
  DASHBOARD_ELEM_FILTER_DIMENSION_KEY,
  DASHBOARD_ELEM_SHOW_TITLE_KEY,
  DASHBOARD_ELEM_TYPE_KEY,
  DASHBOARD_ELEM_TYPE_VALUE_TEXT,
  DASHBOARD_ELEM_TYPE_VALUE_VIEW,
  DASHBOARD_ELEM_VALUE_KEY,
  DASHBOARD_ELEM_WIDTH_KEY,
  emptyDashboardElem
} from "../../utils/dashboards";
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Checkbox from '@material-ui/core/Checkbox';
import I18nDelayedTextField from "../i18n-delayed-text-field";
import {useTranslation} from "react-i18next";
import I18nHtmlEditor from "../i18n-html-editor";
import {localizeI18nObj} from "../../utils/i18n";
import filters from "../../dummy/filters.json";
import {getDashboardDynamicFilterLabelTranslations} from "../../constants/getDashboardDynamicFilterLabelTranslations";

const $ = window.jQuery;

const MAX_VIEW_PER_ROW = 4;

const ROW_ACTION_WIDTH = 48;
const COL_ACTION_WIDTH = 48;
const VIEW_ACTION_WIDTH = 48;

const styles = theme => ({
  filterLevels: {
    fontSize: 16
  },
  rowContainer: {
    width: "100%",
    marginTop: 8
  },
  row: {
    width: `calc(100% - ${2 * ROW_ACTION_WIDTH}px)`,
    height: 164,
    display: "inline-block"
  },
  rowActions: {
    width: (2 * ROW_ACTION_WIDTH),
    display: "inline-block",
    margin: "5px 0"
  },
  rowAction: {
    width: ROW_ACTION_WIDTH
  },
  colContainer: {
    border: "1px solid rgba(0, 0, 0, 0.2)",
    marginRight: 8,
    padding: 16,
    height: "100%",
    display: "inline-block",
    verticalAlign: "middle"
  },
  col: {
    width: `calc(100% - ${COL_ACTION_WIDTH}px)`,
    display: "inline-block",
    verticalAlign: "middle"
  },
  colActions: {
    width: COL_ACTION_WIDTH,
    display: "inline-block",
    verticalAlign: "middle"
  },
  viewContainer: {
    width: "100%"
  },
  view: {
    width: `calc(100% - ${VIEW_ACTION_WIDTH + 4}px)`,
    display: "inline-block",
    verticalAlign: "middle"
  },
  viewAction: {
    width: VIEW_ACTION_WIDTH,
    marginLeft: 4,
    display: "inline-block",
    verticalAlign: "middle"
  },
  input: {
    width: "100%",
    "& > div": {
      width: "100%"
    },
    "& input": {
      padding: 10,
      height: 20
    },
    display: "inline-block",
    verticalAlign: "middle"
  },
  dynamicViewInput: {
    marginLeft: 16,
    "& input": {
      padding: 10,
      height: 20
    }
  },
  htmlInput: {
    "& > div": {
      height: 130
    }
  },
  formLabel: {
    marginLeft: 0,
    marginRight: 0
  }
});

const getStrippedHtmlText = html => {
  const $span = $('<span>').get(0);

  $span.innerHTML = html;
  const text = $span.textContent || $span.innerText || "";

  $($span).remove();

  return text;
}

const DashboardBuilderCol = ({
                               t,
                               classes,
                               defaultLanguage,
                               languages,
                               dashboard,
                               rowIdx,
                               colIdx,
                               col,
                               onColRemove,
                               onTypeReset,
                               fetchViews,
                               onViewReset,
                               onTextEdit,
                               onShowTitleChange,
                               onEnableFiltersChange,
                               onFilterDimensionChange
                             }) =>
  <div
    className={classes.colContainer}
    style={{width: `calc(${col[DASHBOARD_ELEM_WIDTH_KEY]}% - ${2 * 4}px)`}}
  >
    <div className={classes.col}>
      {col[DASHBOARD_ELEM_TYPE_KEY] === null
        ? (
          <Grid container justify="center" style={{margin: "47px 0"}}>
            <Grid item>
              <Button onClick={() => fetchViews(rowIdx, colIdx)}>
                {t("components.dashboardBuilder.actions.addView")}
              </Button>
            </Grid>
            <Grid item>
              <Button onClick={() => onTextEdit(rowIdx, colIdx, "")}>
                {t("components.dashboardBuilder.actions.addText")}
              </Button>
            </Grid>
          </Grid>
        )
        : (
          <div className={classes.viewContainer}>
            <div className={classes.view}>
              {col[DASHBOARD_ELEM_TYPE_KEY] === DASHBOARD_ELEM_TYPE_VALUE_VIEW
                ? (
                  <Grid container>
                    <Grid item xs={12} style={{marginBottom: 8}}>
                      <Selector
                        value={col[DASHBOARD_ELEM_VALUE_KEY] ? dashboard.views[col[DASHBOARD_ELEM_VALUE_KEY]] : null}
                        render={view => view?.title ? localizeI18nObj(view.title, defaultLanguage, languages) : ""}
                        variant="standard"
                        className={classes.input}
                        selectTitle={t("components.dashboardBuilder.actions.selectView")}
                        onSelect={() => fetchViews(rowIdx, colIdx)}
                        resetTitle={t("components.dashboardBuilder.actions.deselectView")}
                        onReset={() => onViewReset(rowIdx, colIdx, col[DASHBOARD_ELEM_VALUE_KEY])}
                      />
                    </Grid>
                    <Grid item xs={12}>
                      <Grid container>
                        <Grid item style={{marginRight: 8}}>
                          <FormControlLabel
                            className={classes.formLabel}
                            label={t("components.dashboardBuilder.actions.showTitle")}
                            control={
                              <Checkbox
                                style={{padding: 5}}
                                checked={col[DASHBOARD_ELEM_SHOW_TITLE_KEY]}
                                onChange={(ev, checked) => onShowTitleChange(rowIdx, colIdx, checked)}
                              />
                            }
                          />
                        </Grid>
                        <Grid item>
                          <FormControlLabel
                            className={classes.formLabel}
                            label={t("components.dashboardBuilder.actions.enableFilters")}
                            control={
                              <Checkbox
                                style={{padding: 5}}
                                checked={col[DASHBOARD_ELEM_ENABLE_FILTERS_KEY]}
                                onChange={(ev, checked) => onEnableFiltersChange(rowIdx, colIdx, checked)}
                              />
                            }
                          />
                        </Grid>
                      </Grid>
                    </Grid>
                    <Grid item xs={12} style={{marginTop: 8}}>
                      <FormControlLabel
                        className={classes.formLabel}
                        style={{marginLeft: 8}}
                        label={t("components.dashboardBuilder.filterDimension.label") + ":"}
                        labelPlacement="start"
                        control={
                          <TextField
                            value={col[DASHBOARD_ELEM_FILTER_DIMENSION_KEY] || ""}
                            placeholder={t("components.dashboardBuilder.filterDimension.noOne")}
                            className={classes.dynamicViewInput}
                            variant="outlined"
                            onChange={({target}) => onFilterDimensionChange(rowIdx, colIdx, target.value)}
                          />
                        }
                      />
                    </Grid>
                  </Grid>
                )
                : (
                  <TextField
                    value={getStrippedHtmlText(localizeI18nObj(col[DASHBOARD_ELEM_VALUE_KEY], defaultLanguage, languages) || "")}
                    className={`${classes.input} ${classes.htmlInput}`}
                    multiline
                    rows={5}
                    InputProps={{
                      readOnly: true,
                      endAdornment: (
                        <Tooltip title={t("components.dashboardBuilder.actions.editText")}>
                          <EditIcon
                            style={{cursor: "pointer", marginLeft: 4}}
                            onClick={() => onTextEdit(rowIdx, colIdx, col[DASHBOARD_ELEM_VALUE_KEY])}
                          />
                        </Tooltip>
                      )
                    }}
                  />
                )
              }
            </div>
            <Tooltip title={t("components.dashboardBuilder.actions.resetColumn")}>
              <IconButton
                className={classes.viewAction}
                onClick={() => onTypeReset(rowIdx, colIdx)}
              >
                <CloseIcon/>
              </IconButton>
            </Tooltip>
          </div>
        )
      }
    </div>
    <Tooltip title={t("components.dashboardBuilder.actions.removeColumn")}>
      <span>
        <IconButton
          className={classes.colActions}
          onClick={() => onColRemove(rowIdx, colIdx)}
          disabled={dashboard.dashboardConfig[rowIdx].length === 1}
        >
          <DeleteIcon/>
        </IconButton>
      </span>
    </Tooltip>
  </div>;

const DashboardBuilderRow = ({
                               t,
                               classes,
                               defaultLanguage,
                               languages,
                               dashboard,
                               row,
                               rowIdx,
                               onRowRemove,
                               onColAdd,
                               onColRemove,
                               onTypeReset,
                               fetchViews,
                               onViewReset,
                               onTextEdit,
                               onShowTitleChange,
                               onEnableFiltersChange,
                               onFilterDimensionChange
                             }) =>
  <div className={classes.rowContainer}>
    <div className={classes.row}>
      {row.map((col, idx) =>
        <DashboardBuilderCol
          key={idx}
          t={t}
          classes={classes}
          defaultLanguage={defaultLanguage}
          languages={languages}
          dashboard={dashboard}
          rowIdx={rowIdx}
          colIdx={idx}
          col={col}
          onColRemove={onColRemove}
          onTypeReset={onTypeReset}
          fetchViews={fetchViews}
          onViewReset={onViewReset}
          onTextEdit={onTextEdit}
          onShowTitleChange={onShowTitleChange}
          onEnableFiltersChange={onEnableFiltersChange}
          onFilterDimensionChange={onFilterDimensionChange}
        />
      )}
    </div>
    <div className={classes.rowActions}>
      <Tooltip title={t("components.dashboardBuilder.actions.addColumn")}>
        <span>
          <IconButton
            className={classes.rowAction}
            onClick={() => onColAdd(rowIdx)}
            disabled={row.length === MAX_VIEW_PER_ROW}
          >
            <AddIcon/>
          </IconButton>
        </span>
      </Tooltip>
      <Tooltip title={t("components.dashboardBuilder.actions.removeRow")}>
        <IconButton
          className={classes.rowAction}
          onClick={() => onRowRemove(rowIdx)}
        >
          <DeleteIcon/>
        </IconButton>
      </Tooltip>
    </div>
  </div>;

function DashboardBuilder(props) {
  const {
    classes,
    defaultLanguage,
    languages,
    nodes,
    dashboard,
    onChange,
    views,
    fetchViews,
    onViewsHide
  } = props;

  const {t} = useTranslation();

  const [selectedRowIdx, setSelectedRowIdx] = useState(null);
  const [selectedColIdx, setSelectedColIdx] = useState(null);

  const [isEditTextVisible, setEditTextVisibility] = useState(false);
  const [tempTextValue, setTempTextValue] = useState(null);

  const handleRowAdd = () => {
    const newDashboard = _.cloneDeep(dashboard);
    newDashboard.dashboardConfig.push([emptyDashboardElem]);
    onChange(newDashboard);
  };

  const handleRowRemove = rowIdx => {
    let newDashboard = _.cloneDeep(dashboard);
    newDashboard.dashboardConfig = newDashboard.dashboardConfig.filter((_, idx) => idx !== rowIdx);
    onChange(newDashboard);
  };

  const handleColAdd = rowIdx => {
    let newDashboard = _.cloneDeep(dashboard);
    newDashboard.dashboardConfig[rowIdx].push(emptyDashboardElem);

    let totalWidth = 0;
    newDashboard.dashboardConfig[rowIdx] = newDashboard.dashboardConfig[rowIdx].map((col, colIdx) => {
      const width = colIdx < (newDashboard.dashboardConfig[rowIdx].length - 1)
        ? Math.floor(100 / newDashboard.dashboardConfig[rowIdx].length)
        : 0;
      totalWidth += width;

      return {
        ...col,
        [DASHBOARD_ELEM_WIDTH_KEY]: width || (100 - totalWidth)
      }
    });
    onChange(newDashboard);
  };

  const handleColRemove = (rowIdx, colIdx) => {
    let newDashboard = _.cloneDeep(dashboard);
    newDashboard.dashboardConfig[rowIdx] = newDashboard.dashboardConfig[rowIdx].filter((_, idx) => idx !== colIdx);
    newDashboard.dashboardConfig[rowIdx] = newDashboard.dashboardConfig[rowIdx].map(col => ({
      ...col,
      [DASHBOARD_ELEM_WIDTH_KEY]: (100 / newDashboard.dashboardConfig[rowIdx].length)
    }));
    onChange(newDashboard);
  };

  const handleTypeReset = (rowIdx, colIdx) => {
    let newDashboard = _.cloneDeep(dashboard);
    newDashboard.dashboardConfig[rowIdx][colIdx][DASHBOARD_ELEM_TYPE_KEY] = null;
    newDashboard.dashboardConfig[rowIdx][colIdx][DASHBOARD_ELEM_VALUE_KEY] = null;
    onChange(newDashboard);
  };

  const handleViewSet = (rowIdx, colIdx, view) => {
    let newDashboard = _.cloneDeep(dashboard);
    newDashboard.dashboardConfig[rowIdx][colIdx][DASHBOARD_ELEM_TYPE_KEY] = DASHBOARD_ELEM_TYPE_VALUE_VIEW;
    newDashboard.dashboardConfig[rowIdx][colIdx][DASHBOARD_ELEM_VALUE_KEY] = view.viewTemplateId;
    newDashboard.views[view.viewTemplateId] = view;
    onChange(newDashboard);
    onViewsHide();
  };

  const handleViewReset = (rowIdx, colIdx, viewId) => {
    let newDashboard = _.cloneDeep(dashboard);
    newDashboard.dashboardConfig[rowIdx][colIdx][DASHBOARD_ELEM_VALUE_KEY] = null;
    onChange(newDashboard);
    onViewsHide();
  };

  const handleEditTextShow = (rowIdx, colIdx, text) => {
    setSelectedRowIdx(rowIdx);
    setSelectedColIdx(colIdx);
    setTempTextValue(text);
    setEditTextVisibility(true);
  };

  const handleEditTextHide = () => {
    setSelectedRowIdx(null);
    setSelectedColIdx(null);
    setTempTextValue(null);
    setEditTextVisibility(false);
  };

  const handleTextChange = (rowIdx, colIdx, text) => {
    let newDashboard = _.cloneDeep(dashboard);
    newDashboard.dashboardConfig[rowIdx][colIdx][DASHBOARD_ELEM_TYPE_KEY] = DASHBOARD_ELEM_TYPE_VALUE_TEXT;
    newDashboard.dashboardConfig[rowIdx][colIdx][DASHBOARD_ELEM_VALUE_KEY] = text;
    onChange(newDashboard);
    setEditTextVisibility(false);
  };

  const handleShowTitleChange = (rowIdx, colIdx, value) => {
    let newDashboard = _.cloneDeep(dashboard);
    newDashboard.dashboardConfig[rowIdx][colIdx][DASHBOARD_ELEM_SHOW_TITLE_KEY] = value;
    onChange(newDashboard);
  };

  const handleEnableFiltersChange = (rowIdx, colIdx, value) => {
    let newDashboard = _.cloneDeep(dashboard);
    newDashboard.dashboardConfig[rowIdx][colIdx][DASHBOARD_ELEM_ENABLE_FILTERS_KEY] = value;
    onChange(newDashboard);
  };

  const handleFilterDimensionChange = (rowIdx, colIdx, value) => {
    let newDashboard = _.cloneDeep(dashboard);
    newDashboard.dashboardConfig[rowIdx][colIdx][DASHBOARD_ELEM_FILTER_DIMENSION_KEY] = value;
    onChange(newDashboard);
  };

  return (
    <Fragment>
      {dashboard && (
        <Grid container spacing={1}>
          <Grid item xs={12}>
            <FormControl fullWidth className={classes.field}>
              <I18nDelayedTextField
                label={t("components.dashboardBuilder.title")}
                value={dashboard.title}
                variant="outlined"
                required
                onChange={value => onChange({...dashboard, title: value})}
              />
            </FormControl>
          </Grid>
          <Grid item xs={12}>
            <Grid container>
              <Grid item className={classes.filterLevels} style={{marginRight: 16, lineHeight: "34px"}}>
                {t("components.dashboardBuilder.filterLevels")}:
              </Grid>
              <Grid item>
                <Grid container>
                  {filters.labels.map((label, idx) =>
                    <Grid item key={idx} style={{marginRight: 8}}>
                      <FormControlLabel
                        className={classes.formLabel}
                        label={getDashboardDynamicFilterLabelTranslations(t)[label]}
                        control={
                          <Checkbox
                            style={{padding: 5}}
                            checked={!!dashboard.filterLevels[label]}
                            onChange={(ev, checked) => onChange({
                              ...dashboard,
                              filterLevels: {
                                ...dashboard.filterLevels,
                                [label]: checked
                              }
                            })}
                          />
                        }
                      />
                    </Grid>
                  )}
                </Grid>
              </Grid>
            </Grid>
          </Grid>
          <Grid item xs={12}>
            {(dashboard.dashboardConfig || []).map((row, idx) =>
              <DashboardBuilderRow
                key={idx}
                t={t}
                classes={classes}
                defaultLanguage={defaultLanguage}
                languages={languages}
                dashboard={dashboard}
                row={row}
                rowIdx={idx}
                onRowRemove={handleRowRemove}
                onColAdd={handleColAdd}
                onColRemove={handleColRemove}
                onTypeReset={handleTypeReset}
                fetchViews={(rowIdx, colIdx) => {
                  fetchViews();
                  setSelectedRowIdx(rowIdx);
                  setSelectedColIdx(colIdx);
                }}
                onViewReset={handleViewReset}
                onTextEdit={handleEditTextShow}
                onShowTitleChange={handleShowTitleChange}
                onEnableFiltersChange={handleEnableFiltersChange}
                onFilterDimensionChange={handleFilterDimensionChange}
              />
            )}
          </Grid>
          <Grid item xs={12}>
            <Grid container justify="center">
              <Button endIcon={<AddIcon/>} onClick={handleRowAdd}>
                {t("components.dashboardBuilder.actions.addRow")}
              </Button>
            </Grid>
          </Grid>
        </Grid>
      )}

      <Dialog
        open={views !== null && views !== undefined}
        fullWidth
        maxWidth="md"
        onClose={onViewsHide}
      >
        <DialogTitle>
          {t("components.dashboardBuilder.modals.views.title")}
        </DialogTitle>
        <DialogContent>
          <InfiniteScrollTable
            data={(views || []).filter(({nodeId}) => nodes.find(node => node.nodeId === nodeId))}
            getRowKey={({viewTemplateId}) => viewTemplateId}
            columns={[
              {
                title: t("components.dashboardBuilder.modals.views.columns.node"),
                dataIndex: 'nodeId',
                render: (_, {nodeId}) => nodes.find(node => node.nodeId === nodeId).code,
                renderText: nodeId => nodes.find(node => node.nodeId === nodeId).code,
                minWidthToContent: true
              },
              {
                title: t("components.dashboardBuilder.modals.views.columns.datasetId"),
                dataIndex: 'datasetId',
                minWidthToContent: true
              },
              {
                title: t("components.dashboardBuilder.modals.views.columns.name"),
                dataIndex: 'title',
                minWidthToContent: true
              }
            ]}
            multilangStrDataIndexes={["title"]}
            onRowClick={({__originalRow: view}) => {
              handleViewSet(selectedRowIdx, selectedColIdx, view);
              setSelectedRowIdx(null);
              setSelectedColIdx(null);
            }}
          />
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => {
              onViewsHide();
              setSelectedRowIdx(null);
              setSelectedColIdx(null);
            }}
          >
            {t("commons.confirm.cancel")}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog
        open={isEditTextVisible}
        fullWidth
        disableEnforceFocus
        maxWidth="md"
        onClose={handleEditTextHide}
      >
        <DialogTitle>
          {t("components.dashboardBuilder.modals.text.title")}
        </DialogTitle>
        <DialogContent>
          <I18nHtmlEditor
            value={tempTextValue}
            onChange={setTempTextValue}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleEditTextHide}>
            {t("commons.confirm.cancel")}
          </Button>
          <Button onClick={() => handleTextChange(selectedRowIdx, selectedColIdx, tempTextValue)}>
            {t("commons.confirm.confirm")}
          </Button>
        </DialogActions>
      </Dialog>

    </Fragment>
  )
}

export default compose(
  withStyles(styles),
  connect(state => ({
    languages: state.app.languages,
    defaultLanguage: state.app.language
  }))
)(DashboardBuilder);