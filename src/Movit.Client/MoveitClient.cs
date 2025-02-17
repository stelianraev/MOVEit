using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoveitApiClient.Extensions;
using MoveitApiClient.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MoveitApiClient
{
    public class MoveitClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MoveitClient> _logger;
        private readonly MoveitConfiguration _config;

        //private string _accessToken;
        //private readonly string _clientId;
        //private readonly string _clientSecret;

        public MoveitClient(HttpClient httpClient, ILogger<MoveitClient> logger, IOptions<MoveitConfiguration> config/*string baseUrl, string clientId, string clientSecret*/)
        {
            _httpClient = httpClient;
            _logger = logger;
            _config = config.Value;
            //_baseUrl = baseUrl;
            //_clientId = clientId;
            //_clientSecret = clientSecret;
        }

        public async Task<MoveitTokenResponse> AuthenticateAsync(string username, string password)
        {
            //var body = new {_username, _password }.CreateJsonContent();
            var requestData = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                //{ "client_id", _clientId },
                //{ "client_secret", _clientSecret },
                { "username", username },
                { "password", password }
            }.CreateJsonContent();

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_config.BaseUrl}/api/v1/token")
            {
                Content = requestData
            };

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to authenticate with MOVEit API. Status Code: {StatusCode}", response.StatusCode);
                throw new  Exception("MOVEit Authentication failed.");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            var tokenResponse = JsonSerializer.Deserialize<MoveitTokenResponse>(responseContent);
            return tokenResponse;            
        }
    }
}
