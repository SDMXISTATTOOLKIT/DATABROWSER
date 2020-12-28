export const USER_ERRORS_INVALID_LOGIN = "InvalidLogin";

export const getUserErrorsTranslations = t => {

  const translations = t => ({
    DuplicateEmail: t("errors.user.duplicateEmail"),
    DuplicateUserName: t("errors.user.duplicateUsername"),
    InvalidLogin: t("errors.user.invalidLogin"),
    PasswordRequiresDigit: t("errors.user.passwordRequiresDigit"),
    PasswordTooShort: t("errors.user.passwordTooShort"),
    PasswordRequiresNonAlphanumeric: t("errors.user.passwordRequiresNonAlphanumeric"),
    PasswordRequiresUpper: t("errors.user.passwordRequiresUpper"),
    PasswordRequiresLower: t("errors.user.passwordRequiresLower"),
    InvalidEmail: t("errors.user.invalidEmail"),
    InvalidToken: t("errors.user.invalidToken"),
    DefaultError: t("errors.user.defaultError"),
    ConcurrencyFailure: t("errors.user.concurrencyFailure"),
    PasswordMismatch: t("errors.user.passwordMismatch"),
    LoginAlreadyAssociated: t("errors.user.loginAlreadyAssociated"),
    InvalidUserName: t("errors.user.invalidUserName"),
    InvalidRoleName: t("errors.user.invalidRoleName"),
    DuplicateRoleName: t("errors.user.duplicateRoleName"),
    UserAlreadyHasPassword: t("errors.user.userAlreadyHasPassword"),
    UserLockoutNotEnabled: t("errors.user.userLockoutNotEnabled"),
    UserAlreadyInRole: t("errors.user.userAlreadyInRole"),
    UserNotInRole: t("errors.user.userNotInRole"),
  });

  return translations(t !== undefined ? t : str => str);
};
