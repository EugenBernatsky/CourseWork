namespace MVCFoodProject.Models.ViewModels
{
    public class CustomerPageViewModel
    {
        public List<Courier> Courier { get; set; } = new List<Courier> { };
        public List<Products> Products { get; set; } = new List<Products> { };

        public List<Orders> Order { get; set; }

        public Users User { get; set; }
    }
}
