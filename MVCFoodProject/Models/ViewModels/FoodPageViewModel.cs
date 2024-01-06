namespace MVCFoodProject.Models.ViewModels
{
    public class FoodPageViewModel
    {
        public List<Products> Products { get; set; }

        public UserProfile? UserProfile { get; set; } = null;
    }
}
