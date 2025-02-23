namespace MoveitDesktopUI.Models
{
    public class GetFilesResponse
    {
        public string CurrenttimeStamp { get; set; }
        public List<Item> Items { get; set; }
        public Paging Paging { get; set; }

        public List<Sorting> Sortings { get; set; }
    }

    public class Sorting
    {
        public string sortField { get; set; }
        public string sortDirection { get; set; }
    }

    public class Paging
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }

    public class Item
    {
        public int FolderId { get; set; }

        public string UploadUsername { get; set; }
        public string Path { get; set; }
        public string TimeStamp { get; set; }

        public bool IsNew { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public string Id { get; set; }
    }
}

