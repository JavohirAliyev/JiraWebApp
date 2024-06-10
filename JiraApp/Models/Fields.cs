using System.Text.Json.Serialization;

namespace JiraApp.Models
{
    public class Fields
    {
        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("status")]
        public Status Status { get; set; }

        [JsonPropertyName("reporter")]
        public Reporter Reporter { get; set; }

    }
}
