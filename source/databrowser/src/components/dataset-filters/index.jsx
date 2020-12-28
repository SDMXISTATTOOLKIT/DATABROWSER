import React, {Fragment, useEffect, useState} from 'react';
import {compose} from "redux";
import {withStyles} from "@material-ui/core";
import InputLabel from "@material-ui/core/InputLabel";
import Select from "@material-ui/core/Select";
import MenuItem from "@material-ui/core/MenuItem";
import FormControl from "@material-ui/core/FormControl";
import Dialog from "@material-ui/core/Dialog";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import Tooltip from "@material-ui/core/Tooltip";
import {getTextWidth} from "../../utils/style";
import InfiniteScrollTable from "../infinite-scroll-table";
import {getDimensionAttributeMap, getDimensionFilterValues, TIME_PERIOD_DIMENSION_KEY} from "../../utils/jsonStat";
import {useTranslation} from "react-i18next";
import {
  LABEL_FORMAT_SELECTOR_LABEL_FORMAT_BOTH,
  LABEL_FORMAT_SELECTOR_LABEL_FORMAT_ID,
  LABEL_FORMAT_SELECTOR_LABEL_FORMAT_NAME
} from "../label-format-selector/constants";

const $ = window.jQuery;

const styles = theme => ({
  root: {},
  filter: {
    display: "inline-block",
    verticalAlign: "bottom",
    minWidth: 120,
    marginRight: 24,
    marginBottom: 8
  },
  dialogContent: {
    padding: "8px !important"
  },
  item: {
    fontSize: 14
  },
  itemDisabled: {
    pointerEvents: "none",
    '& > svg': {
      visibility: "hidden"
    }
  },
  dimensionTitle: {
    marginBottom: 4,
    fontSize: 13,
    color: theme.palette.primary.main
  },
  attributeId: {
    cursor: "default",
    fontSize: 13,
    color: "rgb(255, 255, 255)",
    backgroundColor: "rgb(136, 136, 136)",
    borderRadius: 3,
    padding: "0 4px"
  },
  attributeAst: {
    cursor: "default",
    fontSize: 15,
    color: "rgb(136, 136, 136)",
    fontFamily: "Do Hyeon"
  }
});

