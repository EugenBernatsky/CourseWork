namespace MVCFoodProject.Models.DataBase
{
    public class Products
    {
        [Key]
        public int Id { get; set; }

        public string CategoryType { get; set; }

        public string InternalId { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ProductsDetails ProductsDetails { get; set; }

        public bool Deleted { get; set; } = false;
    }
}
