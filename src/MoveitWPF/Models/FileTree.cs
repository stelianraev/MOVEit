using System.Collections.ObjectModel;

namespace MoveitWpf.Models
{
    public class FileTree
    {
        public FileTree(string name, string path, bool isFolder, string size = "")
        {
            Name = name;
            Path = path;
            Size = size;
            IsFolder = isFolder;
        }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Size { get; set; }
        public bool IsFolder { get; set; }
        public ObservableCollection<FileTree> Children { get; set; } = new ObservableCollection<FileTree>();
    }
}
