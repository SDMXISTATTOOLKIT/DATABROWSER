import React, {Fragment, useEffect, useState} from 'react';
import {compose} from "redux";
import {connect} from "react-redux";
import withStyles from "@material-ui/core/styles/withStyles";
import Box from "@material-ui/core/Box";
import Tabs from "@material-ui/core/Tabs";
import Tab from "@material-ui/core/Tab";
import Call from "../../hocs/call";
import {
  CRITERIA_SELECTION_MODE_ALL,
  CRITERIA_SELECTION_MODE_STEP_BY_STEP,
  CRITERIA_SELECTION_TYPE_DYNAMIC,
  fetchDatasetStructureCodelist,
  fetchDatasetStructureCodelists,
  setDatasetStructureCodelistCriteria,
  setDatasetStructureCodelistTimePeriod
} from "../../state/dataset/datasetActions";
import EnhancedTree from "../enhanced-tree";
import _ from "lodash"
import PlaylistAddCheckIcon from "@material-ui/icons/PlaylistAddCheck"
import {Tooltip} from "@material-ui/core";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import Dialog from "@material-ui/core/Dialog";
import InfiniteScrollTable from "../infinite-scroll-table";
import TimePeriod from "./TimePeriod";
import Typography from '@material-ui/core/Typography';
import {withTranslation} from "react-i18next";
import {TIME_PERIOD_DIMENSION_KEY} from "../../utils/jsonStat";
import TextField from "@material-ui/core/TextField";
import CustomEmpty from "../custom-empty";
import {CRITERIA_FILTER_TYPE_CODES, CRITERIA_FILTER_TYPE_STRING} from "../../utils/criteria";
import IconButton from "@material-ui/core/IconButton";
import LibraryAddCheckIcon from "@material-ui/icons/LibraryAddCheck";
import FilterNoneIcon from "@material-ui/icons/FilterNone";

const CRITERIA_CONTAINER_HEIGHT = 380;
const CODELIST_INFO_HEIGHT = 40;

const styles = theme => ({
  root: {},
  criteriaContainer: {
    width: "100%",
    height: CRITERIA_CONTAINER_HEIGHT,
    padding: 8
  },
  codelistInfo: {
    width: "100%",
    height: CODELIST_INFO_HEIGHT,
    padding: "8px 0"
  },
  codelistContainer: {
    width: "100%",
    height: `calc(100% - ${CODELIST_INFO_HEIGHT}px)`,
  },
  emptyContainer: {
    width: "100%",
    height: "100%"
  },
  timePeriodContainer: {
    width: "100%",
    height: "100%"
  },
  treeActions: {
    marginBottom: 8
  },
});

const mapDispatchToProps = dispatch => ({
  fetchCodelist: ({nodeId, datasetId, dimensionId, type, criteria}) => dispatch(fetchDatasetStructureCodelist(nodeId, datasetId, dimensionId, type, criteria)),
  fetchCodelists: ({nodeId, datasetId, type}) => dispatch(fetchDatasetStructureCodelists(nodeId, datasetId, type)),
  onSetCriteria: criteria => dispatch(setDatasetStructureCodelistCriteria(criteria)),
  onSetTimePeriod: timePeriod => dispatch(setDatasetStructureCodelistTimePeriod(timePeriod)),
});

