namespace MVCFoodProject.Models.ViewModels
{
    public class CourierPageViewModel
    {
        public List<Orders> Order { get; set; }   

        public List<Users> Users { get; set; }

        public Users User { get; set; }

        public Courier Courier { get; set; }
    }
}
