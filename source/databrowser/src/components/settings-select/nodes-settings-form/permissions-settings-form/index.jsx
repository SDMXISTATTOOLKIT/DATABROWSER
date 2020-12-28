import React, {forwardRef, Fragment, useEffect, useImperativeHandle, useState} from 'react';
import Search from "@material-ui/icons/Search";
import Clear from "@material-ui/icons/Clear";
import ArrowUpward from "@material-ui/icons/ArrowUpward";
import {Box, withStyles} from "@material-ui/core";
import {compose} from "redux";
import {connect} from "react-redux";
import {
  clearNodePermissionsConfig,
  fetchNodePermissionsConfig,
  sendNodePermissionsConfig
} from "../../../../state/nodePermissionsConfig/nodePermissionsConfigActions";
import Checkbox from "@material-ui/core/Checkbox";
import {useForm} from "react-hook-form";
import Grid from "@material-ui/core/Grid";
import {useTranslation} from "react-i18next";
import CustomMaterialTable from "../../../custom-material-table";

const tableIcons = {
  Search: forwardRef((props, ref) => <Search {...props} ref={ref}/>),
  ResetSearch: forwardRef((props, ref) => <Clear {...props} ref={ref}/>),
  SortArrow: forwardRef((props, ref) => <ArrowUpward {...props} ref={ref}/>),
};

const styles = theme => ({
  tableToolbar: {
    marginBottom: theme.spacing(1)
  },
  buttonAction: {
    ...theme.typography.button
  }
});

const PERMISSIONS = [
  "ManageCache",
  "ManageTemplate",
  "ManageConfig"
];

const getPermissionsTranslations = t => ({
  ManageCache: t("scenes.nodeSettings.permissions.table.columns.manageCache"),
  ManageTemplate: t("scenes.nodeSettings.permissions.table.columns.manageTemplate"),
  ManageConfig: t("scenes.nodeSettings.permissions.table.columns.manageConfig")
});

const Form = compose(withStyles(styles), forwardRef)(({classes, config, nodeId, onSubmit, onCancel}, ref) => {

  const {register, handleSubmit, watch, setValue} = useForm({
    defaultValues: {
      config: config?.map((user, index) => {
        let res = {...user};
        PERMISSIONS.forEach(p => {
          res[p] = config[index].permission.includes(`${p}_SingleNode_${nodeId}`) === true;
        });
        return res;
      })
    }
  });

  const {t} = useTranslation();

  useEffect(() => {
    if (config) {
      config.forEach((user, index) =>
        PERMISSIONS.forEach(p =>
          register({name: `config[${index}].${p}`})))
    }
  }, [register, config]);

  useImperativeHandle(ref, () => ({
    submit(f) {
      handleSubmit(val => {

        const data = val.config.map((userVal, index) => {
          let res = {
            userId: config[index].userId,
            nodePermissions: []
          };
          PERMISSIONS.forEach(p => {
            if (userVal[p] === true) {
              res.nodePermissions.push(p);
            }
          });
          res.tableData = undefined;
          return res;
        });

        onSubmit(nodeId, data);
        f(data);
      })();
    },
    cancel(f) {
      onCancel();
      f();
    }
  }));

  return config && (
    <Fragment>
      <CustomMaterialTable
        components={{
          Container: Box
        }}
        columns={[
          {
            field: "firstName",
            title: t("scenes.nodeSettings.permissions.table.columns.userFirstName")
          },
          {
            field: "lastName",
            title: t("scenes.nodeSettings.permissions.table.columns.userLastName")
          },
          {
            field: "organization",
            title: t("scenes.nodeSettings.permissions.table.columns.userOrganization")
          },
          {
            field: "email",
            title: t("scenes.nodeSettings.permissions.table.columns.userEmail")
          },
          ...PERMISSIONS.map(p => ({
            title: getPermissionsTranslations(t)[p],
            render: ({tableData}) =>
              <Grid container justify="center">
                <Grid item>
                  <Checkbox
                    checked={watch(`config[${tableData.id}].${p}`)}
                    onChange={(ev, value) => setValue(`config[${tableData.id}].${p}`, value)}
                  />
                </Grid>
              </Grid>
          }))
        ]}
        data={config}
        options={{
          paging: false,
          draggable: true,
          searchFieldAlignment: "left",
          maxBodyHeight: 400,
          showTitle: false
        }}
        icons={tableIcons}
      />
    </Fragment>
  );
});

const mapStateToProps = state => ({
  config: state.nodePermissionsConfig.permissions
});

const mapDispatchToProps = dispatch => ({
  fetchConfig: nodeId => dispatch(fetchNodePermissionsConfig(nodeId)),
  sendConfig: (nodeId, config) => dispatch(sendNodePermissionsConfig(nodeId, config)),
  clearConfig: () => dispatch(clearNodePermissionsConfig())
});

const PermissionsSettingsForm = ({config, nodeId, fetchConfig, sendConfig, clearConfig}, ref) => {

  const [needConfig, setNeedConfig] = useState(nodeId !== null);

  useEffect(() => {

    if (needConfig) {
      setNeedConfig(false);
      fetchConfig(nodeId);
    }
  }, [config, needConfig, setNeedConfig, fetchConfig, nodeId]);

  return ((nodeId === null || config) && (
    <Form
      nodeId={nodeId}
      config={nodeId === null ? undefined : config}
      ref={ref}
      onSubmit={sendConfig}
      onCancel={clearConfig}
    />
  ));
};


export default compose(withStyles(styles), connect(mapStateToProps, mapDispatchToProps, null, {forwardRef: true}), forwardRef)(PermissionsSettingsForm);