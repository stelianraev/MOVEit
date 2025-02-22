//using FluentValidation;
//using Microsoft.AspNetCore.Mvc;
//using MoveitApiClient;
//using Movit.API.Helper;

//namespace MoveitApi.Controllers
//{
//    public class UploadFilesEndpoint : IEndpoint
//    {
//        public void Map(IEndpointRouteBuilder app)
//        {
//            app.MapPost("/files/upload", UploadFilesAsync)
//                .Accepts<UploadFilesRequest>("multipart/form-data")
//                .WithMetadata(new IgnoreAntiforgeryTokenAttribute())
//                .DisableAntiforgery();
//        }

//        public async Task<IResult> UploadFilesAsync([FromHeader] string accessToken,
//            [FromRoute] string id,
//            [FromForm] UploadFilesRequest uploadFilesRequest,
//            MoveitClient movitClient,
//            [FromServices] IValidator<UploadFilesRequest> validator,
//            CancellationToken cancellationToken)
//        {
//            await validator.ValidateAndThrowAsync(uploadFilesRequest, cancellationToken);

//            try
//            {
//                using var fileStream = uploadFilesRequest.File.OpenReadStream();
//                var fileName = uploadFilesRequest.File.FileName;

//                var response = await movitClient.UploadFileAsync(
//                folderId: id,
//                file: fileStream,
//                fileName: uploadFilesRequest.File.FileName,
//                hashType: uploadFilesRequest.HashType,
//                hashValue: uploadFilesRequest.Hash,
//                comments: uploadFilesRequest.Comments,
//                accessToken: accessToken
//            );

//                var response = await movitClient.UploadFileAsync(fileId, fileStream, fileName, accessToken);

//                return Results.Ok(response);
//            }
//            catch (Exception ex)
//            {
//                return Results.StatusCode(500);
//            }
//        }
//    }

//    public record UploadFilesRequest(IFormFile File);

//    public class UploadFilesRequestValidator : AbstractValidator<UploadFilesRequest>
//    {
//        public UploadFilesRequestValidator()
//        {
//            //TODO
//            //RuleFor(x => x.file)
//            //    .NotEmpty()
//            //    .NotNull()
//            //    .MaximumLength(255)
//            //    .WithMessage("FilePath is required");
//        }
//    }
//}
