using Dapper.OData.Infrastructure;
using Dapper.OData.Models;
using Dapper.OData.Sample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Linq;

namespace Dapper.OData.Sample.Controllers
{
    public class OrdersController : ControllerBase
    {
        private readonly IDbConnection _db;

        public OrdersController(IDbConnection db)
        {
            _db = db;
        }
        [EnableQuery]
        public IQueryable<Order> GetOrders(DbParams dbParams)
        {
            var query = @"SELECT [order_id]
                          ,[customer_id]
                          ,[order_status]
                          ,[order_date]
                          ,[required_date]
                          ,[shipped_date]
                          ,[store_id]
                          ,[staff_id]
                      FROM [ODataTestDb].[sales].[orders]";
            //var orders = _db.GetQueryableResult<Order>(query, out _, filter: dbParams.DbFilter, top: dbParams.Top);
            var orders = _db.GetQueryableResult<Order>(query, out _, filter: dbParams.Filter, top: dbParams.Top, skip: dbParams.Skip, take: dbParams.Take, orderBy: dbParams.OrderBy);
            return orders;
        }
    }
}
