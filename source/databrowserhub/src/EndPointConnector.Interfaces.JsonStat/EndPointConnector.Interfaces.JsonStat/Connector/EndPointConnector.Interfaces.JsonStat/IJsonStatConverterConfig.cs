namespace EndPointConnector.Interfaces.JsonStat
{
    public interface ISDMXParsingConfig
    {
        public string OrderAnnotationId { get; set; }
        public string NotDisplayedAnnotationId { get; set; }
        public string GeoAnnotationId { get; set; }

        public abstract bool IsValidOrderAnnotation(string annotation);

        public abstract bool IsValidNotDisplayedAnnotation(string annotation);
    }
}