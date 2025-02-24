using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using MoveitApi.Controllers;
using MoveitClient;
using MoveitClient.Models.ResponseModels;
using static MoveitApi.Controllers.GetTokenEndpoint;

namespace MoveitApiTest.Endpoints.GetTokenEndpointTest
{
    public class GetTokenEndpointShould
    {
        private readonly GetTokenEndpoint _getTokenEndpoint;
        private readonly Mock<IClient> _mockMoveitClient;
        private readonly Mock<IValidator<TokenRequest>> _mockValidator;

        public GetTokenEndpointShould()
        {
            _getTokenEndpoint = new GetTokenEndpoint();
            _mockMoveitClient = new Mock<IClient>();
            _mockValidator = new Mock<IValidator<TokenRequest>>();

            _mockMoveitClient.Setup(x => x.GetToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new TokenResponse() { AccessToken = "testAccessToken", RefreshToken = "testRefreshToken", ExpiresIn = 1234, TokenType = "Bearer" });
        }

        [Test]
        public async Task GetTokenAsyncSuccessfully()
        {
            var tokenRequest = new GetTokenEndpoint.TokenRequest("testUsername", "testPassword");
            var result = await _getTokenEndpoint.GetTokenAsync(tokenRequest, _mockMoveitClient.Object, _mockValidator.Object, CancellationToken.None);

            var okResult = result as Ok<TokenResponse>;
            var response = okResult.Value;

            Assert.That(response.AccessToken, Is.EqualTo("testAccessToken"));
            Assert.That(response.RefreshToken, Is.EqualTo("testRefreshToken"));
            Assert.That(response.ExpiresIn, Is.EqualTo(1234));
            Assert.That(response.TokenType, Is.EqualTo("Bearer"));
        }
    }
}
