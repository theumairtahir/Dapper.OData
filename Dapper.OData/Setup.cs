using Dapper.OData.Infrastructure;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;

namespace Dapper.OData
{
    public static class Setup
    {
        public static void AddDapperOData(this IServiceCollection services, IEdmModel edmModel, string conString, int connectionTimeout = 5, ClientType client = ClientType.SqlClient, bool matchNamesWithUnderscores = false)
        {
            services.AddOData(opt => opt
                                    .AddModel("oData", edmModel)
                                    .Filter()
                                    .Select()
                                    .Count()
                                    .OrderBy()
                                    .Expand());
            services.AddSingleton<IDbConfiguration>(x => new DbConfiguration(conString, connectionTimeout));
            services.AddSingleton<ITryCatch, TryCatch>();
            switch (client)
            {
                case ClientType.SqlClient:
                    services.AddTransient<IDbConnection, DbConnection>();
                    break;
                case ClientType.OracleClient:
                    services.AddTransient<IDbConnection, Infrastructure.Oracle.DbConnection>();
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
}
