using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MoveitApi.SignalR;
using MoveitApiClient;
using Movit.API.Helper;

namespace MoveitApi.Controllers
{
    public class DeleteFileEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapDelete("/files/delete", DeleteFile);
        }
        public async Task<IResult> DeleteFile([FromHeader(Name = "X-Auth-Token")] string accessToken,
                                              [FromQuery] string fileId,
                                              CancellationToken cancellationToken,
                                              MoveitClient movitClient,
                                              [FromServices] IHubContext<FileObserverHub> hubContext)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return Results.Unauthorized();
            }
            try
            {
                var response = await movitClient.DeleteFileAsync(fileId, accessToken);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    await hubContext.Clients.All.SendAsync("FileChanged", fileId);
                    return Results.Ok(responseBody);
                }
                else
                {
                    return Results.BadRequest(responseBody);
                }
            }
            catch (Exception ex)
            {
                return Results.StatusCode(500);
            }
        }
    }
}
