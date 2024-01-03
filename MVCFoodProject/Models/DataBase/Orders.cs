namespace MVCFoodProject.Models.DataBase
{
    public class Orders
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string UserId { get; set; }

        public string CourierID { get; set; }

        public ICollection<ProductOrders> ProductOrders { get; set; }

        public int TotalPrice { get; set; }

        public Status status { get; set; } = Status.Taken;

        public enum Status
        {
            Taken,
            Canceled,
            Completed
        }
    }
}
