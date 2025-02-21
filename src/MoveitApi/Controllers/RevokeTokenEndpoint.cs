using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MoveitApiClient;
using Movit.API.Helper;
using static MoveitApi.Controllers.RevokeTokenEndpoint;

namespace MoveitApi.Controllers
{
    public class RevokeTokenEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/authenticate/revoke", RevokeTokenAsync);
        }

        public async Task<IResult> RevokeTokenAsync([FromBody] RevokeTokenRequest revokeTokenRequest,
            MoveitClient movitClient,
            [FromServices] IValidator<RevokeTokenRequest> validator,
            CancellationToken cancellationToken)
        {
            await validator.ValidateAndThrowAsync(revokeTokenRequest, cancellationToken);

            try
            {
                var response = await movitClient.RevokeTokenAsync(revokeTokenRequest.Token);
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.StatusCode(500);
            }
        }
        public record RevokeTokenRequest(string Token);
    }
    public class GetTokenRequestValidator : AbstractValidator<RevokeTokenRequest>
    {
        public GetTokenRequestValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty()
                .NotNull()
                .WithMessage("Valid Token is required");
        }
    }
}
