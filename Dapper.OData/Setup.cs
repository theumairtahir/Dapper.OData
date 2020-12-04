using Dapper.OData.Infrastructure;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;

namespace Dapper.OData
{
    public static class Setup
    {
        public static void AddDapperOData(this IServiceCollection services, IEdmModel edmModel, string conString, int connectionTimeout = 5)
        {
            //services.AddOData(opt => opt
            //                        .AddModel("oData", edmModel)
            //                        .Filter()
            //                        .Select()
            //                        .Count()
            //                        .OrderBy()
            //                        .Expand());
            services.AddOData();
            services.AddSingleton<IDbConfiguration>(x => new DbConfiguration(conString, connectionTimeout));
            services.AddSingleton<ITryCatch, TryCatch>();
            services.AddTransient<IDbConnection, DbConnection>();
        }
        //public static void UseODataEndpoints(this IEndpointRouteBuilder endpoints, IEdmModel edmModel)
        //{
        //    endpoints.EnableDependencyInjection();
        //    endpoints.Select().Filter().OrderBy().Count().MaxTop(10);
        //}
    }
}
