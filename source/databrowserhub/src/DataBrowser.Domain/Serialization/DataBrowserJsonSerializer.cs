using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBrowser.Domain.Serialization
{
    public sealed class DataBrowserJsonSerializer
    {
        public static readonly JsonSerializerSettings SettingsDictionaryCamlCase = new JsonSerializerSettings
        {
            ContractResolver = new CamelCaseExceptDictionaryKeysResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };

        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCaseExceptDictionaryKeysResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };

        public static T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static string SerializeObject(List<object> o, bool dictionaryCamlCase = true)
        {
            return JsonConvert.SerializeObject(o, dictionaryCamlCase ? SettingsDictionaryCamlCase.Formatting : Settings.Formatting, dictionaryCamlCase ? SettingsDictionaryCamlCase : Settings);
        }

        public static string SerializeObject(object o, bool dictionaryCamlCase = true)
        {
            return JsonConvert.SerializeObject(o, dictionaryCamlCase ? SettingsDictionaryCamlCase.Formatting : Settings.Formatting, dictionaryCamlCase ? SettingsDictionaryCamlCase : Settings);
        }

        class CamelCaseExceptDictionaryKeysResolver : CamelCasePropertyNamesContractResolver
        {
            protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
            {
                JsonDictionaryContract contract = base.CreateDictionaryContract(objectType);

                contract.DictionaryKeyResolver = propertyName => propertyName;

                return contract;
            }
        }
    }
}
