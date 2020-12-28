import React, {Fragment, useState} from 'react';
import "./style.css";
import AutoSearchInput from "../../auto-search-input";
import IconButton from "@material-ui/core/IconButton";
import Popover from "@material-ui/core/Popover";
import Checkbox from '@material-ui/core/Checkbox';
import Grid from '@material-ui/core/Grid';
import FilterListIcon from '@material-ui/icons/FilterList';

const FilterButton = ({
                        value,
                        onChange,
                        options
                      }) => {

  const [anchorEl, setAnchorEl] = useState(null);

  const handleOpen = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  return (
    <Fragment>

      <div className="infinite-scroll-table__header__filter-button__icon-container">
        <IconButton
          onClick={handleOpen}
          style={{padding: 4, top: -4}}
        >
          <FilterListIcon className="infinite-scroll-table__header__icon"/>
        </IconButton>
      </div>

      <Popover
        open={Boolean(anchorEl)}
        anchorEl={anchorEl}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'center',
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'center',
        }}
        onClose={handleClose}
      >
        {options && options.length
          ? (
            options.map((option, index) =>
              <Grid
                container
                className="infinite-scroll-table__header__filter-button__option"
                key={index}
              >
                <Grid item>
                  <Checkbox
                    checked={(value || []).find(val => val === option.value) !== undefined}
                    onChange={({target}) => target.checked
                      ? onChange([...(value || []), option.value])
                      : onChange((value || []).filter(val => val !== option.value))
                    }
                  />
                </Grid>
                <Grid item>
                  <div style={{height: 42, lineHeight: "42px", marginRight: 16}}>
                    {option.text}
                  </div>
                </Grid>
              </Grid>
            )
          )
          : (
            <div style={{padding: 8}}>
              <AutoSearchInput onSearch={onChange} value={value}/>
            </div>
          )
        }
      </Popover>

    </Fragment>
  )
};

export default FilterButton;
