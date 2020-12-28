import React, {Fragment, useEffect, useState} from 'react';
import {compose} from "redux";
import withStyles from "@material-ui/core/styles/withStyles";
import {withTranslation} from "react-i18next";
import {SketchPicker} from 'react-color';
import _ from "lodash";
import Dialog from "@material-ui/core/Dialog";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import Grid from "@material-ui/core/Grid";
import Button from "@material-ui/core/Button";
import AddIcon from '@material-ui/icons/Add';
import Selector from "../selector";
import InfiniteScrollTable from "../infinite-scroll-table";
import Tooltip from "@material-ui/core/Tooltip";
import IconButton from "@material-ui/core/IconButton";
import DeleteIcon from '@material-ui/icons/Delete';
import EditIcon from '@material-ui/icons/Edit';
import Divider from "@material-ui/core/Divider";

export const CHART_COLORS_ALL_DIMENSION_VALUES_KEY = "ALL_ITEMS";
const getAllDimValuesLabel = t => t("scenes.dataViewer.chartSettings.colors.allDimValues");

const styles = theme => ({
  root: {
    width: "100%",
    height: "100%",
    padding: "8px 8px 0 8px"
  },
  title: {
    width: "calc(100% - 96px)"
  },
  scrollableRows: {
    height: "calc(100% - 44px)",
    overflowY: "auto",
    overflowX: "hidden"
  },
  rows: {
    marginBottom: 16
  },
  row: {},
  rowDimension: {
    fontSize: 14,
    lineHeight: "48px",
    whiteSpace: "nowrap",
    overflow: "hidden",
    textOverflow: "ellipsis"
  },
  item: {
    display: "inline-block",
    verticalAlign: "middle",
    width: "calc(100% - 96px)"
  },
  itemLabel: {
    display: "inline-block",
    verticalAlign: "middle",
    width: "calc(50% - 8px)",
    height: 48,
    lineHeight: "48px",
    marginRight: 8,
    fontSize: 14,
    whiteSpace: "nowrap",
    overflow: "hidden",
    textOverflow: "ellipsis"
  },
  itemColor: {
    display: "inline-block",
    verticalAlign: "middle",
    width: "calc(50% - 8px)",
    height: 32,
    marginRight: 8
  },
  rowActions: {
    display: "inline-block",
    verticalAlign: "middle",
    width: 96
  },
  selector: {
    width: "100%"
  }
});

const getClearedColors = (colors, jsonStat) => {
  const newColors = {};

  Object.keys(colors).forEach(dimKey => {
    if (jsonStat.id.includes(dimKey)) {
      Object.keys(colors[dimKey]).forEach(dimValKey => {
        if (jsonStat.dimension[dimKey].category.index.includes(dimValKey) || dimValKey === CHART_COLORS_ALL_DIMENSION_VALUES_KEY) {
          if (colors[dimKey][dimValKey]) {
            newColors[dimKey] = {...newColors[dimKey]};
            newColors[dimKey][dimValKey] = colors[dimKey][dimValKey]
          }
        }
      });
    }
  });

  return newColors;
};

const Row = ({
               t,
               classes,
               jsonStat,
               dimension,
               items,
               onColorEdit,
               onColorRemove
             }) =>

  <div className={classes.row}>
    <Grid container spacing={3}>
      <Grid item xs={4}>
        <Tooltip title={jsonStat.dimension[dimension].label || dimension}>
          <div className={classes.rowDimension}>
            {jsonStat.dimension[dimension].label || dimension}
          </div>
        </Tooltip>
      </Grid>
      <Grid item xs={8}>
        {Object.keys(items)
          .map((item, idx) => {
            const label = jsonStat.dimension[dimension].category.label[item]
              ? jsonStat.dimension[dimension].category.label[item]
              : item === CHART_COLORS_ALL_DIMENSION_VALUES_KEY
                ? getAllDimValuesLabel(t)
                : item;

            return (
              <Fragment key={idx}>
                <div className={classes.item}>
                  <Tooltip title={label}>
                    <div className={classes.itemLabel}>
                      {label}
                    </div>
                  </Tooltip>
                  <div
                    className={classes.itemColor}
                    style={{
                      background: items[item],
                      border: `1px solid ${items[item]}`,
                      borderRadius: "4px"
                    }}
                  />
                </div>
                <div className={classes.rowActions}>
                  <Tooltip title={t("scenes.dataViewer.chartSettings.colors.row.actions.edit")}>
                    <IconButton onClick={() => onColorEdit(dimension, item, items[item])}>
                      <EditIcon/>
                    </IconButton>
                  </Tooltip>
                  <Tooltip title={t("scenes.dataViewer.chartSettings.colors.row.actions.delete")}>
                    <IconButton onClick={() => onColorRemove(dimension, item)}>
                      <DeleteIcon/>
                    </IconButton>
                  </Tooltip>
                </div>
              </Fragment>
            )
          })
          .sort((a, b) => {
            if (a === CHART_COLORS_ALL_DIMENSION_VALUES_KEY) {
              return 1
            } else if (b === CHART_COLORS_ALL_DIMENSION_VALUES_KEY) {
              return -1
            } else {
              return 0
            }
          })
        }
      </Grid>
    </Grid>
  </div>;

