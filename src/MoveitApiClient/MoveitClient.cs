using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoveitApiClient.Models;
using Newtonsoft.Json;
using System.IO;
using System.IO.Enumeration;
using System.IO.Pipes;
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

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);
                return tokenResponse;
            }
            else
            {
                _logger.LogError($"Error getting token: {responseBody}");
                throw new Exception("Error getting token");
            }
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

            if (response.IsSuccessStatusCode)
            {
                var revokeTokenResponse = JsonConvert.DeserializeObject<RevokeTokenResponse>(responseBody);
                return revokeTokenResponse;
            }
            else
            {
                _logger.LogError($"Error getting token: {responseBody}");
                throw new Exception("Error getting token");
            }
        }

        public async Task<HttpResponseMessage> GetAllFilesAsync(int page,
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

        public async Task<UploadFileResponse> UploadFileAsync(string folderId,
                                                              Stream file,
                                                              string fileName,
                                                              string accessToken,
                                                              string hashType = "sha-256",
                                                              string comments = "")
        {
            file.Seek(0, SeekOrigin.Begin);
            var fileSize = file.Length;
            var hashValue = string.Empty;

            using (var sha256 = SHA256.Create())
            {
                hashValue = Convert.ToHexStringLower(sha256.ComputeHash(file));
            }

            file.Seek(0, SeekOrigin.Begin);

            var isFileSizeExceedLimits = IsFileSizeExceedLimits(file);

            if (isFileSizeExceedLimits)
            {
                return await UploadFileInChunksAsync(folderId, file, hashType, hashValue, comments, accessToken);
            }
            else
            {
                return await UploadFileSingleAsync(folderId, file, fileName, hashType, hashValue, comments, accessToken);
            }
        }

        private async Task<UploadFileResponse> UploadFileSingleAsync(string folderId,
                                                                     Stream file,
                                                                     string fileName,
                                                                     string hashValue,
                                                                     string hashType,
                                                                     string comments,
                                                                     string accessToken)
        {
            // Build the form data
            var formData = new MultipartFormDataContent
            {
                { new StringContent(hashType),  "hashtype" },
                { new StringContent(hashValue), "hash" },
                { new StringContent(comments),  "comments" },
                { new StreamContent(file), "file", fileName }
            };

            var requestUrl = $"{_config.BaseUrl}folders/{folderId}/files";
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
            {
                Content = formData
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request, _cancelationToken);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<UploadFileResponse>(responseBody);
                return result;
            }
            else
            {
                _logger.LogError($"Error uploading file '{folderId}': {responseBody}");
                throw new Exception($"Error uploading file: {response.ReasonPhrase}");
            }
        }

        private async Task<UploadFileResponse> UploadFileInChunksAsync(string folderId,
                                                                       Stream file,
                                                                       string hashType,
                                                                       string hashValue,
                                                                       string comments,
                                                                       string accessToken)
        {
            long totalSize = file.Length;

            int chunkSizeBytes = _config.ChunkSize;
            int totalChunks = (int)(totalSize / chunkSizeBytes);
            if (totalSize % chunkSizeBytes != 0)
            {
                totalChunks++;
            }

            UploadFileResponse finalResponse = null;

            for (int i = 0; i < totalChunks; i++)
            {
                long position = (i * (long)_config.ChunkSize);
                int toRead = (int)Math.Min(file.Length - position, _config.ChunkSize);
                byte[] buffer = new byte[toRead];
                await file.ReadExactlyAsync(buffer);

                using (MultipartFormDataContent form = new MultipartFormDataContent())
                {
                    form.Add(new ByteArrayContent(buffer), "files");
                    form.Add(new StringContent(i.ToString()), folderId);

                    var meta = JsonConvert.SerializeObject(new ChunkMetaData
                    {
                        UploadUid = folderId.ToString(),
                        ChunkIndex = i,
                        TotalChunks = totalChunks,
                        TotalFileSize = file.Length,
                        ContentType = "application/unknown"
                    });

                    form.Add(new StringContent(meta), "metaData");

                    var requestUrl = $"{_config.BaseUrl}folders/{folderId}/files";

                    var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
                    {
                        Content = form
                    };

                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    var response = await _httpClient.SendAsync(request, _cancelationToken);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError($"Chunk upload error (chunk={i} of {totalChunks}): {responseBody}");
                        throw new Exception($"Error uploading file: {response.ReasonPhrase}");
                    }

                    finalResponse = JsonConvert.DeserializeObject<UploadFileResponse>(responseBody);
                }
            }

            return finalResponse;
        }

        private bool IsFileSizeExceedLimits(Stream file)
        {
            // Get length of file in bytes
            long fileSizeInBytes = file.Length;

            // Convert the bytes to Kilobytes (1 KB = 1024 Bytes)
            long fileSizeInKB = fileSizeInBytes / 1024;

            // Convert the KB to MegaBytes (1 MB = 1024 KBytes)
            long fileSizeInMB = fileSizeInKB / 1024;

            if (fileSizeInMB > _config.MaxAllowedContentLength)
            {
                return true;
            }

            else return false;
        }


        //TODO
        //public async Task UploadFolderAsync(string folderId,
        //                                    string localFolderPath,
        //                                    string accessToken,
        //                                    string hashType = "sha-256",
        //                                    string comments = "",
        //                                    bool includeSubfolders = false)
        //{

        //    var files = Directory.GetFiles(localFolderPath);
        //    foreach (var file in files)
        //    {
        //        await UploadFileAsync(folderId, file, accessToken, hashType, comments);
        //    }

        //    if (includeSubfolders)
        //    {
        //        var subfolders = Directory.GetDirectories(localFolderPath);
        //        foreach (var subfolder in subfolders)
        //        {
        //            // If you need to create subfolders in Moveit, you'd do that first
        //            // (not shown here). If you simply want to dump everything into
        //            // the same folder, you can skip subfolder creation.

        //            // For demonstration, we'll just re-use the same folderId
        //            await UploadFolderAsync(folderId, subfolder, accessToken, hashType, comments, includeSubfolders: true);
        //        }
        //    }
        //}
    }
}
