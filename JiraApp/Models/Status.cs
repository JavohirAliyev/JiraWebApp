using System.Text.Json.Serialization;

namespace JiraApp.Models
{
    public class Status
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
