using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MoveitApi.Helper;
using MoveitApi.SignalR;
using MoveitClient;

namespace MoveitApi.Controllers
{
    public class UploadFilesEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/files/upload", UploadFilesAsync)
                .Produces<HttpResponseMessage>(StatusCodes.Status200OK)
                .Accepts<UploadFilesRequest>("multipart/form-data")
                .WithMetadata(new IgnoreAntiforgeryTokenAttribute())
                .DisableAntiforgery();
        }

        public async Task<IResult> UploadFilesAsync([FromForm] UploadFilesRequest uploadFilesRequest,
            IClient movitClient,
            [FromServices] IValidator<UploadFilesRequest> validator,
            [FromServices] IHubContext<FileObserverHub> hubContext,
            CancellationToken cancellationToken)
        {
            await validator.ValidateAndThrowAsync(uploadFilesRequest, cancellationToken);

            try
            {
                using var stream = uploadFilesRequest.File.OpenReadStream();

                var response = await movitClient.UploadFileAsync(uploadFilesRequest.FolderId, stream, uploadFilesRequest.File.FileName, uploadFilesRequest.AccessToken);

                await hubContext.Clients.All.SendAsync("FileUploaded", uploadFilesRequest.File.FileName);

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.StatusCode(500);
            }
        }
    }

    public record UploadFilesRequest(string FolderId, IFormFile File, string AccessToken);

    public class UploadFilesRequestValidator : AbstractValidator<UploadFilesRequest>
    {
        public UploadFilesRequestValidator()
        {
            RuleFor(x => x.File)
                .NotNull()
                .WithMessage("File is required");

            RuleFor(x => x.AccessToken)
                .NotEmpty()
                .NotNull()
                .WithMessage("Valid Token is required");
        }
    }
}
