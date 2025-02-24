using MoveitDesktopUI.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MoveitDesktopUI.Services
{
    public class RemoteFileObserver(TreeView remoteTreeView)
    {
        private readonly TreeView _remoteTreeView = remoteTreeView;

        public async Task RemoteResponseObserverAsync(GetFilesResponse remoteFiles)
        {
            if (remoteFiles?.Items != null && remoteFiles.Items.Any())
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _remoteTreeView.Items.Clear();
                    BuildTree(remoteFiles.Items);
                });
            }
            else
            {
                MessageBox.Show("No remote files found.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BuildTree(List<Item> remoteFiles)
        {
            TreeViewItem rootItem = new TreeViewItem
            {
                Header = "Remote Files",
                Tag = "/",
                IsExpanded = true
            };

            foreach (var file in remoteFiles.OrderBy(x => x.Path))
            {
                AddFileToTreeView(rootItem, file);
            }

            _remoteTreeView.Items.Add(rootItem);
        }

        private void AddFileToTreeView(TreeViewItem rootItem, Item file)
        {
            var trimmedPath = file.Path.TrimStart('/');
            var segments = trimmedPath.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
            TreeViewItem current = rootItem;

            for (int i = 0; i < segments.Count; i++)
            {
                bool isLastSegment = (i == segments.Count - 1);
                var segment = segments[i];

                if (isLastSegment)
                {
                    TreeViewItem fileItem = CreateTreeViewItemForRemoteFile(file);
                    current.Items.Add(fileItem);
                }
                else
                {
                    TreeViewItem folderItem = FindChildByHeader(current, segment);
                    if (folderItem == null)
                    {
                        folderItem = CreateTreeViewItemForRemoteFolder(segment);
                        current.Items.Add(folderItem);
                    }
                    current = folderItem;
                }
            }
        }

        private TreeViewItem CreateTreeViewItemForRemoteFolder(string folderName)
        {
            var folderIcon = IconExtractor.GetIcon("C:\\Windows", smallIcon: true);
            return new TreeViewItem
            {
                Header = CreateHeader(folderName, folderIcon),
                Tag = folderName,
                IsExpanded = true
            };
        }

        private TreeViewItem CreateTreeViewItemForRemoteFile(Item file)
        {
            var fileIcon = IconExtractor.GetIcon(file.Path, smallIcon: true);
            string displayText = $"{file.Name} ({FormatBytes(file.Size)})";
            return new TreeViewItem
            {
                Header = CreateHeader(displayText, fileIcon),
                Tag = file.Path
            };
        }

        private TreeViewItem FindChildByHeader(TreeViewItem parent, string headerText)
        {
            foreach (TreeViewItem child in parent.Items)
            {
                if (child.Header is StackPanel panel)
                {
                    var textBlock = panel.Children.OfType<TextBlock>().FirstOrDefault();
                    if (textBlock != null && textBlock.Text == headerText)
                        return child;
                }
                else if (child.Header is string simpleHeader && simpleHeader == headerText)
                {
                    return child;
                }
            }
            return null;
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
