using MoveitClient.Models.ResponseModels;

namespace MoveitClient
{
    public interface IClient
    {
        Task<TokenResponse> GetToken(string username, string password);

        Task<RevokeTokenResponse> RevokeTokenAsync(string token);
        Task<GetFilesResponse> GetAllFilesAsync(int page, int perPage, string? sortField, string? sortDirection, bool? newOnly, string? sinceDate, string accessToken);

        Task<HttpResponseMessage> DeleteFileAsync(string fileId, string accessToken);

        Task<HttpResponseMessage> UploadFileAsync(string folderId, Stream fileStrem, string fileName, string accessToken);

        Task<HttpResponseMessage> UploadFileSingleAsync(string folderId, Stream file, string fileName, string hashType, string hashValue, string accessToken);
        bool IsFileSizeExceedLimits(Stream file);
    }
}
