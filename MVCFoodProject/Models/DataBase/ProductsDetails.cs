using System.Collections;

namespace MVCFoodProject.Models.DataBase
{
    public class ProductsDetails
    {
        [Key]
        public int Id { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }

        public string imgURL { get; set; }
    }
}
