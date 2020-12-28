using System;
using System.Collections.Generic;
using System.Linq;

namespace DataBrowser.AC.Utility.Helpers.ExtMethod
{
    public static class Translates
    {
        public static string GetTranslateItem(this Dictionary<string, string> dic, string lang,
            string getDefaultLangForNotFound = null, string defaultValue = null)
        {
            if (dic == null) return defaultValue;

            var selectLang = dic.FirstOrDefault(i => i.Key.Equals(lang, StringComparison.InvariantCultureIgnoreCase));
            if (selectLang.Equals(default(KeyValuePair<string, string>)) &&
                string.IsNullOrWhiteSpace(getDefaultLangForNotFound))
            {
                selectLang = dic.FirstOrDefault(i => i.Key.Equals(getDefaultLangForNotFound, StringComparison.InvariantCultureIgnoreCase));
            }
            if (selectLang.Equals(default(KeyValuePair<string, string>))) selectLang = dic.FirstOrDefault();
            if (selectLang.Equals(default(KeyValuePair<string, string>)))
            {
                return defaultValue;
            }
            return selectLang.Value;
        }

        public static int? GetTranslateItem(this Dictionary<string, int?> dic, string lang,
            string getDefaultLangForNotFound = null)
        {
            if (dic == null) return null;

            var selectLang = dic.FirstOrDefault(i => i.Key.Equals(lang, StringComparison.InvariantCultureIgnoreCase));
            if (selectLang.Equals(default(KeyValuePair<string, int?>)) &&
                string.IsNullOrWhiteSpace(getDefaultLangForNotFound))
                selectLang = dic.FirstOrDefault(i =>
                    i.Key.Equals(getDefaultLangForNotFound, StringComparison.InvariantCultureIgnoreCase));
            if (selectLang.Equals(default(KeyValuePair<string, int?>))) selectLang = dic.FirstOrDefault();
            return selectLang.Value;
        }

        public static string GetOnlySpecificTranslateItem(this Dictionary<string, string> dic, string lang, string defaultValue = null)
        {
            if (dic == null) return defaultValue;

            var selectLang = dic.FirstOrDefault(i => i.Key.Equals(lang, StringComparison.InvariantCultureIgnoreCase));
            if (selectLang.Equals(default(KeyValuePair<string, string>))) return defaultValue;
            return selectLang.Value;
        }

        public static List<string> GetOnlySpecificTranslateItem(this Dictionary<string, List<string>> dic, string lang)
        {
            if (dic == null) return null;

            var selectLang = dic.FirstOrDefault(i => i.Key.Equals(lang, StringComparison.InvariantCultureIgnoreCase));
            if (selectLang.Equals(default(KeyValuePair<string, string>))) return null;
            return selectLang.Value;
        }

        public static bool GetOnlySpecificTranslateItem(this Dictionary<string, bool> dic, string lang)
        {
            if (dic == null) return false;

            var selectLang = dic.FirstOrDefault(i => i.Key.Equals(lang, StringComparison.InvariantCultureIgnoreCase));
            if (selectLang.Equals(default(KeyValuePair<string, bool>))) return false;
            return selectLang.Value;
        }
    }
}