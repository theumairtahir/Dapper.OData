# Dapper.OData

Enables OData support for a .Net 5 WEBAPI using dapper.

## Installation

Use the package manager to install Dapper.OData.

```bash
Install-Package Dapper.OData -Version 1.0.0
```
Use the .Net CLI to install Dapper.OData.

```bash
dotnet add package Dapper.OData --version 1.0.0
```
Use the Package Reference to install Dapper.OData.

```bash
<PackageReference Include="Dapper.OData" Version="1.0.0" />
```
Use the Packet CLI to install Dapper.OData.

```bash
paket add Dapper.OData --version 1.0.0
```

## Usage

In Startup.cs

```csharp
using Dapper.OData;

public void ConfigureServices(IServiceCollection services)
{

    services.AddControllers();
    ...
    services.AddRouting();
    services.AddDapperOData(GetEdmModel(), Configuration.GetConnectionString("DefaultConnection")); //Add this line
}
```
Creating EDM Model
```csharp
private static IEdmModel GetEdmModel()
{
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    ...
    builder.EntitySet<Order>(nameof(Order) + "s").EntityType.HasKey(x => x.Order_Id);
    ...
    var edmModel = builder.GetEdmModel();
    return edmModel;
}
```

In a Sample Controller
```csharp
using Dapper.OData.Infrastructure;
using Dapper.OData.Models;

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
        var query = "{your query}";
        var orders = _db.GetQueryableResult<Order>(query, out _, filter: dbParams.Filter, top: dbParams.Top, skip: dbParams.Skip, take: dbParams.Take, orderBy: dbParams.OrderBy);
        return orders;
    }
}
```

## Query Data
You can use OData query options ($select, $filter, $count, $top, $skip, $orderby) as well as you can take query result directly from the database by using custom paramters as filter, top, skip, take, orderBy.

## Operator Grammer
These Operators will be used to add filter for the database

| Operator | Description |
| ----------- | ----------- |
| NOT | Not Operator |
| EQ | Equals Operator |
| NE | Not Equals Operator |
| GT | Greater Than |
| LT | Less Than |
| GE | Greater Than and Equals To |
| LE | Less Than and Equals To |
| BTW | Between |
| LK | Like |
| AND | And Operator |
| OR | Or Operator |
| IN(...) | In Operator |
| 'string' | Any string will be placed in single qoutes |

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.


## License
[MIT](LICENSE)
