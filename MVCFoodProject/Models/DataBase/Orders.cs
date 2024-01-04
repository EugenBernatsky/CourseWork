namespace MVCFoodProject.Models.DataBase
{
    public class Orders
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int? UserId { get; set; }

        public virtual Users User { get; set; }

        public int? CourierId { get; set; }

        public virtual Courier? Courier { get; set; }

        public ICollection<ProductOrders> ProductOrders { get; set; }

        public int TotalPrice { get; set; }

        public Status status { get; set; } = Status.Created;

        public enum Status
        {
            Taken,
            Canceled,
            Completed,
            Created
        }
    }
}
