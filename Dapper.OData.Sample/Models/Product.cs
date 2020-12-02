using System.ComponentModel.DataAnnotations;

namespace Dapper.OData.Sample.Models
{
    public class Product
    {
        //[Key]
        public int Product_Id { get; set; }
        //[Required]
        public string Product_Name { get; set; }
        //[Required]
        public int Brand_Id { get; set; }
        //[Required]
        public int Category_Id { get; set; }
        //[Required]
        public short Model_Year { get; set; }
        //[Required]
        public decimal List_Price { get; set; }
    }
}
