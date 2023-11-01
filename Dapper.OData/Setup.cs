using Dapper.OData.Infrastructure;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;

namespace Dapper.OData;

public static class Setup
{
    public static void AddDapperOData(this IMvcBuilder builder, IEdmModel edmModel, string conString, int connectionTimeout = 5, ClientType client = ClientType.SqlClient, bool matchNamesWithUnderscores = false)
    {
        builder.AddOData(opt => opt
            .Filter()
            .Select()
            .Count()
            .OrderBy()
            .Expand()
            .SetMaxTop(null)
            .AddRouteComponents("odata", edmModel));
        builder.Services.AddSingleton<IDbConfiguration>(x => new DbConfiguration(conString, connectionTimeout));
        builder.Services.AddSingleton<ITryCatch, TryCatch>();
        switch (client)
        {
            case ClientType.SqlClient:
                builder.Services.AddTransient<IDbConnection, DbConnection>();
                break;
            case ClientType.OracleClient:
                builder.Services.AddTransient<IDbConnection, Infrastructure.Oracle.DbConnection>();
                break;
        }
        DefaultTypeMap.MatchNamesWithUnderscores = matchNamesWithUnderscores;
    }

}
/// <summary>
/// Type of database client
/// </summary>
public enum ClientType
{
    /// <summary>
    /// MS SQL Server Client
    /// </summary>
    SqlClient,
    /// <summary>
    /// Oracle Db Client
    /// </summary>
    OracleClient
}