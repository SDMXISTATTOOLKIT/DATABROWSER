import React, {Fragment} from "react";
import {compose} from "redux";
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
import Divider from "@material-ui/core/Divider";
import {withTranslation} from "react-i18next";

const styles = theme => ({
  root: {},
  sectionContainer: {
    display: "inline-block",
    verticalAlign: "middle",
    width: "calc(60% - 8px)",
    marginRight: 16
  },
  section: {
    background: "#eeeeee"
  },
  sectionLeft: {
    height: 416
  },
  sectionRight: {
    height: 200
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

const swap = (sourceArr, destinationArr, droppableSource) => {
  const [removedSrc] = sourceArr.splice(droppableSource.index, 1);
  const [removedDst] = destinationArr.splice(0, 1, removedSrc);
  sourceArr.splice(sourceArr.length, 0, removedDst);
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

  } else if (
    (source.droppableId === "secondaryDim" && destination.droppableId === "filters") ||
    (source.droppableId === "filters" && destination.droppableId === "secondaryDim" && newDimensions[destination.droppableId].length === 0)
  ) {
    move(
      newDimensions[source.droppableId],
      newDimensions[destination.droppableId],
      source,
      destination
    );

  } else {
    swap(
      newDimensions[source.droppableId],
      newDimensions[destination.droppableId],
      source
    );
  }

  setDimensions(newDimensions);
};

const Section = ({
                   classes,
                   sectionClasses,
                   id,
                   title,
                   Icon,
                   dimensions,
                   jsonStat
                 }) =>

  <Card variant="outlined" className={`${classes.section} ${sectionClasses}`}>
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
                isDragDisabled={id === "primaryDim"}
              >
                {(provided, snapshot) => (
                  <div
                    ref={provided.innerRef}
                    {...provided.draggableProps}
                    {...provided.dragHandleProps}
                    style={getItemStyle(snapshot.isDragging, provided.draggableProps.style)}
                  >
                    <Paper
                      className={classes.sectionItem}
                    >
                      {`${item} (${jsonStat.size[jsonStat.id.indexOf(item)]})`}
                    </Paper>
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

const ChartLayout = ({
                       t,
                       classes,
                       jsonStat,
                       layout,
                       setLayout
                     }) => {

  const {
    primaryDim,
    secondaryDim,
    filters
  } = layout;

  return (
    <div className={classes.root}>
      <Divider className={classes.divider}/>
      <div className={classes.sectionContainer}>
        <DragDropContext onDragEnd={result => onDragEnd(result, layout, setLayout)}>
          <Grid container spacing={2}>
            <Grid item xs={6}>
              <Section
                classes={classes}
                sectionClasses={classes.sectionLeft}
                id="filters"
                title={t("components.chartLayout.filters.title")}
                Icon={<FilterListIcon/>}
                dimensions={filters}
                jsonStat={jsonStat}
              />
            </Grid>
            <Grid item xs={6}>
              <Grid item xs={12} style={{paddingBottom: 8}}>
                <Section
                  classes={classes}
                  sectionClasses={classes.sectionRight}
                  id="primaryDim"
                  title={t("components.chartLayout.primaryDim.title")}
                  Icon={<ViewCompactIcon/>}
                  dimensions={primaryDim}
                  jsonStat={jsonStat}
                />
              </Grid>
              <Grid item xs={12} style={{paddingTop: 8}}>
                <Section
                  classes={classes}
                  sectionClasses={classes.sectionRight}
                  id="secondaryDim"
                  title={t("components.chartLayout.secondaryDim.title")}
                  Icon={<ViewStreamIcon/>}
                  dimensions={secondaryDim}
                  jsonStat={jsonStat}
                />
              </Grid>
            </Grid>
          </Grid>
        </DragDropContext>
      </div>
    </div>
  )
};

export default compose(
  withStyles(styles),
  withTranslation()
)(ChartLayout)