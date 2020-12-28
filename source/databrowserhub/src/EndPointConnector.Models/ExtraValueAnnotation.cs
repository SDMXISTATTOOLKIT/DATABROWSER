using System;
using System.Collections.Generic;
using System.Text;

namespace EndPointConnector.Models
{
    public class ExtraValueAnnotation : ExtraValue
    {
        public static readonly string AnnotationExtraType = "ANNOTATION";

        public static readonly string AnnotationIdKey = "id";

        public static readonly string AnnotationTypeKey = "type";

        public static readonly string AnnotationTextKey = "__text_";

        public static readonly string AnnotationTitleKey = "title";

        public ExtraValueAnnotation(string id) : base()
        {
            Key = id;
            Type = AnnotationExtraType;
            Values = new Dictionary<string, string>();
            Values[AnnotationIdKey] = Key;
            Values[AnnotationTypeKey] = Key;
            Values[AnnotationTitleKey] = Key;
        }

        public void SetAnnotationType(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                Values[AnnotationTypeKey] = Key;
            }            
        }

        public string GetAnnotationType()
        {
            return Values[AnnotationTypeKey];
        }

        public void AddTextList(Dictionary<string,string> textList)
        {
            foreach (var entry in textList)
            {
                AddText(entry.Value, entry.Key);
            }
        }

        public Dictionary<string, string> GetAllText()
        {
            var result = new Dictionary<string, string>();
            foreach (var entry in Values)
            {
                if (entry.Key.StartsWith(AnnotationTextKey))
                {
                    var lang = entry.Key.Replace(AnnotationTextKey, "");
                    result[lang] = entry.Value;
                }
            }
            return result;
        }

        public void AddText(string text, string language)
        {
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(language))
            {
                var textKey = AnnotationTextKey + language.ToUpper();
                Values[textKey] = text;
            }
        }

        public string GetText(string language)
        {
            if (!string.IsNullOrEmpty(language))
            {
                var textKey = AnnotationTextKey + language.ToUpper();
                Values.TryGetValue(textKey, out string result);
                return result;
            }
            return null;
        }

        public void SetAnnotationTitle(string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                Values[AnnotationTitleKey] = title;
            }
        }

        public string GetAnnotationTitle()
        {
            var titleKey = AnnotationTitleKey;
            Values.TryGetValue(titleKey, out string result);
            return result;
        }        

        public static bool IsAnnotationExtra(ExtraValue extraValue)
        {
            if (extraValue?.Type != null && extraValue.Type == AnnotationExtraType)
            {
                return true;
            }
            return false;
        }

        public static string ExtractAnnotationType(ExtraValue extraValue)
        {
            if (!IsAnnotationExtra(extraValue))
            {
                throw new Exception("ExtraValue is not an ExtraValueAnnotation");
            }
            return ((ExtraValueAnnotation)extraValue).GetAnnotationType();
        }

        public static string ExtractAnnotationTitle(ExtraValue extraValue)
        {
            if (!IsAnnotationExtra(extraValue))
            {
                throw new Exception("ExtraValue is not an ExtraValueAnnotation");
            }
            return ((ExtraValueAnnotation)extraValue).GetAnnotationTitle();
        }

        public static string ExtractText(ExtraValue extraValue, string language)
        {
            if (!IsAnnotationExtra(extraValue))
            {
                throw new Exception("ExtraValue is not an ExtraValueAnnotation");
            }
            return ((ExtraValueAnnotation)extraValue).GetText(language);
        }

        public static Dictionary<string, string> ExtractAllText(ExtraValue extraValue)
        {
            if (!IsAnnotationExtra(extraValue))
            {
                throw new Exception("ExtraValue is not an ExtraValueAnnotation");
            }
            var result = new Dictionary<string, string>();
            foreach (var entry in extraValue?.Values)
            {
                if (entry.Key.StartsWith(AnnotationTextKey))
                {
                    var lang = entry.Key.Replace(AnnotationTextKey, "");
                    result[lang] = entry.Value;
                }
            }
            return result;
        }

    }
}
