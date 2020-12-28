using System;
using System.Collections.Generic;
using System.Linq;

namespace EndPointConnector.JsonStatParser.Adapters.Commons
{
    public class LocalizedString : Dictionary<string, string>
    {

        public new string this[string i]
        {
            get
            {
                if (string.IsNullOrEmpty(i)) {
                    throw new ArgumentException("Value cannot be null or empty.", nameof(i));
                }

                return base[i.ToLower()];
            }
            set
            {
                if (string.IsNullOrEmpty(i)) {
                    throw new ArgumentException("Value cannot be null or empty.", nameof(i));
                }

                base[i.ToLower()] = value;
            }
        }

        public const string DefaultLanguage = "en";

        public LocalizedString(string value, string language = DefaultLanguage)
        {
            this[language.ToLower()] = value;
            AddDefaultValueIfNotExists();
        }

        public LocalizedString(Dictionary<string, string> dict)
        {
            if (dict != null) {
                foreach (var k in dict.Keys)
                    this[k.ToLower()] = dict[k];
            }

            AddDefaultValueIfNotExists();
        }

        public string TryGet(string lang)
        {
            if (TryGetValue(lang.ToLower(), out var res)) {
                return res;
            }

            return Count > 0 ? this[Keys.First()] : null;
        }

        protected void AddDefaultValueIfNotExists()
        {
            if (Count > 0 && !ContainsKey(DefaultLanguage)) {
                this[DefaultLanguage] = this[Keys.ToArray()[0]];
            }
        }

    }
}