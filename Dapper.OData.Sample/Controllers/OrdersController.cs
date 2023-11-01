﻿using Dapper.OData.Infrastructure;
using Dapper.OData.Models;
using Dapper.OData.Sample.Models;
using Microsoft.AspNetCore.OData.Query;
using System.Linq;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Dapper.OData.Sample.Controllers;

public class OrdersController : ODataController
{
    readonly IDbConnection _db;

    public OrdersController(IDbConnection db)
    {
        _db = db;
    }
    [EnableQuery]
    public IQueryable<Order> GetOrders(DbParams dbParams)
    {
        var query = @"SELECT order_id
                          ,o.customer_id
                          ,order_status
                          ,order_date
                          ,required_date
                          ,shipped_date
                          ,store_id
                          ,staff_id
	                      ,first_name
                          ,last_name
                          ,phone
                          ,email
                          ,street
                          ,city
                          ,state
                          ,zip_code
                      FROM orders o
                      inner join customers c on c.customer_id=o.customer_id";
        //var orders = _db.GetQueryableResult<Order>(query, out _, filter: dbParams.DbFilter, top: dbParams.Top);
        var orders = _db.GetQueryableResult<Order, Customer>(query, (x, y) =>
        {
            x.Customer = y;
            return x;
        }, "first_name", out _, filter: dbParams.Filter, top: dbParams.Top, skip: dbParams.Skip, take: dbParams.Take, orderBy: dbParams.OrderBy);
        return orders;
    }
}