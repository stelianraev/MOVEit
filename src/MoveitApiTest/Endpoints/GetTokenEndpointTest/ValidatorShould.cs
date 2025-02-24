using FluentValidation.TestHelper;
using static MoveitApi.Controllers.GetTokenEndpoint;

namespace MoveitApiTest.Endpoints.GetTokenEndpointTest
{
    public class ValidatorShould
    {
        [Test]
        public void FailValidationWhenUsernameIsEmpty()
        {
            var tokenRequest = new TokenRequest("", "password");

            var validator = new GetTokenRequestValidator();

            var result = validator.TestValidate(tokenRequest);

            result.ShouldHaveValidationErrorFor(x => x.Username);
        }

        [Test]
        public void FailValidationWhenUsernameIsNull()
        {
            var tokenRequest = new TokenRequest(null, "password");

            var validator = new GetTokenRequestValidator();

            var result = validator.TestValidate(tokenRequest);

            result.ShouldHaveValidationErrorFor(x => x.Username);
        }

        [Test]
        public void FailValidationWhenPasswordIsEmpty()
        {
            var tokenRequest = new TokenRequest("username", "");

            var validator = new GetTokenRequestValidator();

            var result = validator.TestValidate(tokenRequest);

            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Test]
        public void FailValidationWhenPasswordIsNull()
        {
            var tokenRequest = new TokenRequest("username", null);

            var validator = new GetTokenRequestValidator();

            var result = validator.TestValidate(tokenRequest);

            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Test]
        public void SuccessValidationWhenPasswordAndUsernameAreCorrect()
        {
            var tokenRequest = new TokenRequest("username", "passowrd");

            var validator = new GetTokenRequestValidator();

            var result = validator.TestValidate(tokenRequest);

            result.ShouldNotHaveValidationErrorFor(x => x.Username);
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
        }
    }
}
