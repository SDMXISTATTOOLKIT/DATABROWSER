import React, {Fragment} from 'react';
import {compose} from "redux";
import {connect} from "react-redux";
import {withStyles} from "@material-ui/core";
import {CKEditor} from '@ckeditor/ckeditor5-react';
import Editor from '@ckeditor/ckeditor5-build-custom'
import "./style.css"

import '@ckeditor/ckeditor5-build-custom/build/translations/it';

const styles = theme => ({
  preToolbar: {
    marginBottom: theme.spacing(1)
  }
});

class HtmlEditor extends React.Component {

  constructor(props) {
    super(props);
    this.state = HtmlEditor.getInitState(props);
    this.handleEditorChange = this.handleEditorChange.bind(this);
  }

  static getInitState(props) {
    return {
      value: props ? (props.value || '') : ''
    };
  }

  static getDerivedStateFromProps(props) {
    if (props.value !== null && props.value !== undefined) {
      return {
        value: props.value
      };

    } else {
      return HtmlEditor.getInitState(props);
    }
  }

  handleEditorChange(event, editor) {
    const data = editor.getData();
    if (this.props.onChange) {
      this.props.onChange(data);
    }
  }

  render() {

    const {
      value
    } = this.state;

    const {
      defaultLanguage,
      disabled
    } = this.props;

    return (
      <Fragment>
        {/* <Grid container className={classes.preToolbar}>
          <Grid item>
            <I18nInputAdornmentSelect/>
          </Grid>
        </Grid>*/}
        <CKEditor
          data={value}
          editor={Editor}
          config={{
            language: defaultLanguage
          }}
          disabled={disabled}
          onChange={this.handleEditorChange}
        />
      </Fragment>
    );
  }
}

export default compose(
  withStyles(styles),
  connect(state => ({
    defaultLanguage: state.app.language
  }))
)(HtmlEditor);
