namespace MVCFoodProject.Models.Auth
{
    public class UserProfile
    {
        public string userId { get; set; }

        public string? token { get; set; }  

        public string role { get; set; }

        // ToDo add other fields for user profile
    }
}
