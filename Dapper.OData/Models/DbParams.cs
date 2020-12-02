namespace Dapper.OData.Models
{
    public record DbParams(string Filter = null, int? Top = null, int? Skip = null, int? Take = null, string OrderBy = null);
}
