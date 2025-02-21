using MoveitWpf.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace MoveitWpf.ViewModels
{
    public class LocalFilesViewModel : ViewModelBase
    {
        private readonly string _baseFolder = @"C:\\MOOVit";

        public LocalFilesViewModel()
        {
            FileTree = new ObservableCollection<FileTree>();
            LoadSpecificFolder(_baseFolder);
        }

        public ObservableCollection<FileTree> FileTree { get; set; }

        private void LoadSpecificFolder(string folderPath)
        {
            Application.Current.Dispatcher.Invoke(() => FileTree.Clear());

            if (!Directory.Exists(folderPath))
            {
                return;
            }

            var rootFolder = new FileTree(Path.GetFileName(folderPath), folderPath, true);
            Application.Current.Dispatcher.Invoke(() => FileTree.Add(rootFolder));

            LoadChildren(rootFolder);
        }

        private void LoadChildren(FileTree parent)
        {
            if (!Directory.Exists(parent.Path))
            {
                return;
            }

            try
            {
                foreach (var dir in Directory.GetDirectories(parent.Path))
                {
                    var folderNode = new FileTree(Path.GetFileName(dir), dir, true);
                    Application.Current.Dispatcher.Invoke(() => parent.Children.Add(folderNode));
                    LoadChildren(folderNode);
                }

                foreach (var file in Directory.GetFiles(parent.Path))
                {
                    FileInfo fi = new FileInfo(file);
                    var fileNode = new FileTree(fi.Name, file, false, FormatBytes(fi.Length));
                    Application.Current.Dispatcher.Invoke(() => parent.Children.Add(fileNode));
                }
            }
            catch (Exception ex)
            {
                // TODO: Log error
            }
        }

        private string FormatBytes(long bytes)
        {
            if (bytes >= 1024 * 1024 * 1024)
                return $"{(double)bytes / (1024 * 1024 * 1024):0.##} GB";
            else if (bytes >= 1024 * 1024)
                return $"{(double)bytes / (1024 * 1024):0.##} MB";
            else if (bytes >= 1024)
                return $"{(double)bytes / 1024:0.##} KB";
            else
                return $"{bytes} bytes";
        }
    }
}

