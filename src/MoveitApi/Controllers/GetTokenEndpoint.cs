using MoveitApiClient;
using Movit.API.Helper;

namespace Movit.API.Controllers
{
    public class GetTokenEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/authenticate/token", GetTokenAsync);
        }

        public async Task<TokenResponse> GetTokenAsync(string username, string password, MoveitClient movitClient, CancellationToken cancellationToken)
        {
            var token = await movitClient.AuthenticateAsync(username, password);

            var accessToken = token?.AccessToken ?? throw new Exception("Invalid token response.");

            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            //_logger.LogInformation("Successfully authenticated with MOVEit API.");

            return new TokenResponse(token.AccessToken, token.ExpiresIn, token.RefreshToken);
            //TODO Token expiration
        }

        public record TokenResponse(string Token, int ExpiresIn, string RefreshToken);
    }
}