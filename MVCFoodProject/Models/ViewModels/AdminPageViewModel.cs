namespace MVCFoodProject.Models.ViewModels
{
    public class AdminPageViewModel
    {
        public List<Courier> Courier { get; set; } = new List<Courier> { };

        public List<Products> Products { get; set; } = new List<Products> { };

        public List<Users> UsersList { get; set; } = new List<Users> { };

        public List<Courier> Couriers { get; set; } = new List<Courier> { };

        public Users Me { get; set; } = new Users { };
    }
}
