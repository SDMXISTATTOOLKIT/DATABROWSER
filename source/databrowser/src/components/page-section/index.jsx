import React, {Fragment} from 'react';
import Typography from "@material-ui/core/Typography";
import Divider from "@material-ui/core/Divider";
import Box from "@material-ui/core/Box";

const PageSection = props =>
  <Box {...props}>
    {props.sectiontitle && (
      <Fragment>
        <Typography variant="h4" gutterBottom margin={4} style={{fontSize: 34}}>
          {props.sectiontitle}
        </Typography>
        <Divider/>
      </Fragment>
    )}
    {props.children}
  </Box>;

export default PageSection;