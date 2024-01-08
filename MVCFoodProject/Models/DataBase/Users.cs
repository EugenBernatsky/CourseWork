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

        public string Email { get; set; }

        public string UID { get; set; }

        public ICollection<Orders>? UserOrders { get; set; }

        public UserRole Role { get; set; } = UserRole.Customer;

        public static implicit operator List<object>(Users v)
        {
            //For what?
            throw new NotImplementedException();
        }

        public enum UserRole
        {
            Admin,
            Customer,
            Courier
        }

    }
}
