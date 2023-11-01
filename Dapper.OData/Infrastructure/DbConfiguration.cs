namespace Dapper.OData.Infrastructure;

internal interface IDbConfiguration
{
    string ConnectionString { get; }
    int CommandTimeout { get; }
}
internal class DbConfiguration : IDbConfiguration
{
    public DbConfiguration(string connectionString, int commandTimeout)
    {
        ConnectionString = connectionString;
        CommandTimeout = commandTimeout;
    }
    public string ConnectionString { get; init; }

    public int CommandTimeout { get; init; }
}