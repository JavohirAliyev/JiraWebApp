using JiraApp.Models;
using System.Net.Sockets;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace JiraApp.Services
{

    public class JiraService
    {
        private readonly HttpClient _httpClient;

        public JiraService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://dotnetitransition.atlassian.net/rest/api/3/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes("")));
        }
        
        public async Task<string> GetAccountIdByEmail(string email)
        {
            var response = await _httpClient.GetAsync($"https://dotnetitransition.atlassian.net/rest/api/3/user/search?query={email}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var users = JsonSerializer.Deserialize<List<JiraUser>>(await response.Content.ReadAsStringAsync());

            return users.FirstOrDefault()?.AccountId;
        }

        public async Task<JiraResult<string>> CreateTicket(string summary, string priority, string userEmail, string collectionName, string pageLink)
        {
            var accountId = await GetAccountIdByEmail(userEmail);
            var result = new JiraResult<string>();

            if (accountId == null)
            {
                result.Errors.Add($"{userEmail} is not a part of Atlassian accounts. Please, create an Atlassian account first and join dotnetitransition.atlassian.com to start creating tickets");
                return result;
            }

            var payload = new
            {
                fields = new
                {
                    project = new { key = "KAN" },
                    summary = summary,
                    reporter = new { id = accountId },
                    description = new
                    {
                        type = "doc",
                        version = 1,
                        content = new[]
                        {
                    new
                    {
                        type = "paragraph",
                        content = new[]
                        {
                            new
                            {
                                type = "text",
                                text = $"Reported by: {userEmail}\nCollection: {collectionName}\nPage Link: {pageLink}"
                            }
                        }
                    }
                }
                    },
                    issuetype = new { name = "Task" },
                    priority = new { name = priority }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("issue", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                result.Errors.Add($"Failed to create Jira ticket: {response.StatusCode} - {responseContent}");
                return result;
            }

            result.Data = responseContent;
            return result;
        }


        public async Task<JiraResult<List<Ticket>>> GetTickets(string projectKey)
        {
            var result = new JiraResult<List<Ticket>>();

            try
            {
                var jqlQuery = Uri.EscapeDataString($"project = \"{projectKey}\"");
                var response = await _httpClient.GetAsync($"search?jql={jqlQuery}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    result.Errors.Add($"Failed to retrieve Jira tickets.");
                    return result;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var jiraIssues = JsonSerializer.Deserialize<JiraResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                result.Data = jiraIssues?.Issues.ConvertAll(issue => new Ticket
                {
                    Summary = issue.Fields.Summary,
                    Status = issue.Fields.Status.Name,
                    Reporter = issue.Fields.Reporter.DisplayName,
                    Url = $"https://dotnetitransition.atlassian.net/browse/{issue.Key}"
                });
            }
            catch (Exception ex)
            {
                result.Errors.Add("An error occurred while fetching tickets: " + ex.Message);
            }

            return result;
        }
    }
}
