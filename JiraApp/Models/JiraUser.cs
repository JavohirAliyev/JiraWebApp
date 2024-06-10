using System.Text.Json.Serialization;

namespace JiraApp.Models
{
    public class JiraUser
    {
        [JsonPropertyName("accountId")]
        public string AccountId { get; set; }
    }
}
