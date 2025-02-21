using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MoveitApiClient;
using Movit.API.Helper;

namespace MoveitApi.Controllers
{
    [IgnoreAntiforgeryToken]
    public class UploadFilesEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/files/upload", UploadFilesAsync)
                .Accepts<UploadFilesRequest>("multipart/form-data");
        }

        public async Task<IResult> UploadFilesAsync([FromHeader] string accessToken,
            [FromForm] UploadFilesRequest uploadFilesRequest,
            MoveitClient movitClient,
            [FromServices] IValidator<UploadFilesRequest> validator,
            CancellationToken cancellationToken)
        {
            await validator.ValidateAndThrowAsync(uploadFilesRequest, cancellationToken);

            try
            {
                var fileId = Guid.NewGuid().ToString();
                using var fileStream = uploadFilesRequest.File.OpenReadStream();
                var fileName = uploadFilesRequest.File.FileName;
                var response = await movitClient.UploadFileAsync(fileId, fileStream, fileName, accessToken);

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.StatusCode(500);
            }
        }
    }

    public record UploadFilesRequest(IFormFile File);

    public class UploadFilesRequestValidator : AbstractValidator<UploadFilesRequest>
    {
        public UploadFilesRequestValidator()
        {
            //TODO
            //RuleFor(x => x.file)
            //    .NotEmpty()
            //    .NotNull()
            //    .MaximumLength(255)
            //    .WithMessage("FilePath is required");
        }
    }
}
