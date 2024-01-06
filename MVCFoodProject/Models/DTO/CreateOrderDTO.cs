namespace MVCFoodProject.Models.DTO
{

    public class CreateOrderItem
    {
        [Required]
        public string ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }

    public class CreateOrderDTO
    {
        [Required]
        public List<CreateOrderItem> Orders { get; set; } = new List<CreateOrderItem>();
    }
}
