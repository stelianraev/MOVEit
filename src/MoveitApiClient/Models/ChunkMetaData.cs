namespace MoveitApiClient.Models
{
    public class ChunkMetaData
    {
        public string UploadUid { get; set; }
        public string FileSystemName { get; set; }
        public int ChunkIndex { get; set; }
        public int TotalChunks { get; set; }
        public long TotalFileSize { get; set; }
        public string ContentType { get; set; }
    }

}
