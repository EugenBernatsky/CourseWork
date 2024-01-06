namespace MVCFoodProject.Models.Auth
{
    public class RegisterModel
    {
        public string username { get; set; }
        public string password { get; set; }
        
        public string email { get; set; }

        public string? phone { get; set; }

        public string? address { get; set;}
    }
}
