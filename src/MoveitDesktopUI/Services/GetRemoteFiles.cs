using MoveitDesktopUI.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;

namespace MoveitDesktopUI.Services
{
    public class GetRemoteFiles
    {
        private readonly HttpClient _httpClient;
        private readonly CancellationToken _cancellationToken;

        public GetRemoteFiles(HttpClient httpClient, TreeView remoteTreeView, CancellationToken cancellationToken)
        {
            _httpClient = httpClient;
            _cancellationToken = cancellationToken;
        }
        public async Task<GetFilesResponse> GetRemoteFilesAsync(int page, int perPage, string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7040/files/getall?page={page}&perPage={perPage}");
                request.Headers.Add("X-Auth-Token", token);

                var response = await _httpClient.SendAsync(request, _cancellationToken);
                var content = await response.Content.ReadAsStringAsync();
                var remoteFiles = JsonConvert.DeserializeObject<GetFilesResponse>(content);

                return remoteFiles;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading remote files: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
