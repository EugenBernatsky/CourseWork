using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Text.Json.Serialization;

namespace MVCFoodProject.Models.ViewModels
{
    public class OrderViewModel
    {
       
        public int OrderId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int TotalPrice { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Orders.Status Status { get; set; }

        public ICollection<ProductOrders> OrderProducts { get; set; }

        public Courier? Courier { get; set; }

        public Users User { get; set; }

        public OrderViewModel (Orders order)
        {
            OrderId = order.Id;
            Status = order.status;
            CreatedDate = order.CreatedDate; 
            TotalPrice = order.TotalPrice;
            OrderProducts = order.ProductOrders;
            Courier = order.Courier;
            User = order.User;

        }
    }
}
