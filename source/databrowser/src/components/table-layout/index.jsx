import React, {Fragment} from "react";
import {compose} from "redux";
import {withTranslation} from "react-i18next";
import {DragDropContext, Draggable, Droppable} from "react-beautiful-dnd";
import withStyles from "@material-ui/core/styles/withStyles";
import CardHeader from "@material-ui/core/CardHeader";
import Grid from "@material-ui/core/Grid";
import Paper from "@material-ui/core/Paper";
import Typography from "@material-ui/core/Typography";
import Card from "@material-ui/core/Card";
import FilterListIcon from '@material-ui/icons/FilterList';
import ViewCompactIcon from '@material-ui/icons/ViewCompact';
import ViewStreamIcon from '@material-ui/icons/ViewStream';
import ViewColumnIcon from '@material-ui/icons/ViewColumn';
import JsonStatTable from "../table";
import Divider from "@material-ui/core/Divider";
import {Tooltip} from "@material-ui/core";

const styles = theme => ({
  root: {},
  sectionContainer: {
    display: "inline-block",
    verticalAlign: "middle",
    width: "calc(60% - 8px)",
    marginRight: 16
  },
  section: {
    height: 216,
    background: "#eeeeee"
  },
  sectionHeaderContainer: {
    height: 48,
    padding: "16px 16px 0"
  },
  sectionHeaderIcon: {
    display: "inline-block",
    verticalAlign: "middle",
    marginRight: 8
  },
  sectionHeaderTitle: {
    display: "inline-block",
    verticalAlign: "middle",
    height: 30,
  },
  sectionContentWrapper: {
    width: "100%",
    height: "calc(100% - 48px)",
    padding: 16
  },
  draggingHover: {
    border: "1px dashed #999999"
  },
  sectionContent: {
    width: "100%",
    height: "100%",
    overflowY: "auto",
    overflowX: "hidden"
  },
  sectionItem: {
    textAlign: "center",
    height: "100%",
    padding: "4px 16px"
  },
  previewContainer: {
    display: "inline-block",
    verticalAlign: "middle",
    width: "calc(40% - 8px)"
  },
  divider: {
    marginBottom: 16
  }
});

const getItemStyle = (isDragging, draggableStyle) => ({
  userSelect: 'none',
  height: 32,
  marginBottom: 8,
  ...draggableStyle
});

const reorder = (list, startIndex, endIndex) => {
  const [removed] = list.splice(startIndex, 1);
  list.splice(endIndex, 0, removed);
};

const move = (source, destination, droppableSource, droppableDestination) => {
  const [removed] = source.splice(droppableSource.index, 1);
  destination.splice(droppableDestination.index, 0, removed);
};

const onDragEnd = (result, dimensions, setDimensions) => {

  const {
    source,
    destination
  } = result;

  // dropped outside the list
  if (!destination) {
    return;
  }

  const newDimensions = JSON.parse(JSON.stringify(dimensions));

  if (source.droppableId === destination.droppableId) {
    reorder(
      newDimensions[source.droppableId],
      source.index,
      destination.index
    );

  } else {
    move(
      newDimensions[source.droppableId],
      newDimensions[destination.droppableId],
      source,
      destination
    );
  }

  setDimensions(newDimensions);
};

const Section = ({
                   classes,
                   id,
                   title,
                   Icon,
                   dimensions,
                   jsonStat
                 }) =>

  <Card variant="outlined" className={classes.section}>
    <CardHeader
      title={
        <Fragment>
          <div className={classes.sectionHeaderIcon}>{Icon}</div>
          <Typography className={classes.sectionHeaderTitle} variant="button">{title}</Typography>
        </Fragment>
      }
      className={classes.sectionHeaderContainer}
    />
    <Droppable droppableId={id}>
      {(provided, snapshot) => (
        <div className={`${classes.sectionContentWrapper} ${snapshot.isDraggingOver ? classes.draggingHover : ""}`}>
          <div ref={provided.innerRef} className={classes.sectionContent}>
            {dimensions.map((item, idx) =>
              <Draggable
                key={idx}
                index={idx}
                draggableId={`${id}-${idx}`}
              >
                {(provided, snapshot) => (
                  <div
                    ref={provided.innerRef}
                    {...provided.draggableProps}
                    {...provided.dragHandleProps}
                    style={getItemStyle(snapshot.isDragging, provided.draggableProps.style)}
                  >
                    <Tooltip title={jsonStat.dimension[item].label}>
                      <Paper className={classes.sectionItem}>
                        {`${item} (${jsonStat.size[jsonStat.id.indexOf(item)]})`}
                      </Paper>
                    </Tooltip>
                  </div>
                )}
              </Draggable>
            )}
            {provided.placeholder}
          </div>
        </div>
      )}
    </Droppable>
  </Card>;

const TableLayout = ({
                       t,
                       classes,
                       jsonStat,
                       layout,
                       labelFormat,
                       setLayout
                     }) => {
  const {
    filters,
    sections,
    rows,
    cols,
  } = layout;

  let colCount = 1;
  cols.forEach(col => colCount *= jsonStat.size[jsonStat.id.indexOf(col)]);
  let rowCount = 1;
  rows.forEach(row => rowCount *= jsonStat.size[jsonStat.id.indexOf(row)]);
  sections.forEach(section => rowCount *= jsonStat.size[jsonStat.id.indexOf(section)]);

  return (
    <div className={classes.root}>
      <Divider className={classes.divider}/>
      <div className={classes.sectionContainer}>
        <DragDropContext onDragEnd={result => onDragEnd(result, layout, setLayout)}>
          <Grid container spacing={2}>
            <Grid item xs={6}>
              <Section
                classes={classes}
                id="filters"
                title={t("components.tableLayout.sections.filters")}
                Icon={<FilterListIcon/>}
                dimensions={filters}
                jsonStat={jsonStat}
              />
            </Grid>
            <Grid item xs={6}>
              <Section
                classes={classes}
                id="sections"
                title={t("components.tableLayout.sections.sections")}
                Icon={<ViewCompactIcon/>}
                dimensions={sections}
                jsonStat={jsonStat}
              />
            </Grid>
            <Grid item xs={6}>
              <Section
                classes={classes}
                id="rows"
                title={t("components.tableLayout.sections.rows")}
                Icon={<ViewStreamIcon/>}
                dimensions={rows}
                jsonStat={jsonStat}
              />
            </Grid>
            <Grid item xs={6}>
              <Section
                classes={classes}
                id="cols"
                title={t("components.tableLayout.sections.columns")}
                Icon={<ViewColumnIcon/>}
                dimensions={cols}
                jsonStat={jsonStat}
              />
            </Grid>
          </Grid>
        </DragDropContext>
      </div>
      <div className={`${classes.previewContainer}`}>
        <JsonStatTable
          jsonStat={jsonStat}
          layout={layout}
          labelFormat={labelFormat}
          isPreview={true}
        />
        <Grid container spacing={1} style={{marginTop: 16}}>
          <Grid item xs={12} style={{textAlign: "end"}}>
            {t("components.tableLayout.rowCount", {rowCount})}
          </Grid>
          <Grid item xs={12} style={{textAlign: "end"}}>
            {t("components.tableLayout.colCount", {colCount})}
          </Grid>
        </Grid>
      </div>
    </div>
  )
};

export default compose(
  withStyles(styles),
  withTranslation()
)(TableLayout)