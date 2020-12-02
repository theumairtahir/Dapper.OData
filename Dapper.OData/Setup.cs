using Dapper.OData.Infrastructure;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;

namespace Dapper.OData
{
    public static class Setup
    {
        public static void AddDapperOData(this IServiceCollection services, IEdmModel edmModel, string conString, int connectionTimeout = 5)
        {
            services.AddOData(opt => opt
                                    .AddModel("oData", edmModel)
                                    .Filter()
                                    .Select()
                                    .Count()
                                    .OrderBy());
            services.AddSingleton<IDbConfiguration>(x => new DbConfiguration(conString, connectionTimeout));
            services.AddSingleton<ITryCatch, TryCatch>();
            services.AddTransient<IDbConnection, DbConnection>();
        }

    }
}
