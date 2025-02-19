using DesktopUI.ViewModels;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace MoveitWpf.ViewModels
{
    public class ListFileViewModel : ViewModelBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _token;
        private List<Item> files = new List<Item>();

        public ListFileViewModel()
        {
            _httpClient = new HttpClient();
            var _token = TokenStorage.GetToken();
        }

        public ICommand LoginCommand { get; }

        public async Task GetUploadedFiles()
        {
            int currentPage = 0;

            using (var client = new HttpClient())
            {
                while (true)
                {
                    var requestData = new GetAllFilesRequest(currentPage, 10000, null, null, false, null);
                    string accessToken = _token;

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var queryParams = new List<string>
                    {
                        $"page={requestData.Page}",
                        $"perPage={requestData.PerPage}",
                        requestData.SortField != null ? $"sortField={requestData.SortField}" : "",
                        requestData.SortDirection != null ? $"sortDirection={requestData.SortDirection}" : "",
                        requestData.NewOnly.HasValue ? $"newOnly={requestData.NewOnly.Value.ToString().ToLower()}" : "",
                        requestData.SinceDate != null ? $"sinceDate={requestData.SinceDate}" : ""
                    };

                    string requestUrl = $"https://localhost:7040/files/getall?{string.Join("&", queryParams.Where(q => !string.IsNullOrEmpty(q)))}";

                    HttpResponseMessage response = await client.GetAsync(requestUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var filesResponse = JsonConvert.DeserializeObject<FileResponse>(responseBody);

                        if (filesResponse != null && filesResponse.Items.Count > 0)
                        {
                            foreach (var item in filesResponse.Items)
                            {
                                files.Add(item);
                            }

                            if (currentPage >= filesResponse!.Paging.TotalPages)
                            {
                                break;
                            }
                            else
                            {
                                currentPage++;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to fetch files.");
                        break;
                    }
                }
            }
        }

        public record GetAllFilesRequest(int Page, int PerPage, string SortField, string SortDirection, bool? NewOnly, string SinceDate);

        public record FileResponse(string CurrentTimestamp, List<Item> Items, Paging Paging, List<Sorting> Sorting);
        public record Item(int FolderID, string UploadUsername, string Path, string UploadStamp, bool IsNew, string Name, long Size, string Id);
        public record Paging(int Page, int PerPage, int TotalItems, int TotalPages);
        public record Sorting(string SortField, string SortDirection);

    }
}
