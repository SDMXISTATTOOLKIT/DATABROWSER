import React from 'react';
import {withTranslation} from "react-i18next";
import Grid from "@material-ui/core/Grid";
import FormControl from "@material-ui/core/FormControl";
import I18nTextField from "../i18n-text-field";

const ViewBuilder = ({
                       t,
                       view,
                       onChange
                     }) =>
  <Grid container spacing={2}>
    <Grid item xs={12}>
      <FormControl fullWidth>
        <I18nTextField
          label={t("scenes.dataViewer.viewBuilder.form.title.label")}
          required
          variant="outlined"
          value={view.title}
          onChange={value => onChange({...view, title: value})}
        />
      </FormControl>
    </Grid>
  </Grid>

export default withTranslation()(ViewBuilder);