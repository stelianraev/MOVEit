using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoveitApiClient.Models;
using MoveitApiClient.Models.ResponseModels;
using MoveitApiClient.Models.Responses;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Web;

namespace MoveitApiClient
{
    public class MoveitClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<MoveitClient> _logger;
        private readonly MoveitConfiguration _config;
        private readonly CancellationToken _cancelationToken;

        public MoveitClient(IHttpClientFactory clientFactory, ILogger<MoveitClient> logger, IOptions<MoveitConfiguration> config, CancellationTokenSource cancellationTokenSource)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _config = config.Value;
            _cancelationToken = cancellationTokenSource.Token;
            _httpClient = _clientFactory.CreateClient();
        }

        public async Task<TokenResponse> GetToken(string username, string password)
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
            var responseBody = await response.Content.ReadAsStringAsync();

            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);
            return tokenResponse;
        }

        public async Task<RevokeTokenResponse> RevokeTokenAsync(string token)
        {
            var requestData = new Dictionary<string, string>
                {
                      { "token", token }
                };

            var content = new FormUrlEncodedContent(requestData);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_config.BaseUrl}token/revoke")
            {
                Content = content
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request, _cancelationToken);
            var responseBody = await response.Content.ReadAsStringAsync();

            var revokeTokenResponse = JsonConvert.DeserializeObject<RevokeTokenResponse>(responseBody);
            revokeTokenResponse.StatusCode = (int)response.StatusCode;
            return revokeTokenResponse;
        }

        public async Task<GetFilesResponse> GetAllFilesAsync(int page,
                                                             int perPage,
                                                             string? sortField,
                                                             string? sortDirection,
                                                             bool? newOnly,
                                                             string? sinceDate,
                                                             string accessToken)
        {
            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["page"] = page.ToString();
            queryParams["perPage"] = perPage.ToString();
            queryParams["sortField"] = sortField;
            queryParams["sortDirection"] = sortDirection;

            if (newOnly.HasValue)
            {
                queryParams["newOnly"] = newOnly.Value.ToString().ToLower();
            }

            if (!string.IsNullOrEmpty(sinceDate))
            {
                queryParams["sinceDate"] = sinceDate;
            }

            string requestUrl = $"{_config.BaseUrl}files?{queryParams}";

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request, _cancelationToken);
            var responseBody = await response.Content.ReadAsStringAsync();

            var parsed = JsonConvert.DeserializeObject<GetFilesResponse>(responseBody);

            return parsed;
        }

        public async Task<HttpResponseMessage> DeleteFileAsync(string fileId, string accessToken)
        {
            var requestUrl = $"{_config.BaseUrl}files/{fileId}";
            using var request = new HttpRequestMessage(HttpMethod.Delete, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.SendAsync(request, _cancelationToken);
            return response;
        }


        public async Task<HttpResponseMessage> UploadFileAsync(string folderId,
                                                              Stream fileStrem,
                                                              string fileName,
                                                              string accessToken)
        {
            fileStrem.Seek(0, SeekOrigin.Begin);
            var fileSize = fileStrem.Length;
            var hashValue = string.Empty;

            using (var sha256 = SHA256.Create())
            {
                hashValue = Convert.ToHexStringLower(sha256.ComputeHash(fileStrem));
            }

            fileStrem.Seek(0, SeekOrigin.Begin);

            var isFileSizeExceedLimits = IsFileSizeExceedLimits(fileStrem);

            if (isFileSizeExceedLimits)
            {
                return new HttpResponseMessage(HttpStatusCode.RequestEntityTooLarge);
            }

            return await UploadFileSingleAsync(folderId, fileStrem, fileName, "sha-256", hashValue, accessToken);
        }

        private async Task<HttpResponseMessage> UploadFileSingleAsync(string folderId,
                                                                     Stream file,
                                                                     string fileName,
                                                                     string hashType,
                                                                     string hashValue,
                                                                     string accessToken)
        {
            var fileContent = new StreamContent(file);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"file\"",
                FileName = $"\"{fileName}\""
            };

            using var formData = new MultipartFormDataContent
            {
                { new StringContent(hashType),  "hashtype" },
                { new StringContent(hashValue), "hash" },
                { new StreamContent(file), "file", fileName }
            };

            var requestUrl = $"{_config.BaseUrl}folders/{folderId}/files";
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
            {
                Content = formData
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request, _cancelationToken);

            return response;
        }

        private bool IsFileSizeExceedLimits(Stream file)
        {
            // Get length of file in bytes
            long fileSizeInBytes = file.Length;

            // Convert the bytes to Kilobytes (1 KB = 1024 Bytes)
            long fileSizeInKB = fileSizeInBytes / 1024;

            // Convert the KB to MegaBytes (1 MB = 1024 KBytes)
            long fileSizeInMB = fileSizeInKB / 1024;

            if (fileSizeInMB > _config.MaxAlloweedContentLenght)
            {
                return true;
            }

            else return false;
        }
    }
}
