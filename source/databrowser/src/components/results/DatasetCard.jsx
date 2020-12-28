import React, {Fragment, useState} from "react";
import Typography from "@material-ui/core/Typography";
import CardActionArea from "@material-ui/core/CardActionArea";
import Grid from "@material-ui/core/Grid";
import Chip from "@material-ui/core/Chip";
import IconButton from "@material-ui/core/IconButton";
import ExpandLessIcon from "@material-ui/icons/ExpandLess";
import ExpandMoreIcon from "@material-ui/icons/ExpandMore";
import Card from "@material-ui/core/Card";
import CardContent from "@material-ui/core/CardContent";
import Avatar from "@material-ui/core/Avatar";
import DialogContent from "@material-ui/core/DialogContent";
import Dialog from "@material-ui/core/Dialog";
import List from "@material-ui/core/List";
import ListItem from "@material-ui/core/ListItem";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import {useTranslation} from "react-i18next";
import getDownloadButtonStyle from './getDownloadButtonStyle';
import withStyles from "@material-ui/core/styles/withStyles";
import SanitizedHTML from "../sanitized-html";

const styles = () => ({
  collapsedDescription: {
    marginTop: 8,
    height: 49,
    overflow: "hidden"
  },
  expandedDescription: {
    marginTop: 8
  },
  resultSource: {
    fontSize: 14,
    color: "rgba(0, 0, 0, 0.54)",
  },
  cardContent: {
    padding: "16px !important"
  },
  description: {
    "& > p": {
      margin: 0
    }
  },
  downloadChipDeleteIcon: {
    backgroundColor: "white"
  },
  counterAvatar: {
    width: 16,
    height: 16,
    marginLeft: -4,
    marginRight: 4
  }
});

const DatasetCard = ({
                       dataset,
                       classes,
                       isExpanded,
                       onViewDataset,
                       onExpand,
                       onCollapse,
                       xs
                     }) => {

  const [isDownloadModalOpen, setIsDownloadModalOpen] = useState(false);
  const [downloadModal, setDownloadModal] = useState(null);
  const {t} = useTranslation();

  const groupedAttachments = [];
  (dataset.attachedDataFiles || []).forEach(({format, url}) => {

    const group = groupedAttachments.find(({extension}) => extension === format.toLowerCase());

    if (!group) {
      groupedAttachments.push({
        extension: format.toLowerCase(),
        urls: [url]
      })
    } else {
      group.urls.push(url);
    }
  });

  const headerContent =
    <Fragment>
      <Grid container justify="space-between" spacing={2} alignItems="center">
        <Grid item>
          <Typography variant="h6">
            {dataset.title}
          </Typography>
        </Grid>
        {groupedAttachments.length > 0 && (
          <Grid item>
            <Grid container spacing={1}>
              {groupedAttachments
                .map((group, index) =>
                  <Grid item key={index}>
                    <Chip
                      className={`download-btn--${group.extension.toLowerCase()}`}
                      label={group.extension.toUpperCase()}
                      deleteIcon={
                        group.urls.length > 1
                          ? (
                            <Avatar style={{fontSize: 12, color: "black"}}
                                    classes={{root: classes.counterAvatar}}>
                              {group.urls.length}
                            </Avatar>
                          )
                          : null
                      }
                      onDelete={
                        group.urls.length > 1
                          ? e => {
                            e.stopPropagation();
                            setIsDownloadModalOpen(true);
                            setDownloadModal(group)
                          }
                          : null
                      }
                      size="small"
                      onClick={
                        e => {
                          e.stopPropagation();
                          if (group.urls.length > 1) {
                            setIsDownloadModalOpen(true);
                            setDownloadModal(group)
                          } else {
                            window.open(group.urls[0], "_blank")
                          }
                        }
                      }
                      style={{
                        cursor: "pointer",
                        ...getDownloadButtonStyle(group.extension)
                      }}
                      classes={{
                        deleteIconSmall: classes.downloadChipDeleteIcon
                      }}
                      aria-label={
                        t("scenes.results.datasetCard.downloadAria", {
                          title: dataset.title,
                          extension: group.extension.toUpperCase()
                        })
                      }
                    />
                  </Grid>
                )}
            </Grid>
          </Grid>
        )}
      </Grid>
      {dataset.source && (
        <Typography variant="body1" className={classes.resultSource}>
          {dataset.source}
        </Typography>
      )}
    </Fragment>;

  const header =
    dataset.catalogType !== "ONLY_FILE"
      ? (
        <CardActionArea
          onClick={onViewDataset}
        >
          {headerContent}
        </CardActionArea>
      )
      : headerContent;

  const description =
    (
      dataset.description ||
      (dataset.keywords && dataset.keywords.length > 0)
    ) && (
      <Typography
        component='div'
        variant="body1"
        className={isExpanded ? classes.expandedDescription : classes.collapsedDescription}
      >
        <SanitizedHTML html={dataset.description} className={classes.description}/>
        {dataset.keywords && dataset.keywords.length > 0 && (
          <Grid container spacing={1}>
            {dataset.keywords.map((keyword, index) =>
              <Grid item key={index}>
                <Chip
                  label={keyword}
                  size="small"
                />
              </Grid>
            )}
          </Grid>
        )}
      </Typography>
    );

  const expandIcon = <Grid container justify="flex-end">
    <Grid item>
      <IconButton
        onClick={isExpanded ? onCollapse : onExpand}
      >
        {isExpanded
          ? <ExpandLessIcon/>
          : <ExpandMoreIcon/>
        }
      </IconButton>
    </Grid>
  </Grid>;

  return (
    <Fragment>
      <Grid item xs={xs} id={encodeURIComponent(dataset.identifier)}>
        <Card>
          <CardContent className={classes.cardContent}>
            <Grid container>
              <Grid item xs={12}>
                {header}
              </Grid>
            </Grid>
            {description}
            {description && expandIcon}
          </CardContent>
        </Card>
      </Grid>
      <Dialog
        open={!!isDownloadModalOpen}
        disableEscapeKeyDown
        disableBackdropClick
        onClose={() => setIsDownloadModalOpen(false)}
        onExited={() => setDownloadModal(null)}
      >
        <DialogContent>
          <List>
            {downloadModal?.urls.map((url, key) =>
              <ListItem key={key}>
                <a href={url} target="_blank" rel="noopener noreferrer">{url}</a>
              </ListItem>
            )}
          </List>
          <DialogActions>
            <Button onClick={() => setIsDownloadModalOpen(false)}>
              {t("commons.confirm.close")}
            </Button>
          </DialogActions>
        </DialogContent>
      </Dialog>
    </Fragment>
  );
};

export default withStyles(styles)(DatasetCard);