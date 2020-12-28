using System;
using System.Linq;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;

namespace EndPointConnector.JsonStatParser.StructureUtils.Annotations
{
    internal class AnnotationUtils
    {

        public static string GetLocalizedAnnotationText(IAnnotation annotation, string lang, string defaultLang)
        {
            if (annotation == null || annotation.Text?.Count == 0 && annotation.Title == null) {
                return null;
            }

            if (!(annotation.Text?.Count > 0)) {
                return annotation.Title;
            }

            // localized  value
            var localizedString = annotation.Text
                .FirstOrDefault(t => string.Equals(t.Locale, lang, StringComparison.CurrentCultureIgnoreCase))?.Value;

            if (localizedString != null) {
                return localizedString;
            }

            // default language
            var localizedDefaultString =
                annotation.Text.FirstOrDefault(t =>
                    string.Equals(t.Locale, defaultLang, StringComparison.CurrentCultureIgnoreCase))?.Value;

            return localizedDefaultString ?? annotation.Text[0].Value;
        }

    }
}