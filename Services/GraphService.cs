using ReactAspNetApp.Extensions;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReactAspNetApp.Services
{
    public class GraphService
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;

        private GraphService(HttpClient httpClient, string accessToken)
        {
            _httpClient = httpClient;
            _accessToken = accessToken;
        }

        public static async Task<GraphService> CreateOnBehalfOfUserAsync(string userToken, IConfiguration configuration)
        {
            var clientApp = ConfidentialClientApplicationBuilder
                .Create(configuration["AzureAd:ClientId"])
                .WithTenantId(configuration["AzureAd:TenantId"])
                .WithClientSecret(configuration["AzureAd:ClientSecret"])
                .Build();

            var authResult = await clientApp
                .AcquireTokenOnBehalfOf(new[] { configuration["GraphAPI:Scopes"] }, new UserAssertion(userToken))
                .ExecuteAsync();

            var httpClient = new HttpClient();
            return new GraphService(httpClient, authResult.AccessToken);
        }

        public async Task<IEnumerable<string>> CheckMemberGroupsAsync(IEnumerable<string> groupIds)
        {
            var batchSize = 20;
            var allMemberGroups = new List<string>();

            foreach (var groupsBatch in groupIds.Batch(batchSize))
            {
                var requestBody = new { groupIds = groupsBatch.ToList() };
                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

                var response = await _httpClient.PostAsync(
                    "https://graph.microsoft.com/v1.0/me/checkMemberGroups",
                    content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(responseContent);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("value", out JsonElement valueElement))
                    {
                        foreach (var item in valueElement.EnumerateArray())
                        {
                            if (item.GetString() is string groupId)
                            {
                                allMemberGroups.Add(groupId);
                            }
                        }
                    }
                }
            }

            return allMemberGroups;
        }
    }
}
