namespace MVCFoodProject.Models.DTO
{
    public class EditProfileDTO 
    {
        public string? Name { get; set; }
        public string? Number { get; set; }
        public string? Address { get; set; }
        public IFormFile? image { get; set; }
    }
}
