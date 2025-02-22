using MoveitDesktopUI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
                _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token);
                var url = $"https://localhost:7040/files/getall?page={page}&perPage={perPage}";

                var response = await _httpClient.GetAsync(url, _cancellationToken).ConfigureAwait(false);
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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
