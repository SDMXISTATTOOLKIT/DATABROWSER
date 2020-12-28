namespace DataBrowser.Interfaces.Dto
{
    public class GenericJsonResponse : ResponseMessage
    {
        public GenericJsonResponse(object objectResult, bool success = false, string message = "") : base(success,
            message)
        {
            ObjectResult = objectResult;
        }

        public object ObjectResult { get; }
    }
}