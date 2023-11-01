using Dapper.OData.Infrastructure;
using Dapper.OData.Sample.Models;
using Microsoft.AspNetCore.OData.Query;
using System.Linq;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Dapper.OData.Sample.Controllers;

public class ProductsController : ODataController
{
    readonly IDbConnection _db;

    public ProductsController(IDbConnection db)
    {
        _db = db;
    }
    [EnableQuery]
    public IQueryable<Product> GetProducts(string dbFilter = null)
    {
        var query = @"SELECT [product_id]
                          ,[product_name]
                          ,[brand_id]
                          ,[category_id]
                          ,[model_year]
                          ,[list_price]
                      FROM [ODataTestDb].[production].[products]";
        var products = _db.GetQueryableResult<Product>(query, out _, filter: dbFilter);
        return products;
    }

}