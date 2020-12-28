namespace DataBrowser.Interfaces
{
    public abstract class ResponseMessage
    {
        protected ResponseMessage(bool success = false, string message = null)
        {
            Success = success;
            Message = message;
        }

        public bool Success { get; }
        public string Message { get; }
    }
}