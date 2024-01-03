﻿namespace MVCFoodProject.Models.DataBase
{
    public class Orders
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public Users User { get; set; }

        public Courier? Courier { get; set; }

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
