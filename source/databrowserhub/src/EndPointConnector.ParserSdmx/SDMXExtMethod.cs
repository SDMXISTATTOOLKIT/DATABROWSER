using System;
using System.Collections.Generic;
using System.Linq;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;

namespace EndPointConnector.ParserSdmx
{
    public static class SDMXExtMethod
    {
        public static string GetTranslateItem(this IList<ITextTypeWrapper> dic, string lang,
            string getDefaultLangForNotFound = null)
        {
            if (dic == null) return null;

            var selectLang =
                dic.FirstOrDefault(i => i.Locale.Equals(lang, StringComparison.InvariantCultureIgnoreCase));
            if (selectLang == null && string.IsNullOrWhiteSpace(getDefaultLangForNotFound))
                selectLang = dic.FirstOrDefault(i =>
                    i.Locale.Equals(getDefaultLangForNotFound, StringComparison.InvariantCultureIgnoreCase));
            if (selectLang == null) selectLang = dic.FirstOrDefault();
            return selectLang?.Value;
        }

        public static Dictionary<string, string> GetAllTranslateItem(this IList<ITextTypeWrapper> dic)
        {
            if (dic == null) return null;

            return dic.ToDictionary(i => i.Locale, i => i.Value);
        }
    }
}