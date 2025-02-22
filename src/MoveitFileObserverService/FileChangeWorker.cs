using FileObserverService.Enum;
using FileObserverService.Models.Configuration;
using Microsoft.Extensions.Options;

namespace FileObserverService
{
    public class Worker(ILogger<Worker> logger, IOptionsMonitor<ServiceConfig> config) : BackgroundService
    {
        private string _folderPath = config.CurrentValue.FolderMonitoringPath;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(5000, cancellationToken);

                var isDirectoryExisted = CreateObservableFolder();
                if (isDirectoryExisted)
                {
                    var watcher = new FileSystemWatcher
                    {
                        Path = _folderPath,
                        NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                        IncludeSubdirectories = true,
                        EnableRaisingEvents = true
                    };

                    watcher.Created += OnFileChanged;
                    watcher.Deleted += OnFileChanged;
                    watcher.Renamed += OnFileRenamed;
                    watcher.Changed += OnFileChanged;
                }
            }
        }

        private async void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            //TODO
            var changeType = e.ChangeType switch
            {
                WatcherChangeTypes.Created => FileChangeType.Created,
                WatcherChangeTypes.Deleted => FileChangeType.Deleted,
                WatcherChangeTypes.Changed => FileChangeType.Modified,
                _ => FileChangeType.Unknown
            };

            if (changeType != FileChangeType.Unknown)
            {
                await HandleChange(e.FullPath, changeType);
            }
        }
        private async void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            await HandleChange(e.FullPath, FileChangeType.Renamed);
            logger.LogInformation($"File renamed from {e.OldFullPath} to {e.FullPath}");
        }

        private async Task HandleChange(string path, FileChangeType changeType)
        {
            try
            {
                // Wait for file to be ready (if creation/update)
                if (changeType is FileChangeType.Created or FileChangeType.Modified)
                {
                    await WaitForFileReady(path);
                }

                //TODO
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling file change for {Path}", path);
            }
        }
        private async Task WaitForFileReady(string path)
        {

            //TODO add POLLy
            const int maxAttempts = 5;
            const int delayMs = 500;

            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                try
                {
                    using var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
                    return;
                }
                catch (IOException)
                {
                    if (attempt == maxAttempts - 1) throw;
                    await Task.Delay(delayMs);
                }
            }
        }

        private bool CreateObservableFolder()
        {
            try
            {
                var entryFolder = _folderPath;
                if (!Directory.Exists(entryFolder))
                {
                    Directory.CreateDirectory(entryFolder);
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating entry folder");
                return false;
            }
        }
    }
}
