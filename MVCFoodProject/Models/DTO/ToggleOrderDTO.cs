using System.Text.Json.Serialization;

namespace MVCFoodProject.Models.DTO
{
    public class ToggleOrderDTO
    {
        public int Id { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ACTION action { get; set; }

        public enum ACTION
        {
            take,
            untake
        }
    }
}
