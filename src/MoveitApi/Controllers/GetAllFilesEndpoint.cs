using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MoveitApiClient;
using Movit.API.Helper;

namespace MoveitApi.Controllers
{
    public class GetAllFilesEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/files/getall", GetAllFiles);
        }

        public async Task<IResult> GetAllFiles(
                                    [FromQuery] GetAllFilesRequest getAllFilesRequest,
                                    [FromHeader(Name = "Authorization")] string accessToken,
                                    MoveitClient movitClient,
                                    [FromServices] IValidator<GetAllFilesRequest> validator,
                                    CancellationToken cancellationToken)
        {
            await validator.ValidateAndThrowAsync(getAllFilesRequest, cancellationToken);

            var response = await movitClient.GetAllFiles(getAllFilesRequest.Page,
                                                         getAllFilesRequest.PerPage,
                                                         getAllFilesRequest.SortField,
                                                         getAllFilesRequest.SortDirection,
                                                         getAllFilesRequest.NewOnly,
                                                         getAllFilesRequest.SinceDate,
                                                         accessToken);

            if (!response.IsSuccessStatusCode)
            {
                return Results.BadRequest("Authentication Failed");
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            return Results.Ok(responseBody);
        }

    }

    public record GetAllFilesRequest(int Page, int PerPage, string SortField, string SortDirection, bool NewOnly, string SinceDate);

    public class GetAllFilesRequestValidator : AbstractValidator<GetAllFilesRequest>
    {
        public GetAllFilesRequestValidator()
        {
            RuleFor(x => x.Page)
             .Must(x => x >= 0)
             .WithMessage("Page must be positive number");

            RuleFor(x => x.PerPage)
             .Must(x => x >= 0)
             .WithMessage("PerPage must be positive number");
        }
    }
}
