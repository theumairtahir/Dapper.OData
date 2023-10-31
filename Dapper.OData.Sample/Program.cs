using Dapper.OData;
using Dapper.OData.Sample.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using OData.Swagger.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddDapperOData(GetEdmModel(), builder.Configuration.GetConnectionString("OracleConnection"), client: ClientType.OracleClient);
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dapper.OData.Sample", Version = "v1" });
    c.DocInclusionPredicate((name, api) => api.HttpMethod != null);
});
builder.Services.AddRouting();
builder.Services.AddOdataSwaggerSupport();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dapper.OData.Sample v1"));
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();
app.MapDefaultControllerRoute();
return;

static IEdmModel GetEdmModel()
{
    ODataConventionModelBuilder builder = new();
    builder.EntitySet<Product>(nameof(Product) + "s").EntityType.HasKey(x => x.Product_Id);
    builder.EntitySet<Order>(nameof(Order) + "s").EntityType.HasKey(x => x.Order_Id);
    builder.EntitySet<Customer>(nameof(Customer) + "s").EntityType.HasKey(x => x.Customer_Id);
    return builder.GetEdmModel();
}