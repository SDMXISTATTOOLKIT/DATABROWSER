import React, {forwardRef, useEffect, useImperativeHandle, useState} from 'react';
import Box from "@material-ui/core/Box";
import {compose} from "redux";
import {connect} from "react-redux";
import EditIcon from "@material-ui/icons/Edit";
import DeleteIcon from "@material-ui/icons/Delete";
import CheckIcon from "@material-ui/icons/Check";
import ClearIcon from "@material-ui/icons/Clear";
import SearchIcon from "@material-ui/icons/Search";
import ArrowUpwardIcon from "@material-ui/icons/ArrowUpward";
import AddIcon from "@material-ui/icons/Add";
import {
  clearDataflowCache,
  createDataflowCache,
  deleteAllDataflowCache,
  deleteCatalogCache,
  deleteDataflowCache,
  fetchDataflowCache,
  updateDataflowCache
} from "../../../../state/cache/cacheActions";
import Grid from "@material-ui/core/Grid";
import FormControl from "@material-ui/core/FormControl";
import Button from "@material-ui/core/Button";
import Paper from "@material-ui/core/Paper";
import {Checkbox, withStyles, withTheme} from "@material-ui/core";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import {useTranslation} from "react-i18next";
import CustomMaterialTable from "../../../custom-material-table";


const tableIcons = {
  Delete: forwardRef((props, ref) => <DeleteIcon {...props} ref={ref}/>),
  Edit: forwardRef((props, ref) => <EditIcon {...props} ref={ref}/>),
  Check: forwardRef((props, ref) => <CheckIcon {...props} ref={ref}/>),
  Clear: forwardRef((props, ref) => <ClearIcon {...props} ref={ref}/>),
  ResetSearch: forwardRef((props, ref) => <ClearIcon {...props} ref={ref}/>),
  SortArrow: forwardRef((props, ref) => <ArrowUpwardIcon {...props} ref={ref}/>),
  Search: forwardRef((props, ref) => <SearchIcon {...props} ref={ref}/>),
  Add: forwardRef((props, ref) => <AddIcon {...props} ref={ref}/>)
};

const styles = theme => ({
  root: {
    height: 440
  },
  field: {
    marginTop: theme.spacing(3)
  },
  paper: {
    marginTop: theme.spacing(2),
    padding: theme.spacing(3)
  },
  tabContent: {
    overflowY: "auto",
    overflowX: "hidden",
    height: "calc(100% - 56px)",
    marginTop: 8,
    padding: "0 4px"
  },
  title: {
    fontSize: 16
  }
});


const mapStateToProps = state => ({
  config: state.cacheConfig
});


const mapDispatchToProps = dispatch => ({
  fetchCache: nodeId => dispatch(fetchDataflowCache(nodeId)),
  clearCache: () => dispatch(clearDataflowCache()),
  updateCache: (nodeId, cacheId, ttl) => dispatch(updateDataflowCache(nodeId, cacheId, ttl)),
  createCache: (nodeId, data) => dispatch(createDataflowCache(nodeId, data)),
  deleteFromCache: (nodeId, cacheId) => dispatch(deleteDataflowCache(nodeId, cacheId)),
  deleteAllFromCache: (nodeId) => dispatch(deleteAllDataflowCache(nodeId)),
  deleteCatalogCache: (nodeId) => dispatch(deleteCatalogCache(nodeId))
});


const SECONDS_PER_DAY = 86400;
const SECONDS_PER_HOUR = 3600;


/**
 * Convert seconds to HH:MM:SS
 * If seconds exceeds 24 hours, hours will be greater than 24 (30:05:10)
 *
 * @param {number} seconds
 * @returns {string}
 */
const secondsToHms = (totalSeconds) => {

  if (!totalSeconds) {
    return "0 s";
  }

  var days = Math.floor(totalSeconds / SECONDS_PER_DAY);
  var hours = Math.floor((totalSeconds - (days * SECONDS_PER_DAY)) / SECONDS_PER_HOUR);
  var minutes = Math.floor((totalSeconds - (days * SECONDS_PER_DAY) - (hours * SECONDS_PER_HOUR)) / 60);
  var seconds = totalSeconds - (days * SECONDS_PER_DAY) - (hours * SECONDS_PER_HOUR) - (minutes * 60);

  // round seconds
  seconds = Math.round(seconds * 100) / 100

  return wellFormattedTimeString(days, hours, minutes, seconds);
};

