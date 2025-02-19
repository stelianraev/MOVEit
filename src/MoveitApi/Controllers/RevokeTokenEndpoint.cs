using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MoveitApiClient;
using Movit.API.Helper;

namespace MoveitApi.Controllers
{
    public class RevokeTokenEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/authenticate/revoke", RevokeTokenAsync);
        }

        public async Task<IResult> RevokeTokenAsync([FromBody] string token,
            MoveitClient movitClient,
            [FromServices] IValidator<string> validator,
            CancellationToken cancellationToken)
        {
            await validator.ValidateAndThrowAsync(token, cancellationToken);

            var response = await movitClient.RevokeTokenAsync(token);

            if (response.IsSuccessStatusCode)
            {
                return Results.Ok(response);
            }
            else
            {
                return Results.BadRequest("Revoke Token Failed");
            }
        }
        public record RevokeTokenRequest(string Token);
    }
    public class GetTokenRequestValidator : AbstractValidator<string>
    {
        public GetTokenRequestValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .NotNull()
                .WithMessage("Valid Token is required");
        }
    }
}
