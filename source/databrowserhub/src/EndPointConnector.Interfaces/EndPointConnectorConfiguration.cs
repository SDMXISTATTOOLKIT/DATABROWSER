namespace EndPointConnector.Interfaces
{
    public class EndPointConnectorConfiguration
    {
        public NodeRepositoryConfig NodeRepository { get; set; }
        public bool Sdmx { get; set; }
        public bool Spod { get; set; }
    }

    public class NodeRepositoryConfig
    {
        public string ConnectionString { get; set; }
        public string Type { get; set; }
    }
}