import React from "react";
import withStyles from "@material-ui/core/styles/withStyles";
import Grid from "@material-ui/core/Grid";
import Card from "@material-ui/core/Card";
import CardHeader from "@material-ui/core/CardHeader";
import {useTranslation} from "react-i18next";

const styles = theme => ({
  root: {},
  cardTitle: {
    fontSize: 17,
    fontWeight: "bold"
  },
  datasetAttributesCard: {
    background: "#F5F5F5"
  },
  datasetAttributes: {
    margin: 0,
    padding: "0 16px 16px"
  },
  seriesAttributes: {
    margin: 0,
    padding: "0 0 16px"
  },
  attributeGroup: {
    margin: 0,
    padding: "0 16px"
  }
});

const Attribute = ({attribute, list}) =>
  <Grid item xs={12}>
    {`${list ? "- " : ""}${attribute.id}: ${attribute.value}`}
  </Grid>;

function AttributeList(props) {

  const {
    classes,
    datasetAttributes,
    otherAttributes
  } = props;

  const {t} = useTranslation();

  return (
    <div className={classes.root}>
      {datasetAttributes.length > 0 && (
        <Card variant="outlined" className={classes.datasetAttributesCard}>
          <CardHeader
            title={
              <div className={classes.cardTitle}>
                {t("components.attributeList.datasetInformation.title")}:
              </div>
            }
            disableTypography
          />
          <Grid container spacing={2} className={classes.datasetAttributes}>
            {datasetAttributes.map((attribute, idx) =>
              <Attribute
                key={idx}
                attribute={attribute}
              />
            )}
          </Grid>
        </Card>
      )}
      {otherAttributes.length > 0 && (
        <Card variant="outlined" style={{marginTop: 16}}>
          <CardHeader
            title={
              <div className={classes.cardTitle}>
                {t("components.attributeList.seriesInformation.title")}:
              </div>
            }
            disableTypography
          />
          <Grid container spacing={2} className={classes.seriesAttributes}>
            {otherAttributes.map(({dims, attributes}, idx) =>
              <Grid
                container
                key={idx}
                spacing={2}
                className={classes.attributeGroup}
                style={{background: idx % 2 ? "#FFFFFF" : "#F5F5F5"}}
              >
                <Grid item xs={12}>
                  {dims.map(({id, value}, idx) =>
                    <div key={idx} style={{display: "inline-block", marginRight: 8}}>
                      <b>{id}</b>: <i>{value}</i>{idx < dims.length - 1 ? "," : ""}
                    </div>
                  )}
                </Grid>
                {attributes.map((attribute, idx) =>
                  <Attribute
                    key={idx}
                    attribute={attribute}
                    list
                  />
                )}
              </Grid>
            )}
          </Grid>
        </Card>
      )}
    </div>
  )
}

export default withStyles(styles)(AttributeList)