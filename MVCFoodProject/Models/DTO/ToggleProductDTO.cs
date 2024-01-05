using System.Text.Json.Serialization;

namespace MVCFoodProject.Models.DTO
{
    public class ToggleProductDTO
    {
        public string internalId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ACTION action { get; set; }


        public enum ACTION
        {
            activate,
            disactivate
        }
    }
}
