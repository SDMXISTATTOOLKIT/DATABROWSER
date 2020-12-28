using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DataBrowser.Interfaces.Constants
{
    [JsonConverter(typeof(StringEnumConverter))]
    public static class PolicyName
    {
        public const string ManageCache = "ManageCache";
        public const string ManageTemplate = "ManageTemplate";
        public const string ManageConfig = "ManageConfig";
        public const string ManageView = "ManageView";
        public const string ViewPrivateData = "ViewPrivateData";
        public const string UploadFile = "UploadFile";
        public const string ServiceUser = "ServiceUser";
        public const string AuthenticatedUser = "AuthenticatedUser";
    }
}