const wellFormattedTimeString = (days, hours, mins, sec) => {
  var result = "";
  result += (days > 0 ? days + "d " : ""); // hide days if 0
  result += (hours < 10 ? " " + hours : hours) + "h ";
  result += (mins < 10 ? " " + mins : mins) + "m ";

  if (days === 0) { // truncate if days is greater than zero
    result += (sec < 10 ? " " + sec : sec) + "s";
  }

  return result;
}

const CacheSettingsForm = ({classes, config, nodeId, fetchCache, createCache, updateCache, clearCache, deleteFromCache, deleteAllFromCache, deleteCatalogCache}, ref) => {
//const CacheSettingsForm = compose(withStyles(styles), forwardRef)(({classes, config, nodeId, fetchCache, updateCache, clearCache, deleteFromCache}, ref) => {

  const [needConfig, setNeedConfig] = useState(nodeId !== null && (!config || config.length === 0));
  const [hideEmptyDataflowCache, setEmptyDataflowCacheHidden] = useState(true);
  const [emptyRowsCounter, setEmptyRowsCounter] = useState(0);
  const [rowsCounter, setRowsCounter] = useState(0);
  const {t} = useTranslation();

  useEffect(() => {

    if (!config || config.length === 0) {
      setRowsCounter(0);
      setEmptyRowsCounter(0);
      return;
    }
    setRowsCounter(config.length);
    setEmptyRowsCounter(config.filter(val => val?.cacheSize === 0).length);
  }, [config]);

  useEffect(() => {
    if (needConfig) {
      setNeedConfig(false);
      fetchCache(nodeId);
    }


  }, [config, fetchCache, nodeId, needConfig, setNeedConfig]);
  /*
      const {register, errors, handleSubmit, watch, setValue} = useForm({
          defaultValues: {
              ...config
          }
      });
  */

  useImperativeHandle(ref, () => ({
    submit(f) {
      f();
    },
    cancel(f) {
      clearCache();
      f();
    }
  }));


  return (

    <Grid container>
      <Grid container spacing={2}>
        <Grid item xs={12}>

          <Paper variant="outlined" className={classes.paper}>

            <Grid container spacing={2}>


              <Grid item xs={4}>
                <FormControl fullWidth>
                  <Button container style={{marginRight: 24}} size="large" variant="contained"
                          color="primary" onClick={() => deleteCatalogCache(nodeId)}>
                    {t("scenes.nodeSettings.cacheSettings.deleteCatalogCache")}
                  </Button>
                </FormControl>
              </Grid>
              {/* <Grid item xs={4}>
                                <FormControl fullWidth>
                                    <Button size="large" variant="contained" color="primary"
                                            onClick={window.notImplemented.show}>
                                        Refresh catalog cache
                                    </Button>
                                </FormControl>
                            </Grid>
                            <Grid item xs={4}/>*/}
            </Grid>
          </Paper>

        </Grid>
      </Grid>

      <Grid container spacing={2}>
        <Grid item xs={12}>
          <Paper variant="outlined" className={classes.paper}>
            <Grid container spacing={1}>
              <Grid item xs={4}>
                <FormControl fullWidth>
                  <Button container style={{marginRight: 24}} size="large" variant="contained"
                          color="primary" onClick={() => deleteAllFromCache(nodeId)}>
                    {t("scenes.nodeSettings.cacheSettings.deleteAllDataflowCache")}
                  </Button>
                </FormControl>
              </Grid>
              {/*<Grid item xs={4} >*/}
              {/*    <FormControl fullWidth>*/}
              {/*        <Button disabled size="large" variant="contained" color="primary"*/}
              {/*                onClick={window.notImplemented.show}>*/}
              {/*            Refresh dataflow cache*/}
              {/*        </Button>*/}
              {/*    </FormControl>*/}
              {/*</Grid>*/}
              <Grid item xs={4}/>
              <Grid item xs={4}/>
              <Grid item xs={12}>
                <FormControl fullWidth className={classes.field}>
                  <FormControlLabel
                    label={t("scenes.nodeSettings.cacheSettings.hideEmptyEntries")}
                    control={
                      <Checkbox
                        name="active"
                        required
                        disabled={emptyRowsCounter === 0 || rowsCounter === 0}
                        checked={hideEmptyDataflowCache}
                        onChange={(e, value) => setEmptyDataflowCacheHidden(value)}
                      />
                    }
                  />
                  {/*<FormHelperText>*/}
                  {/*    { (!hideEmptyDataflowCache? "Hide" : "Show") + " cache entries which size is 0 Kb." }*/}
                  {/*</FormHelperText>*/}
                </FormControl>
              </Grid>
            </Grid>
            <Grid item xs={12}>
              <CustomMaterialTable
                components={{
                  Container: Box
                }}
                icons={tableIcons}
                columns={[
                  {
                    title: t("scenes.nodeSettings.cacheSettings.table.dataflowTitle"),
                    field: "title",
                    editable: 'never'
                  },
                  /*{title: "Title", field: "dataflowId", editable: 'never'},*/
                  {
                    title: t("scenes.nodeSettings.cacheSettings.table.dataflowValidity"),
                    field: "ttl",
                    type: "numeric",
                    editable: 'onUpdate',
                    render: rowData => secondsToHms(rowData.ttl)
                  },
                  {
                    title: t("scenes.nodeSettings.cacheSettings.table.dataflowFiles"),
                    field: "cachedDataflow",
                    type: "numeric",
                    editable: 'never'
                  },
                  {
                    title: t("scenes.nodeSettings.cacheSettings.table.dataflowSize"),
                    field: "cacheSize",
                    type: "numeric",
                    editable: 'never',
                    render: rowData => rowData.cacheSize + " Kb"
                  },
                  {
                    title: t("scenes.nodeSettings.cacheSettings.table.dataflowAccess"),
                    field: "cachedDataAccess",
                    type: "numeric",
                    editable: 'never'
                  }

                ]}
                data={config?.filter(val => !hideEmptyDataflowCache || val.cacheSize > 0) || []}
                editable={{
                  onRowUpdate: (newData, oldData) =>
                    new Promise((resolve, reject) => {
                      if (newData.ttl < 0) { // invalid ttl
                        reject();
                        return;
                      }

                      if (newData.ttl === oldData.ttl) { //no update is needed here
                        resolve();
                        return;
                      }

                      const dataUpdate = [...config];
                      const index = oldData.tableData.id;
                      dataUpdate[index] = newData;

                      if (!newData.id || newData.id === '00000000-0000-0000-0000-000000000000') {
                        createCache(nodeId, newData);
                      } else {
                        updateCache(nodeId, newData.id, newData.ttl)
                      }


                      // config([...dataUpdate]);

                      resolve();
                    }),
                  onRowDelete: oldData =>
                    new Promise((resolve, reject) => {
                      const dataDelete = [...config];
                      const index = oldData.tableData.id;
                      dataDelete.splice(index, 1);

                      //data does not exists. no need to make a server call
                      if (!oldData.id || oldData.id === '00000000-0000-0000-0000-000000000000') {
                        resolve();
                        return;
                      }

                      deleteFromCache(nodeId, oldData.id)
                      //setCacheConfig([...dataDelete]);

                      resolve();
                    }),
                }}
                /*actions={[
                  {
                    icon: EditIcon,
                    tooltip: 'Edit node',
                    onClick: (_, {id}) => {window.notImplemented.show();}
                  },
                  {
                    icon: DeleteIcon,
                    tooltip: 'Delete node',
                    onClick: (_, {id}) => {window.notImplemented.show();}
                  }]}
                 */
                options={{
                  paging: false,
                  draggable: true,
                  actionsColumnIndex: -1,
                  searchFieldAlignment: "left",
                  //maxBodyHeight: 400,
                  showTitle: false
                }}
                localization={{
                  body: {
                    editRow: {
                      deleteText: t("scenes.nodesSettings.cacheSettings.modals.deleteDataflowCache.title")
                    },
                    editTooltip: t("scenes.nodeSettings.cacheSettings.table.actions.editRow"),
                    deleteTooltip: t("scenes.nodeSettings.cacheSettings.table.actions.deleteRow")
                  }
                }}
              />
            </Grid>

          </Paper>

        </Grid>
      </Grid>
    </Grid>


  );
};

export default compose(withStyles(styles), withTheme, compose(connect(mapStateToProps, mapDispatchToProps, null, {forwardRef: true}),
  forwardRef))(CacheSettingsForm);
