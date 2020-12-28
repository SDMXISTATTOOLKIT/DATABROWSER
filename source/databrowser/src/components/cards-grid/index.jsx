import React from 'react';
import Grid from "@material-ui/core/Grid";
import Card from "@material-ui/core/Card";
import CardMedia from "@material-ui/core/CardMedia";
import CardContent from "@material-ui/core/CardContent";
import Typography from "@material-ui/core/Typography";
import CardActionArea from "@material-ui/core/CardActionArea";
import {withStyles} from "@material-ui/core";

const styles = () => ({
  card: {
    height: "100%"
  }
});

const CardsGrid = ({list, onClick, classes, squareImage}) =>
  <Grid container spacing={2}>
    {(() => {
      const hasImage = squareImage && list.find(c => c.image);
      return list.map((c, i) =>
        <Grid key={i} item xs={hasImage ? 3 : 4}>
          <Card className={classes.card} id={`cards-grid__card-${c.id}`}>
            <CardActionArea onClick={() => onClick(c)}>
              {c.image && (
                <CardMedia
                  component="img"
                  height={hasImage ? 232 : (squareImage ? 315 : 177)}
                  title={c.label}
                  image={c.image}
                  alt={c.label}
                />
              )}
              <CardContent>
                <Typography gutterBottom variant="h5" style={{fontSize: 20}}>
                  {c.label}
                </Typography>
              </CardContent>
            </CardActionArea>
          </Card>
        </Grid>
      );
    })()}
  </Grid>;

export default withStyles(styles)(CardsGrid);