namespace Dapper.OData.Infrastructure
{
    internal interface IDbConfiguration
    {
        string ConnectionString { get; }
        int ConnectionTimeout { get; }
    }
    internal class DbConfiguration : IDbConfiguration
    {

        public DbConfiguration(string connectionString, int connectionTimeout)
        {
            ConnectionString = connectionString;
            ConnectionTimeout = connectionTimeout;
        }
        public string ConnectionString { get; init; }

        public int ConnectionTimeout { get; init; }
    }
}
