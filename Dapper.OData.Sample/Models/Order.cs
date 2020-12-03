using System;

namespace Dapper.OData.Sample.Models
{
    public class Order
    {
        public int Order_Id { get; set; }
        public int Customer_Id { get; set; }
        public int Order_Status { get; set; }
        public DateTime Order_Date { get; set; }
        public DateTime Required_Date { get; set; }
        public DateTime? Shipped_Date { get; set; }
        public int Store_Id { get; set; }
        public int Staff_Id { get; set; }
        public Customer Customer { get; set; }
    }
}
