using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoveitApiClient.Models;
using System.Net.Http.Headers;
using System.Web;

namespace MoveitApiClient
{
    public class MoveitClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<MoveitClient> _logger;
        private readonly MoveitConfiguration _config;

        public MoveitClient(IHttpClientFactory clientFactory, ILogger<MoveitClient> logger, IOptions<MoveitConfiguration> config)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _config = config.Value;

            _httpClient = _clientFactory.CreateClient();
        }

        public async Task<HttpResponseMessage> GetToken(string username, string password)
        {
            var requestData = new Dictionary<string, string>
                {
                      { "grant_type", "password" },
                      { "username", username },
                      { "password", password }
                };


            var request = new HttpRequestMessage(HttpMethod.Post, $"{_config.BaseUrl}token")
            {
                Content = new FormUrlEncodedContent(requestData)
            };

            var response = await _httpClient.SendAsync(request);

            return response;
        }

        public async Task<HttpResponseMessage> RevokeTokenAsync(string token)
        {
            var requestData = new Dictionary<string, string>
                {
                      { "token", token }
                };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_config.BaseUrl}token/revoke")
            {
                Content = new FormUrlEncodedContent(requestData)
            };

            var response = await _httpClient.SendAsync(request);
            return response;
        }

        public async Task<HttpResponseMessage> GetAllFiles(int page,
                                                           int perPage,
                                                           string sortField,
                                                           string sortDirection,
                                                           bool? newOnly,
                                                           string sinceDate,
                                                           string accessToken)
           {
            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["page"] = page.ToString();
            queryParams["perPage"] = perPage.ToString();
            queryParams["sortField"] = sortField;
            queryParams["sortDirection"] = sortDirection;

            if (newOnly.HasValue)
                queryParams["newOnly"] = newOnly.Value.ToString().ToLower();

            if (!string.IsNullOrEmpty(sinceDate))
                queryParams["sinceDate"] = sinceDate;

            string requestUrl = $"{_config.BaseUrl}files?{queryParams}";

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _httpClient.SendAsync(request);
        }
    }
}