function Criteria(props) {
  const {
    t,
    classes,
    nodeId,
    datasetId,
    dimensions,
    mode,
    type,
    codes,
    isCodesFlat,
    codelists,
    areCodelistsFlat,
    codelistsLength,
    criteria,
    timePeriod,
    fetchCodelist,
    fetchCodelists,
    onSetCriteria,
    onSetTimePeriod,
    isCriteriaValid,
    setCriteriaValidity
  } = props;

  const [tabId, setTabId] = useState(0);
  const [isCallDisabled, setIsCallDisabled] = useState(false);

  const [tmpCriteria, setTmpCriteria] = useState(null);
  const [tmpTabId, setTmpTabId] = useState(null);

  const handleTabChange = tabId => {

    if (type === CRITERIA_SELECTION_TYPE_DYNAMIC && tabId < (dimensions.length - 1)) {
      const newCriteria = _.cloneDeep(criteria);

      const criteriaToRemove = dimensions
        .map(({id}) => id)
        .slice(tabId + 1)
        .filter(dim => newCriteria[dim] !== null && newCriteria[dim] !== undefined);

      if (criteriaToRemove.length > 0) {
        criteriaToRemove.forEach(dim => newCriteria[dim] = undefined);
        setTmpCriteria(newCriteria);
        setTmpTabId(tabId);

      } else {
        setTabId(tabId);
        setIsCallDisabled(false);
      }

    } else {
      setTabId(tabId);
      setIsCallDisabled(false);
    }
  };

  const handleCheck = (checkedKeys, type) => {
    const newCriteria = _.cloneDeep(criteria);
    const dimensionId = dimensions[tabId].id;

    newCriteria[dimensionId] = (checkedKeys || []).length > 0
      ? {
        id: dimensionId,
        type: (type || CRITERIA_FILTER_TYPE_CODES),
        filterValues: checkedKeys,
        period: null,
        from: null,
        to: null
      }
      : undefined;

    onSetCriteria(newCriteria);
  };

  useEffect(() => {
    if (dimensions && isCallDisabled === false) {
      setIsCallDisabled(true)
    }
  }, [dimensions, isCallDisabled]);

  const checkedKeys = (criteria[dimensions[tabId].id]?.filterValues || []);

  return (
    <div className={classes.root}>
      {dimensions && (
        <Fragment>
          <Box>
            <Tabs
              value={tabId}
              variant="scrollable"
              scrollButtons="auto"
              onChange={(event, newValue) => handleTabChange(newValue)}
            >
              {(dimensions || []).map((dim, idx) => (
                <Tab
                  key={idx}
                  label={
                    <Tooltip title={dim.label || ""}>
                      <span>
                        {dim.id + (
                          codelistsLength[idx]
                            ? ` (${(criteria[dim.id]?.filterValues || []).length}/${codelistsLength[idx]})`
                            : ''
                        )}
                      </span>
                    </Tooltip>
                  }
                  disabled={!isCriteriaValid}
                />
              ))}
            </Tabs>
          </Box>
          <div key={tabId} className={classes.criteriaContainer}>
            <Call
              cb={mode === CRITERIA_SELECTION_MODE_STEP_BY_STEP
                ? fetchCodelist
                : fetchCodelists
              }
              cbParam={{
                nodeId,
                datasetId,
                dimensionId: dimensions[tabId].id,
                mode,
                type,
                criteria
              }}
              disabled={!nodeId || !datasetId || !dimensions || (mode === CRITERIA_SELECTION_MODE_ALL && !!codelists) || isCallDisabled}
            >
              {(() => {
                const data = mode === CRITERIA_SELECTION_MODE_STEP_BY_STEP
                  ? codes
                  : codelists?.[tabId];

                const isCheckDisabled = ((data || []).length === 1 && (data[0].children || []).length === 0);

                if (!data) {
                  return (
                    <div className={classes.emptyContainer}>
                      <CustomEmpty/>
                    </div>
                  )

                } else if (dimensions[tabId].id === TIME_PERIOD_DIMENSION_KEY) {
                  return (
                    <div className={classes.timePeriodContainer}>
                      <TimePeriod
                        timePeriod={timePeriod}
                        onSetTimePeriod={onSetTimePeriod}
                        isCriteriaValid={isCriteriaValid}
                        setCriteriaValidity={setCriteriaValidity}
                      />
                    </div>
                  )

                } else {
                  return (
                    <Fragment>
                      <Typography className={classes.codelistInfo}>
                        {`${dimensions[tabId].label || dimensions[tabId].id}${dimensions[tabId]?.extra?.DataStructureRef ? ` (${dimensions[tabId].extra.DataStructureRef})` : ""}`}
                      </Typography>
                      <div className={classes.codelistContainer}>
                        {(data.length === 0)
                          ? (
                            <Fragment>
                              <div style={{margin: "16px 0"}}>
                                {t("components.criteria.noCodelistAssociated.info")}
                              </div>
                              <TextField
                                value={checkedKeys[0] || ""}
                                placeholder={t("components.criteria.noCodelistAssociated.noFilter")}
                                variant="outlined"
                                style={{width: "100%"}}
                                onChange={({target}) => handleCheck([target.value], CRITERIA_FILTER_TYPE_STRING)}
                              />
                            </Fragment>
                          )
                          : (isCodesFlat === true || areCodelistsFlat?.[tabId] === true)
                            ? (
                              <InfiniteScrollTable
                                data={data}
                                getRowKey={({id}) => id}
                                showHeader={false}
                                columns={[
                                  {
                                    title: "",
                                    dataIndex: 'name',
                                    render: (_, {id, name}) => `[${id}] ${name}`,
                                    renderText: (_, {id, name}) => `[${id}] ${name}`,
                                    minWidthToContent: true,
                                    noFilter: true
                                  }
                                ]}
                                rowSelection={{
                                  selectedRowKeys: checkedKeys,
                                  onChange: handleCheck,
                                }}
                                leftActions={
                                  <Fragment>
                                    <Tooltip title={t("components.criteria.table.actions.selectAll.tooltip")}>
                                      <span>
                                        <IconButton
                                          onClick={() => handleCheck(data.map(({id}) => id))}
                                          style={{padding: 8}}
                                          disabled={isCheckDisabled}
                                        >
                                          <LibraryAddCheckIcon/>
                                        </IconButton>
                                      </span>
                                    </Tooltip>
                                    <Tooltip title={t("components.criteria.table.actions.deselectAll.tooltip")}>
                                      <span>
                                        <IconButton
                                          onClick={() => handleCheck([])}
                                          style={{padding: 8}}
                                          disabled={isCheckDisabled}
                                        >
                                          <FilterNoneIcon style={{padding: 1}}/>
                                        </IconButton>
                                      </span>
                                    </Tooltip>
                                  </Fragment>
                                }
                                isCheckDisabled={isCheckDisabled}
                                height={204}
                              />
                            )
                            : (
                              <EnhancedTree
                                key={tabId}
                                tree={data}
                                getNodeKey={({id}) => id}
                                childrenKey="children"
                                labelKey="name"
                                getNodeLabel={node => `[${node.id}] ${node.name}`}
                                isLabelNotMultilingual
                                checkable
                                withLevelSelector
                                checkedKeys={checkedKeys}
                                getIsCheckable={node => node.isSelectable !== false}
                                // defaultExpandedKeys={criteria[dimensions[tabId].id] || []}
                                onCheck={handleCheck}
                                isCheckDisabled={isCheckDisabled}
                                nodeActions={[
                                  node => (node.children && node.children.length > 0)
                                    ? {
                                      icon: <PlaylistAddCheckIcon/>,
                                      title: t("components.criteria.tree.nodeActions.selectAll.title"),
                                      onClick: node => {
                                        let newCheckedKeys = [...checkedKeys];
                                        const childrenKeys = (node.children || []).filter(({isSelectable}) => isSelectable !== false).map(({id}) => id)
                                        const unselectedChildrenKeys = childrenKeys.filter(key => !newCheckedKeys.includes(key))

                                        newCheckedKeys = (unselectedChildrenKeys.length > 0)
                                          ? newCheckedKeys.concat(childrenKeys)
                                          : newCheckedKeys.filter(key => !childrenKeys.includes(key));
                                        handleCheck(_.uniq(newCheckedKeys));
                                      }
                                    }
                                    : null
                                ]}
                              />
                            )
                        }
                      </div>
                    </Fragment>
                  )
                }
              })()}
            </Call>
          </div>

          <Dialog
            open={tmpCriteria !== null}
            fullWidth
            maxWidth="sm"
            onClose={() => setTmpCriteria(null)}
          >
            <DialogContent>
              {t("components.criteria.dialogs.warning.losingFilters.title")}
            </DialogContent>
            <DialogActions>
              <Button
                onClick={() => {
                  setTmpCriteria(null);
                  setTmpTabId(null);
                }}
              >
                cancel
              </Button>
              <Button
                autoFocus
                onClick={() => {
                  setTmpCriteria(null);
                  onSetCriteria(tmpCriteria);
                  setTmpTabId(null);
                  setTabId(tmpTabId);
                  setIsCallDisabled(false);
                }}
                color="primary"
              >
                ok
              </Button>
            </DialogActions>
          </Dialog>

        </Fragment>
      )}
    </div>
  )
}

export default compose(
  withStyles(styles),
  withTranslation(),
  connect(null, mapDispatchToProps)
)(Criteria)