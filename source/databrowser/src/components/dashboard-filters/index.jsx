import React, {Fragment, useState} from "react";
import {useTranslation} from "react-i18next";
import withStyles from "@material-ui/core/styles/withStyles";
import {getTextWidth} from "../../utils/style";
import FormControl from "@material-ui/core/FormControl";
import InputLabel from "@material-ui/core/InputLabel";
import Select from "@material-ui/core/Select";
import MenuItem from "@material-ui/core/MenuItem";
import Dialog from "@material-ui/core/Dialog";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import InfiniteScrollTable from "../infinite-scroll-table";
import {getNode, getNodes, getNodesAtDepth} from "../../utils/tree";
import {getDashboardDynamicFilterLabelTranslations} from "../../constants/getDashboardDynamicFilterLabelTranslations";

const $ = window.jQuery;

const styles = theme => ({
  root: {},
  filter: {
    display: "inline-block",
    verticalAlign: "bottom",
    marginRight: 16,
    "& label": {
      fontSize: 16
    }
  },
  item: {
    fontSize: 14
  },
  dialogContent: {
    padding: "8px !important"
  },
});

const getLabelsMap = tree => {
  const map = {};
  getNodes(tree, "children", () => true).forEach(({id, name}) => map[id] = name);

  return map;
}

function DashboardFilters(props) {
  const {
    classes,
    filters,
    selectValues,
    setSelectValues,
    lastValorizedIdx,
    setLastValorizedIdx,
    filterLevels,
    onFilterApply
  } = props;

  const {
    labels,
    values
  } = filters;

  const {t} = useTranslation();

  const [isApplyDisabled, setIsApplyDisable] = useState(false);

  const [visibleId, setVisibleId] = useState(null);

  const [labelsMap] = useState(getLabelsMap(values));

  const handleFilterSelect = (dimension, value) => {
    const newSelectedValues = {};
    let found = false;
    labels.forEach(label => {
      if (!found) {
        if (label !== dimension) {
          newSelectedValues[label] = selectValues[label];
        } else {
          found = true;
          newSelectedValues[label] = value;
        }
      } else {
        newSelectedValues[label] = "";
      }
    })
    setSelectValues(newSelectedValues);
    setLastValorizedIdx(visibleId);
    setVisibleId(null);
    setIsApplyDisable(false);
  };

  let data = [];
  if (visibleId !== null) {
    data = visibleId === 0
      ? values
      : getNode(values, "children", ({id}) => id === selectValues[labels[visibleId - 1]]).children
  }

  return (
    <Fragment>
      <div className={classes.root}>
        {(() => {
          const selects = [];

          let lastLavorizableIdx = -1;
          labels.forEach((label, idx) => {
            if (filterLevels?.[labels[idx]] === true) {
              lastLavorizableIdx = idx;
            }
          });

          const getTextWidthEl = $('<span>').css({
            visibility: 'hidden',
            position: 'absolute',
            fontSize: 16
          }).appendTo('body').get(0);

          for (let i = 0; i <= Math.min(lastValorizedIdx + 1, lastLavorizableIdx); i++) {
            const label = labels[i];
            const minWidth = getTextWidth(label, getTextWidthEl) + 32;

            selects.push(
              <div key={i} className={classes.filter}>
                <FormControl style={{display: "inline-block", verticalAlign: "bottom"}}>
                  <InputLabel>{getDashboardDynamicFilterLabelTranslations(t)[label]}</InputLabel>
                  <Select
                    open={false}
                    style={{minWidth: minWidth}}
                    value={selectValues[label] || ""}
                    renderValue={value => labelsMap[value]}
                    className={classes.item}
                    onOpen={() => setVisibleId(i)}
                  >
                    {getNodesAtDepth(values, "children", (i + 1)).map(({id}, idx) =>
                      <MenuItem key={idx} value={id}>{id}</MenuItem>
                    )}
                  </Select>
                </FormControl>
              </div>
            );
          }

          $(getTextWidthEl).remove();

          if (lastLavorizableIdx >= 0) {
            selects.push(
              <Button
                key={labels.length}
                onClick={() => {
                  onFilterApply(selectValues[labels[lastValorizedIdx]]);
                  setIsApplyDisable(true);
                }}
                disabled={isApplyDisabled || lastValorizedIdx < 0 || filterLevels?.[labels[lastValorizedIdx]] === false}
              >
                {t("commons.confirm.apply")}
              </Button>
            );
          }

          return selects
        })()}
      </div>

      <Dialog
        open={visibleId !== null}
        onClose={() => setVisibleId(null)}
        fullWidth
        maxWidth="md"
      >
        <DialogContent className={classes.dialogContent}>
          <InfiniteScrollTable
            data={data}
            getRowKey={({id}) => id}
            showHeader={false}
            columns={[
              {
                title: "ID",
                dataIndex: "id",
                minWidth: 100
              },
              {
                title: "Label",
                dataIndex: "name",
                minWidth: 150
              }
            ]}
            onRowClick={rowData => handleFilterSelect(labels[visibleId], rowData.id)}
            getRowStyle={rowData => ({
              background: rowData.id === selectValues[labels[visibleId]]
                ? "#fff9e5"
                : undefined
            })}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setVisibleId(null)} color="primary">
            {t("commons.confirm.cancel")}
          </Button>
        </DialogActions>
      </Dialog>
    </Fragment>
  )
}

export default withStyles(styles)(DashboardFilters);