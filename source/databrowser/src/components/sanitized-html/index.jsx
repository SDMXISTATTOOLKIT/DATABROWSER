import React from 'react';
import {sanitize} from "dompurify";
import Box from "@material-ui/core/Box";

const SanitizedHTML = ({html, allowTarget, ...props}) =>
  <Box
    {...props}
    html={undefined}
    dangerouslySetInnerHTML={{
      __html: sanitize(
        html,
        {
          ADD_ATTR: [allowTarget ? "target" : ""]
        }
      )
    }}
  />;

export default SanitizedHTML;