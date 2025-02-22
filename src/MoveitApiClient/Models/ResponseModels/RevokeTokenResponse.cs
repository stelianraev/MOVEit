namespace MoveitApiClient.Models.Responses
{
    public class RevokeTokenResponse
    {
        public string Token { get; set; }

        public string Message { get; set; }

        public int StatusCode { get; set; }
    }
}
