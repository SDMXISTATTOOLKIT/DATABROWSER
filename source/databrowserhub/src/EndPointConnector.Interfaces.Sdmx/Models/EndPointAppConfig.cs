namespace EndPointConnector.Interfaces.Sdmx.Models
{
    public class EndPointAppConfig
    {
        public bool TracingQueryData { get; set; }
        public Noderepository NodeRepository { get; set; }

        public class Noderepository
        {
            public string Type { get; set; }
            public string ConnectionString { get; set; }
        }
    }
}