namespace MoveitApiClient.Models
{
    public class MoveitConfiguration
    {
        public string? BaseUrl {  get; set; }

        public int MaxAllowedContentLength { get; set; }

        public int ChunkSize { get; set; }
    }
}
