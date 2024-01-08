using static MVCFoodProject.Models.DataBase.Users;

namespace MVCFoodProject.Models.DataBase
{
    public class Courier
    {
        [Key]
        public int Id { get; set; }

        public ICollection<Orders>? Order { get; set; } = new List<Orders>();

        public Status status { get; set; } = Status.free;

        public Users? User { get; set; }

        public enum Status
        {
            free,
            busy
        }
    }
}
