import React from 'react';
import MaterialTable from "material-table";
import _ from 'lodash';
import {useTranslation} from "react-i18next";

const CustomMaterialTable = props => {

  const {t} = useTranslation();

  // these props are meant to be used for customizing material table all spread the app
  const customProps = {
    localization: {
      body: {
        emptyDataSourceMessage: t("components.customMaterialTable.empty"),
      },
      header: {
        actions: ""
      },
      toolbar: {
        searchPlaceholder: t("components.customMaterialTable.search.placeholder"),
        searchTooltip: t("components.customMaterialTable.search.tooltip")
      }
    }
  };

  // props passed to this component are merged with and can override the custom props
  const mergedProps = _.merge(customProps, props);

  return <MaterialTable {...mergedProps}/>;
};

export default CustomMaterialTable;
