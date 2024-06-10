using System.Text.Json.Serialization;

namespace JiraApp.Models
{
    public class Reporter
    {
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
    }
}
