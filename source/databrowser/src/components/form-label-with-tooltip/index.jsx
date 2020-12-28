import React, {Fragment} from 'react';
import InfoTooltip from "../info-tooltip";

const FormLabelWithTooltip = ({children, tooltip, tooltipOnRight}) => {

  const Tooltip = () =>
    <span
      style={{
        pointerEvents: "auto",
        marginLeft: tooltipOnRight ? 10 : null,
        marginRight: !tooltipOnRight ? 10 : null,
      }}
    >
        <InfoTooltip>
          {tooltip}
        </InfoTooltip>
     </span>;

  return (
    <Fragment>
      {!tooltipOnRight && <Tooltip/>}
      <span style={{pointerEvents: "none"}}>
        {children}
      </span>
      {tooltipOnRight && <Tooltip/>}
    </Fragment>
  );
};

export default FormLabelWithTooltip;