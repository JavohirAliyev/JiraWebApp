using System.Text.Json.Serialization;

namespace JiraApp.Models
{
    public class Issue
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("fields")]
        public Fields Fields { get; set; }
    }
}
