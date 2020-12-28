import React, {Fragment} from 'react';
import Grid from "@material-ui/core/Grid";
import List from "@material-ui/core/List";
import ListItem from "@material-ui/core/ListItem";
import ListItemText from "@material-ui/core/ListItemText";
import {withStyles} from '@material-ui/core/styles';
import Typography from "@material-ui/core/Typography";
import {Link} from "@material-ui/core";

const styles = theme => ({
  list: {
    paddingLeft: theme.spacing(1),
    paddingRight: theme.spacing(1)
  },
  cardContainer: {
    position: "relative"
  },
  card: {
    height: "100%",
    backgroundColor: theme.palette.background.default
  },
  item: {
    padding: 0
  },
  link: {
    cursor: "pointer"
  }
});

const CategoriesLists = ({classes, categories, onClick}) => {

  const rows = [];

  const chunkSize = 3;

  for (let i = 0; i < categories.length; i += chunkSize) {

    const categoriesChunk = categories.slice(i, i + chunkSize);

    /* first level */
    rows.push(
      <Grid key={i} container spacing={2}>
        {categoriesChunk.map((rootCategory, i) =>
          <Grid item key={i} xs={4}>
            <Typography variant="h5" className={classes.title} style={{fontSize: 20}}>
              {rootCategory.datasetIdentifiers?.length > 0
                ? (
                  <Link onClick={() => onClick([rootCategory.id])} className={classes.link}>
                    {rootCategory.label} ({rootCategory.datasetIdentifiers.length})
                  </Link>
                ) : rootCategory.label
              }
            </Typography>
          </Grid>
        )}
      </Grid>
    );

    const fontSizes = [18, 16, 15, 14];

    /* recursively renders subtree of categories */
    const Subtree = ({root, depth, fontSizes, parentPath = []}) =>
      root.childrenCategories && (
        <div className={classes.card}>
          <List className={classes.list}>
            {root.childrenCategories.map((child, i) =>
              <Fragment key={i}>
                <ListItem className={classes.item}>
                  <ListItemText
                    primary={
                      <Typography style={{fontSize: fontSizes[0]}}>
                        {child.datasetIdentifiers?.length > 0
                          ? (
                            <Link onClick={() => onClick([...parentPath, root.id, child.id])} className={classes.link}>
                              {child.label} ({child.datasetIdentifiers.length})
                            </Link>
                          ) : child.label}
                      </Typography>
                    }
                  />
                </ListItem>
                {/* recursion on subtree of child category */}
                {depth > 1 && (
                  <Subtree
                    root={child}
                    depth={depth - 1}
                    fontSizes={fontSizes.slice(1)}
                    parentPath={[...parentPath, root.id]}
                  />
                )}
              </Fragment>
            )}
          </List>
        </div>
      );

    /* 4 levels subtree */
    rows.push(
      <Grid key={i + 1} container spacing={2}>
        {categoriesChunk.map((rootCategory, i) =>
          <Grid item key={i} xs={4} className={classes.cardContainer}>
            <Subtree root={rootCategory} depth={4} fontSizes={fontSizes}/>
          </Grid>
        )}
      </Grid>
    );
  }

  return rows;
};

export default withStyles(styles)(CategoriesLists);
