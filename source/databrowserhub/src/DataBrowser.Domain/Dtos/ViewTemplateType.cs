using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBrowser.Domain.Dtos
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ViewTemplateType
    {
        View,
        Template
    }
}
