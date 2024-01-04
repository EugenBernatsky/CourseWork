namespace MVCFoodProject.Models.ViewModels
{
    public class CourierPageViewModel
    {
        public List<Orders> Order { get; set; }   

        public List<Users> User { get; set; }

        public Courier Courier { get; set; }
    }
}
