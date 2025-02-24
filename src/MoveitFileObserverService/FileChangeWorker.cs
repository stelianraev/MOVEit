using Microsoft.Extensions.Options;
using MoveitFileObserverService.Models;
using MoveitFileObserverService.Models.Configuration;
using MoveitWpf.MoveitFileOberverService.Services;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.Http.Headers;

namespace MoveitFileObserverService
{
    public class FileChangeWorker : BackgroundService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly ILogger<FileChangeWorker> _logger;
        private readonly ServiceConfig _config;
        private readonly FileSystemWatcher _watcher;
        private readonly ConcurrentQueue<string> _fileCreateQueue;
        private readonly ConcurrentQueue<string> _fileDeleteQueue;
        private readonly SemaphoreSlim _queueSemaphore;

        private bool _isProcessingFiles;

        public FileChangeWorker(ILogger<FileChangeWorker> logger, IOptionsMonitor<ServiceConfig> config)
        {
            _config = config.CurrentValue;
            _logger = logger;
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
            _fileCreateQueue = new ConcurrentQueue<string>();
            _fileDeleteQueue = new ConcurrentQueue<string>();
            _queueSemaphore = new SemaphoreSlim(1, 1);
            _isProcessingFiles = false;

            CreateObservableFolder();

            _watcher = new FileSystemWatcher
            {
                Path = _config.FolderMonitoringPath,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                IncludeSubdirectories = true
            };

            _watcher.Created += OnFileCreated;
            _watcher.Deleted += OnFileDeleted;

        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _watcher.EnableRaisingEvents = true;

            while (!cancellationToken.IsCancellationRequested)
            {
                //TODO Logic to stop listener if token is expired

                await ProcessFileQueue();

                await Task.Delay(2000, cancellationToken);
            }
        }

        private async Task ProcessFileQueue()
        {
            if (_isProcessingFiles)
            {
                return;
            }

            await _queueSemaphore.WaitAsync();

            try
            {
                _isProcessingFiles = true;

                while (_fileCreateQueue.Count > 0 || _fileDeleteQueue.Count > 0)
                {
                    if (_fileCreateQueue.TryDequeue(out var filePath))
                    {
                        await UploadFileAsync(filePath);
                    }

                    if (_fileDeleteQueue.TryDequeue(out var fileName))
                    {
                        await DeleteFileAsync(fileName);
                    }
                }

                _isProcessingFiles = false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Processing file error: {ex.Message}");
            }
            finally
            {
                _queueSemaphore.Release();
            }

        }


        private async Task UploadFileAsync(string filePath)
        {
            try
            {
                var token = TokenStorage.GetAccessToken().AccessToken;
                var folderId = _config.RemoteFolderId;
                var fileName = Path.GetFileName(filePath);

                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, 4096, true);
                using var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                using var content = new MultipartFormDataContent
                {
                    { new StringContent(folderId), "FolderId" },
                    { new StringContent(token), "AccessToken" },
                    { fileContent, "File", fileName }
                };

                using var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7040/files/upload")
                {
                    Content = content
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"[UploadFile] File successfully sent: {fileName}");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"[UploadFile] Failed to upload {fileName}: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[UploadFile] Error uploading file: {filePath}");
            }
        }

        private async Task DeleteFileAsync(string fileName)
        {
            try
            {
                var token = TokenStorage.GetAccessToken()?.AccessToken;
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("[DeleteFile] No valid access token.");
                    return;
                }

                var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7040/files/getall?perPage=10000");
                request.Headers.Add("X-Auth-Token", token);

                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                var remoteFiles = JsonConvert.DeserializeObject<GetFilesResponse>(content);

                var fileToRemove = remoteFiles?.Items.FirstOrDefault(f => f.Name == fileName);
                if (fileToRemove == null)
                {
                    _logger.LogWarning($"[DeleteFile] File not found in remote storage: {fileName}");
                    return;
                }

                var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"https://localhost:7040/files/delete?fileId={fileToRemove.Id}");
                deleteRequest.Headers.Add("X-Auth-Token", token);

                var deleteResponse = await _httpClient.SendAsync(deleteRequest);
                var deleteResponseContent = await deleteResponse.Content.ReadAsStringAsync();

                if (deleteResponse.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"[DeleteFile] File deleted successfully: {fileName}");
                }
                else
                {
                    _logger.LogError($"[DeleteFile] Failed to delete file {fileName}: {deleteResponseContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[DeleteFile] Error deleting file: {fileName}");
            }
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                _fileCreateQueue.Enqueue(e.FullPath);
                _logger.LogInformation($"[OnFileCreated] Queued file: {e.Name}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[OnFileCreated] Error queuing file: {e.Name}");
            }
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            try
            {
                _fileDeleteQueue.Enqueue(e.Name);
                _logger.LogInformation($"[OnFileDeleted] Queued file for deletion: {e.Name}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[OnFileDeleted] Error queuing file for deletion: {e.Name}");
            }
        }

        private bool CreateObservableFolder()
        {
            try
            {
                var entryFolder = _config.FolderMonitoringPath;
                if (!Directory.Exists(entryFolder))
                {
                    Directory.CreateDirectory(entryFolder);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating entry folder");
                return false;
            }
        }
    }
}
