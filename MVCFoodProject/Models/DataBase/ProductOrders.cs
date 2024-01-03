namespace MVCFoodProject.Models.DataBase
{
    public class ProductOrders
    {
        [Key]
        public int Id { get; set; }

        public Products? Product { get; set; }

        public int Quantity { get; set; }

        public int Total { get; set; }

        public Orders? Order { get; set; }
    }
}
