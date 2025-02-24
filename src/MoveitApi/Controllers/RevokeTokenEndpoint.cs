using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MoveitApi.Helper;
using MoveitClient;
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
            IClient movitClient,
            [FromServices] IValidator<RevokeTokenRequest> validator,
            CancellationToken cancellationToken)
        {
            await validator.ValidateAndThrowAsync(revokeTokenRequest, cancellationToken);

            try
            {
                var response = await movitClient.RevokeTokenAsync(revokeTokenRequest.Token);

                if(response.StatusCode == 401)
                {
                    return Results.Unauthorized();
                }
                if(response.StatusCode == 404)
                {
                    return Results.NotFound();
                }
                if (response.StatusCode == 200)
                {
                    return Results.Ok(response);
                }
                else
                {
                    return Results.BadRequest(response);
                }
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
