﻿using Microsoft.AspNetCore.Mvc;
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

        public async Task<IResult> GetAllFiles([FromHeader(Name = "X-Auth-Token")] string accessToken,
                                               CancellationToken cancellationToken,
                                               MoveitClient movitClient,
                                               [FromQuery] int page = 1,
                                               [FromQuery] int perPage = 1000,
                                               [FromQuery] string? sortField = "path",
                                               [FromQuery] string? sortDirection = "asc",
                                               [FromQuery] bool newOnly = false,
                                               [FromQuery] string? sinceDate = null)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return Results.Unauthorized();
            }               

            try
            {
                var response = await movitClient.GetAllFilesAsync(page,
                                                                  perPage,
                                                                  sortField,
                                                                  sortDirection,
                                                                  newOnly,
                                                                  sinceDate,
                                                                  accessToken);


                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.StatusCode(500);
            }
        }
    }   

    public record GetAllFilesRequest(int Page, int PerPage, string SortField, string SortDirection, bool? NewOnly, string SinceDate);

    //public class GetAllFilesRequestValidator : AbstractValidator<GetAllFilesRequest>
    //{
    //    public GetAllFilesRequestValidator()
    //    {
    //        RuleFor(x => x.Page)
    //         .Must(x => x >= 0)
    //         .WithMessage("Page must be positive number");

    //        RuleFor(x => x.PerPage)
    //         .Must(x => x >= 0)
    //         .WithMessage("PerPage must be positive number");
    //    }
    //}
}
