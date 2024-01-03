using static MVCFoodProject.Models.DataBase.Users;

namespace MVCFoodProject.Models.DataBase
{
    public class Courier
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public Orders? Order { get; set; }

        public string imgURl { get; set; }

        public Status status { get; set; } = Status.free;

        public enum Status
        {
            free,
            busy
        }
    }
}