const getFormattedName = (id, name, labelFormat) => {
  switch (labelFormat) {
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

const isMultiSelectDim = (dim, layout) => (layout?.primaryDim?.[0] === dim || layout?.secondaryDim?.[0] === dim);

function DatasetFilters(props) {
  const {
    classes,
    jsonStat,
    hiddenAttributes,
    layout,
    filterTree,
    labelFormat,
    onSelect
  } = props;

  const {t} = useTranslation();

  const [filters] = useState((layout?.primaryDim || []).concat((layout?.secondaryDim || [])).concat(layout?.filters || []));

  const [visibleId, setVisibleId] = useState(null);
  const [data, setData] = useState(null);

  const [checkedKeys, setCheckedKeys] = useState(null);

  const [dimensionAttributesMap, setDimensionAttributesMap] = useState(null);

  useEffect(() => {
    setDimensionAttributesMap(getDimensionAttributeMap(jsonStat, hiddenAttributes))
  }, [jsonStat, hiddenAttributes]);

  const handleFilterOpen = visibleIdx => {
    setVisibleId(visibleIdx);
    const data = getDimensionFilterValues(filters[visibleIdx], jsonStat, layout, filterTree, true);
    if (filters[visibleIdx] === TIME_PERIOD_DIMENSION_KEY) {
      data.reverse();
    }
    setData(data);

    const dim = filters[visibleIdx];
    if (isMultiSelectDim(dim, layout)) {
      if (checkedKeys === null && dim === layout?.primaryDim?.[0]) {
        setCheckedKeys(layout.primaryDimValues);
      } else if (checkedKeys === null && dim === layout?.secondaryDim?.[0]) {
        setCheckedKeys(layout.secondaryDimValues);
      }
    }
  };

  const handleFilterClose = () => {
    setVisibleId(null);
    setCheckedKeys(null);
    setData(null);
  };

  const handleFilterSubmit = (dimension, values, isArr) => {
    let data;
    if (isArr) {
      data = jsonStat.dimension[dimension].category.index.filter(dimVal => values.includes(dimVal));
    } else {
      data = values;
    }
    onSelect(dimension, data);
    handleFilterClose();
  };

  return (
    <Fragment>
      <div className={classes.root}>
        {(() => {
          const getTextWidthEl = $('<span>').css({
            visibility: 'hidden',
            position: 'absolute',
            fontSize: 16
          }).appendTo('body').get(0);

          const select = [];
          filters.forEach((filter, idx) => {

            const {
              filtersValue
            } = layout;

            if (isMultiSelectDim(filter, layout) || jsonStat.size[jsonStat.id.indexOf(filter)] !== 1) {

              const dimLabel = (jsonStat.dimension[filter].label || filter);
              const minWidth = getTextWidth(dimLabel, getTextWidthEl) + 16;

              select.push(
                <div key={idx} className={classes.filter}>
                  {isMultiSelectDim(filter, layout) && (
                    <InputLabel className={classes.dimensionTitle}>
                      {layout.primaryDim[0] === filter
                        ? t("components.datasetFilters.primaryDim")
                        : t("components.datasetFilters.secondaryDim")
                      }
                    </InputLabel>
                  )}
                  <FormControl style={{display: "inline-block", verticalAlign: "bottom"}}>
                    <InputLabel>{dimLabel}</InputLabel>
                    <Select
                      open={false}
                      style={{minWidth: minWidth}}
                      value={isMultiSelectDim(filter, layout)
                        ? layout.primaryDim[0] === filter
                          ? layout.primaryDimValues
                          : layout.secondaryDimValues
                        : filtersValue[filter]
                      }
                      multiple={isMultiSelectDim(filter, layout)}
                      renderValue={value => isMultiSelectDim(filter, layout)
                        ? jsonStat.size[jsonStat.id.indexOf(filter)] !== 1
                          ? value.length === 1
                            ? t("components.datasetFilters.selectedCount", {selected: value.length})
                            : t("components.datasetFilters.selectedCount_plural", {selected: value.length})
                          : getFormattedName(value[0], jsonStat.dimension[filter].category.label[value[0]], labelFormat)
                        : getFormattedName(value, jsonStat.dimension[filter].category.label[value], labelFormat)
                      }
                      className={`${classes.item} ${jsonStat.size[jsonStat.id.indexOf(filter)] === 1 ? classes.itemDisabled : ""}`}
                      onOpen={() => jsonStat.size[jsonStat.id.indexOf(filter)] === 1
                        ? null
                        : handleFilterOpen(idx)
                      }
                    >
                      {jsonStat.dimension[filter].category.index.map((el, idx) => (
                        <MenuItem key={idx + el} value={el}>{el}</MenuItem>
                      ))}
                    </Select>
                  </FormControl>
                  {dimensionAttributesMap && dimensionAttributesMap[filter][filtersValue[filter]] && ( // TODO: attributi per dimensioni multiselezionabili
                    <div style={{display: "inline-block", verticalAlign: "bottom", marginLeft: 8, marginBottom: 4}}>
                      <Tooltip
                        title={
                          <div>
                            {dimensionAttributesMap[filter][filtersValue[filter]].attributes.map(({id, label, valueId, valueLabel}, idx) => (
                              <div
                                key={idx}>{`${label || id}: ${valueLabel || valueId}${valueLabel !== valueId ? ` [${valueId}]` : ''}`}</div>
                            ))}
                          </div>
                        }
                        placement="top"
                      >
                        {(() => {
                          const ids = dimensionAttributesMap[filter][filtersValue[filter]].ids;
                          if (ids && ids.length === 1 && ids[0].length <= 2) {
                            return <div className={classes.attributeId}>{ids[0]}</div>
                          } else {
                            return <div className={classes.attributeAst}>(*)</div>
                          }
                        })()}
                      </Tooltip>
                    </div>
                  )}
                </div>
              )
            }
          });

          $(getTextWidthEl).remove();

          return select
        })()}
      </div>

      <Dialog
        open={data !== null}
        onClose={handleFilterClose}
        fullWidth
        maxWidth="md"
      >
        <DialogContent className={classes.dialogContent}>
          {!isMultiSelectDim(filters[visibleId], layout)
            ? (
              <InfiniteScrollTable
                data={data}
                getRowKey={({id}) => id}
                showHeader={false}
                columns={[
                  {
                    title: "",
                    dataIndex: 'label',
                    render: (_, {id, label}) => getFormattedName(id, label, labelFormat),
                    minWidth: 100
                  }
                ]}
                onRowClick={rowData => handleFilterSubmit(filters[visibleId], rowData.id)}
                getRowStyle={rowData => ({
                  background: rowData.id === layout.filtersValue[filters[visibleId]]
                    ? "#fff9e5"
                    : undefined
                })}
              />
            )
            : (
              <InfiniteScrollTable
                data={data}
                getRowKey={({id}) => id}
                columns={[
                  {
                    title: t("components.datasetFilters.table.columns.label.title"),
                    dataIndex: 'label',
                    render: (_, {id, label}) => getFormattedName(id, label, labelFormat),
                    minWidth: 100,
                    noFilter: true
                  }
                ]}
                rowSelection={{
                  selectedRowKeys: checkedKeys,
                  onChange: checkedKeys => setCheckedKeys(data.map(({id}) => id).filter(key => checkedKeys.includes(key)))
                }}
              />
            )
          }
        </DialogContent>
        <DialogActions>
          <Button onClick={handleFilterClose} color="primary">
            {t("commons.confirm.cancel")}
          </Button>
          {isMultiSelectDim(filters[visibleId], layout) && (
            <Button
              onClick={() => handleFilterSubmit(filters[visibleId], checkedKeys, true)}
              color="primary"
              disabled={(checkedKeys || []).length < 1}
            >
              {t("commons.confirm.confirm")}
            </Button>
          )}
        </DialogActions>
      </Dialog>
    </Fragment>
  )
}

export default compose(
  withStyles(styles)
)(DatasetFilters);