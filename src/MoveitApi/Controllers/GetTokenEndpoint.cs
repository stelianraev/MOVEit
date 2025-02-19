using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MoveitApiClient;
using Movit.API.Helper;
using static Movit.API.Controllers.GetTokenEndpoint;

namespace Movit.API.Controllers
{
    public class GetTokenEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/authenticate/token", GetTokenAsync);
        }

        public async Task<IResult> GetTokenAsync([FromBody] TokenRequest tokenRequest,
            MoveitClient movitClient,
            [FromServices] IValidator<TokenRequest> validator,
            CancellationToken cancellationToken)
        {
            await validator.ValidateAndThrowAsync(tokenRequest, cancellationToken);
            var response = await movitClient.GetToken(tokenRequest.Username, tokenRequest.Password);

            if (response.IsSuccessStatusCode)
            {
                return Results.Ok(response);
            }
            else
            {
                return Results.BadRequest("Authentication Failed");
            }
        }
        public record TokenRequest(string Username, string Password);
    }
}

public class GetTokenRequestValidator : AbstractValidator<TokenRequest>
{
    public GetTokenRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .NotNull()
            .WithMessage("Username is required");

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull()
            .WithMessage("Password is required");
    }
}
