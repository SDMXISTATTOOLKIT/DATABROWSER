import Alert from "@material-ui/lab/Alert";
import {getUserErrorsTranslations} from "../../constants/getUserErrorsTranslations";
import React from "react";
import {useTranslation} from "react-i18next";

const UserErrors = ({errors}) => {

  const {t} = useTranslation();

  return errors !== null
    ? (
      errors.map((str, index) =>
        <Alert
          severity="error"
          style={{
            marginBottom: index + 1 < errors.length ? 8 : 16
          }}
          key={index}
        >
          {getUserErrorsTranslations(t)[str] || t("errors.user.generic")}
        </Alert>
      ))
    : null;
}

export default UserErrors;