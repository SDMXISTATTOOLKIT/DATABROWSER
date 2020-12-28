import React, {Fragment} from 'react';
import ArrowDownwardIcon from '@material-ui/icons/ArrowDownward';
import ArrowUpwardIcon from '@material-ui/icons/ArrowUpward';
import IconButton from "@material-ui/core/IconButton";
import "./style.css";

export const SORT_DIRECTION_ASC = 'SORT_DIRECTION_ASC';
export const SORT_DIRECTION_DESC = 'SORT_DIRECTION_DESC';

const SortButtons = ({
                       value,
                       onChange
                     }) =>
  <Fragment>
    <div className="infinite-scroll-table__header__sort-buttons__icon-container">
      <IconButton
        onClick={() => onChange(value !== SORT_DIRECTION_ASC ? SORT_DIRECTION_ASC : SORT_DIRECTION_DESC)}
        style={{padding: 4, top: -4}}
      >
        {
          value === SORT_DIRECTION_ASC
            ? <ArrowUpwardIcon className="infinite-scroll-table__header__icon"/>
            : <ArrowDownwardIcon className="infinite-scroll-table__header__icon"/>
        }
      </IconButton>
    </div>
  </Fragment>;

export default SortButtons;
