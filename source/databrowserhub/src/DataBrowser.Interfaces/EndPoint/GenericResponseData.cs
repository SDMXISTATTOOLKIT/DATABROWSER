using System.Collections.Generic;

namespace DataBrowser.Interfaces.EndPoint
{
    public class GenericResponseData<T>
    {
        public T Data { get; set; }
        public long ItemsCount { get; set; }
        public long? ItemsFrom { get; set; }
        public long? ItemsTo { get; set; }
        public long ItemsMax { get; set; }
        public bool LimitExceeded { get; set; }
        public bool HaveError { get; set; }
        public bool NotFoundResource { get; set; }
        public bool NotAcceptableDataResponse { get; set; }
        public List<string> Errors { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseType { get; set; }
        public Dictionary<string, string> Timers { get; set; }
    }
}