const defaultColor = "rgba(0, 0, 0, 0.1)";

function ChartSettingsColors(props) {
  const {
    t,
    classes,
    jsonStat,
    colors: initialColors,
    onColorsSet
  } = props

  const [colors, setColors] = useState(false);

  const [isAddColorVisible, setAddColorVisibility] = useState(false);

  const [newDim, setNewDim] = useState(null);
  const [newDimVal, setNewDimVal] = useState(null);
  const [newColor, setNewColor] = useState(defaultColor);

  const [data, setData] = useState(null);
  const [isDimension, setIsDimension] = useState(true);

  const [isColorPickerVisible, setColorPickerVisibility] = useState(false);

  const [tmpDim, setTmpDim] = useState(null);
  const [tmpDimVal, setTmpDimVal] = useState(null);
  const [tmpColor, setTmpColor] = useState(null);

  useEffect(() => {
    setColors(getClearedColors(initialColors, jsonStat));
  }, [initialColors, jsonStat]);

  const handleAddColorClose = () => {
    setAddColorVisibility(false);

    setNewDim(null);
    setNewDimVal(null);
    setNewColor(defaultColor);
  };

  const handleAddColorSubmit = () => {
    setAddColorVisibility(false);

    const newColors = _.cloneDeep(colors);
    newColors[newDim] = {...newColors[newDim]};
    newColors[newDim][newDimVal] = newColor
    onColorsSet(newColors);

    setNewDim(null);
    setNewDimVal(null);
    setNewColor(defaultColor);
  };

  const handleColorRemove = (dim, dimVal) => {
    const newColors = _.cloneDeep(colors);
    newColors[dim][dimVal] = undefined;
    onColorsSet(getClearedColors(newColors, jsonStat));
  };

  const handleColorEdit = (dim, dimVal, color) => {
    setTmpDim(dim);
    setTmpDimVal(dimVal);
    setTmpColor(color);
    setColorPickerVisibility(true);
  };

  const handleColorPickerClose = () => {
    setTmpDim(null);
    setTmpDimVal(null);
    setTmpColor(null);
    setColorPickerVisibility(false);
  };

  return (
    <Fragment>
      <div className={classes.root}>
        <div className={classes.title}>
          <Grid item xs={12}>
            <Grid container spacing={3} style={{margin: 0}}>
              <Grid item xs={4}>
                <Grid container justify="center">
                  {t("scenes.dataViewer.chartSettings.colors.dimension")}
                </Grid>
              </Grid>
              <Grid item xs={4}>
                <Grid container justify="center">
                  {t("scenes.dataViewer.chartSettings.colors.dimensionValue")}
                </Grid>
              </Grid>
              <Grid item xs={4}>
                <Grid container justify="center">
                  {t("scenes.dataViewer.chartSettings.colors.color")}
                </Grid>
              </Grid>
            </Grid>
          </Grid>
        </div>
        <div className={classes.scrollableRows}>
          <div className={classes.rows}>
            {Object.keys(colors).map((dimension, idx) => (
              <Fragment key={idx}>
                <Row
                  t={t}
                  classes={classes}
                  jsonStat={jsonStat}
                  dimension={dimension}
                  items={colors[dimension]}
                  onColorEdit={handleColorEdit}
                  onColorRemove={handleColorRemove}
                />
                <Divider/>
              </Fragment>
            ))}
          </div>
          <Grid container justify="center">
            <Button endIcon={<AddIcon/>} onClick={() => setAddColorVisibility(true)}>
              {t("scenes.dataViewer.chartSettings.colors.actions.addColor")}
            </Button>
          </Grid>
        </div>
      </div>

      <Dialog
        open={isAddColorVisible}
        onClose={handleAddColorClose}
        fullWidth
        maxWidth="md"
      >
        <DialogContent>
          <Grid container>
            <Grid item xs={12}>
              <Grid container spacing={2} style={{margin: 0}}>
                <Grid item xs={4}>
                  <Grid container justify="center">
                    {t("scenes.dataViewer.chartSettings.colors.dimension")}
                  </Grid>
                </Grid>
                <Grid item xs={4}>
                  <Grid container justify="center">
                    {t("scenes.dataViewer.chartSettings.colors.dimensionValue")}
                  </Grid>
                </Grid>
                <Grid item xs={4}>
                  <Grid container justify="center">
                    {t("scenes.dataViewer.chartSettings.colors.color")}
                  </Grid>
                </Grid>
              </Grid>
            </Grid>
            <Grid item xs={12}>
              <Grid container spacing={2}>
                <Grid item xs={4}>
                  <Selector
                    value={newDim}
                    render={dim => dim ? (jsonStat.dimension[dim].label || dim) : ""}
                    selectTitle={t("scenes.dataViewer.chartSettings.colors.addColor.selectDimension")}
                    onSelect={() => {
                      setIsDimension(true);
                      setData(jsonStat.id.map(dim => ({
                        id: dim,
                        label: jsonStat.dimension[dim].label || dim
                      })));
                    }}
                    resetTitle={t("scenes.dataViewer.chartSettings.colors.addColor.resetDimension")}
                    onReset={() => {
                      setNewDim(null);
                      setNewDimVal(null);
                    }}
                    className={classes.selector}
                  />
                </Grid>
                <Grid item xs={4}>
                  {newDim && (
                    <Selector
                      value={newDimVal}
                      render={dimVal => dimVal
                        ? jsonStat.dimension[newDim].category.label[dimVal]
                          ? jsonStat.dimension[newDim].category.label[dimVal]
                          : dimVal === CHART_COLORS_ALL_DIMENSION_VALUES_KEY
                            ? getAllDimValuesLabel(t)
                            : dimVal
                        : ""
                      }
                      selectTitle={t("scenes.dataViewer.chartSettings.colors.addColor.selectDimensionValue")}
                      onSelect={() => {
                        setIsDimension(false);
                        const data = [{
                          id: CHART_COLORS_ALL_DIMENSION_VALUES_KEY,
                          label: getAllDimValuesLabel(t)
                        }];
                        jsonStat.dimension[newDim].category.index.forEach(dimVal => data.push({
                          id: dimVal,
                          label: jsonStat.dimension[newDim].category.label[dimVal] || dimVal
                        }));
                        setData(data);
                      }}
                      resetTitle={t("scenes.dataViewer.chartSettings.colors.addColor.resetDimensionValue")}
                      onReset={() => setNewDimVal(null)}
                      className={classes.selector}
                    />
                  )}
                </Grid>
                <Grid item xs={4}>
                  {newDimVal && (
                    <div
                      style={{
                        width: "100%",
                        height: "100%",
                        background: newColor,
                        border: `1px solid ${newColor}`,
                        borderRadius: "4px"
                      }}
                      onClick={() => setColorPickerVisibility(true)}
                    />
                  )}
                </Grid>
              </Grid>
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleAddColorClose}>
            {t("commons.confirm.cancel")}
          </Button>
          <Button onClick={handleAddColorSubmit} disabled={!newDim || !newDimVal || !newColor}>
            {t("commons.confirm.confirm")}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog
        open={data !== null}
        onClose={() => setData(null)}
        fullWidth
        maxWidth="md"
      >
        <DialogContent>
          <InfiniteScrollTable
            data={data}
            getRowKey={({id}) => id}
            showHeader={false}
            columns={[
              {
                title: "",
                dataIndex: 'label',
                render: (_, {label}) => label,
                minWidth: 100
              }
            ]}
            onRowClick={({id}) => {
              if (isDimension) {
                if (id !== newDim) {
                  setNewDim(id);
                  setNewDimVal(null);
                }
              } else {
                if (id !== newDimVal) {
                  setNewDimVal(id);
                }
              }
              setData(null);
            }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setData(null)} color="primary">
            {t("commons.confirm.cancel")}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog
        open={isColorPickerVisible}
        onClose={handleColorPickerClose}
        maxWidth="sm"
      >
        <DialogContent style={{padding: 0}}>
          <SketchPicker
            color={tmpColor || newColor}
            onChange={({rgb}) => {
              const rgba = `rgba(${rgb.r}, ${rgb.g}, ${rgb.b}, ${rgb.a})`;
              if (tmpDim && tmpDimVal && tmpColor) {
                setTmpColor(rgba);
                const newColors = _.cloneDeep(colors);
                newColors[tmpDim][tmpDimVal] = rgba;
                onColorsSet(newColors);
              } else {
                setNewColor(rgba);
              }
            }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleColorPickerClose}>
            {t("commons.confirm.close")}
          </Button>
        </DialogActions>
      </Dialog>

    </Fragment>
  )
}

export default compose(
  withStyles(styles),
  withTranslation()
)(ChartSettingsColors);