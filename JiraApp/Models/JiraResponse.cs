using System.Text.Json.Serialization;

namespace JiraApp.Models
{
    public class JiraResponse
    {
        [JsonPropertyName("issues")]
        public List<Issue> Issues { get; set; }
    }
}
