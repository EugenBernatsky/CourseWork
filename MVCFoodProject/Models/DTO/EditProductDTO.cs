namespace MVCFoodProject.Models.DTO
{
    public class EditProductDTO
    {
        public IFormFile? image {  get; set; }

        public string? description { get; set; }

        public string? productName { get; set; }

        public int? price { get; set; }
    }
}
