namespace DataBrowser.AC.Utility
{
    public static class RequestAdapter
    {
        public static string ConvertDataflowUriToDataflowId(string requestDataflowId)
        {
            return requestDataflowId.Replace(',', '+');
        }

        public static string ConvertDataflowIdToUriFormat(string requestDataflowId)
        {
            return requestDataflowId.Replace('+', ',');
        }
    }
}