namespace DataBrowser.Interfaces.Configuration
{
    public class DatabaseConfig
    {
        public string DbType { get; set; }
        public string ConnectionString { get; set; }
        public bool EnableQueryOptimizer { get; set; }
        public string QueryOptimizerDbType { get; set; }
        public string QueryOptimizerConnectionString { get; set; }
        public bool UseMigrationScript { get; set; }
    }
}