namespace MoveitApiClient.Models
{
    public class UploadFileResponse
    {
        public string FileId { get; set; }
        public long Bytes { get; set; }
        public long MaxChunkSize { get; set; }
    }
}
