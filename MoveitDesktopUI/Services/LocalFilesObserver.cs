using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Image = System.Windows.Controls.Image;

namespace MoveitDesktopUI.Services
{
    public class LocalFilesObserver
    {
        private readonly TreeView _fileTree;

        public LocalFilesObserver(TreeView fileTree)
        {
            _fileTree = fileTree ?? throw new ArgumentNullException(nameof(fileTree));
        }

        /// <summary>
        /// Loads the specified folder into the TreeView.
        /// </summary>
        public void LoadSpecificFolder(string folderPath)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _fileTree.Items.Clear();
                if (Directory.Exists(folderPath))
                {
                    TreeViewItem rootItem = CreateTreeViewItemForFolder(folderPath);
                    _fileTree.Items.Add(rootItem);
                }
                else
                {
                    MessageBox.Show($"Folder does not exist: {folderPath}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        private TreeViewItem CreateTreeViewItemForFolder(string folderPath)
        {
            var folderIcon = IconExtractor.GetIcon(folderPath, smallIcon: true);

            TreeViewItem item = new TreeViewItem
            {
                Header = CreateHeader(GetFolderDisplayText(folderPath), folderIcon),
                Tag = folderPath
            };

            // Add dummy item for lazy loading
            item.Items.Add("Loading...");
            item.Expanded += (s, e) => TreeViewItem_Expanded(s, e, folderPath);

            return item;
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e, string folderPath)
        {
            if (sender is TreeViewItem item)
            {
                if (string.IsNullOrEmpty(folderPath)) return;

                if (item.Items.Count == 1 && item.Items[0] is string loadingText && loadingText == "Loading...")
                {
                    item.Items.Clear();
                    LoadSubfoldersAndFiles(item, folderPath);
                }
            }
        }

        private void LoadSubfoldersAndFiles(TreeViewItem parentItem, string folderPath)
        {
            try
            {
                foreach (string subfolder in Directory.GetDirectories(folderPath))
                {
                    TreeViewItem subItem = CreateTreeViewItemForFolder(subfolder);
                    parentItem.Items.Add(subItem);
                }

                foreach (string file in Directory.GetFiles(folderPath))
                {
                    TreeViewItem fileItem = CreateTreeViewItemForFile(file);
                    parentItem.Items.Add(fileItem);
                }
            }
            catch (UnauthorizedAccessException)
            {
                parentItem.Items.Add(new TreeViewItem { Header = "Access Denied" });
            }
        }

        private TreeViewItem CreateTreeViewItemForFile(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            var fileIcon = IconExtractor.GetIcon(filePath, smallIcon: true);

            return new TreeViewItem
            {
                Header = CreateHeader($"{fileInfo.Name} ({FormatBytes(fileInfo.Length)})", fileIcon),
                Tag = filePath
            };
        }

        private string GetFolderDisplayText(string folderPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            long folderSize = GetFolderSize(folderPath);
            int fileCount = dirInfo.GetFiles().Length;

            return $"{dirInfo.Name} - {FormatBytes(folderSize)}, {fileCount} files";
        }

        private long GetFolderSize(string folderPath)
        {
            try
            {
                return Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories)
                    .Sum(file => new FileInfo(file).Length);
            }
            catch (UnauthorizedAccessException)
            {
                return 0; // Ignore inaccessible folders
            }
        }

        private string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double len = bytes;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private StackPanel CreateHeader(string text, ImageSource icon)
        {
            StackPanel stack = new StackPanel { Orientation = Orientation.Horizontal };

            if (icon != null)
            {
                Image img = new Image
                {
                    Source = icon,
                    Width = 16,
                    Height = 16,
                    Margin = new Thickness(0, 0, 5, 0)
                };
                stack.Children.Add(img);
            }

            TextBlock tb = new TextBlock
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center
            };
            stack.Children.Add(tb);

            return stack;
        }
    }
}
