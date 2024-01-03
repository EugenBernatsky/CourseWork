namespace MVCFoodProject.Models.DataBase
{
    public class Users
    {
        [Key]

        public int Id { get; set; }

        public string Name { get; set; }

        public string? imgURL { get; set; }

        public string? Number { get; set; }

        public string? Adress { get; set; }

        public string UID { get; set; }

        public Orders? UserOrders { get; set; }

        public UserRole Role { get; set; } = UserRole.Customer;

        public enum UserRole
        {
            Admin,
            Customer,
            Courier
        }

    }
}
