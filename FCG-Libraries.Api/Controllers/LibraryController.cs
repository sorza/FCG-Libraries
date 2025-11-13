using FCG_Libraries.Application.Libraries.Requests;
using FCG_Libraries.Application.Libraries.Services;
using FCG_Libraries.Application.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace FCG_Libraries.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LibraryController(ILibraryService service) : ControllerBase
    {
        [HttpGet("all")]
        public async Task<IResult> GetAllLibrariesAsync(CancellationToken cancellationToken = default)
            => TypedResults.Ok((await service.GetAllLibrariesAsync(cancellationToken)).Value);


        [HttpGet("game/{gameId}")]
        public async Task<IResult> GetLibrariesByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await service.GetLibrariesByGameIdAsync(gameId, cancellationToken);

                IResult response = result.IsFailure
                    ? TypedResults.NotFound(new Error("404", result.Error.Message))
                    : TypedResults.Ok(result.Value);

                return response;
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(new Error("400", ex.Message));
            }

        }

        [HttpGet("user/{userId}")]
        public async Task<IResult> GetLibrariesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await service.GetLibrariesByUserIdAsync(userId, cancellationToken);
                IResult response = result.IsFailure
                    ? TypedResults.NotFound(new Error("404", result.Error.Message))
                    : TypedResults.Ok(result.Value);
                return response;
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(new Error("400", ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IResult> GetLibraryByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await service.GetLibraryByIdAsync(id, cancellationToken);

                IResult response = result.IsFailure
                    ? TypedResults.NotFound(new Error("404", result.Error.Message))
                    : TypedResults.Ok(result.Value);
                return response;
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(new Error("400", ex.Message));
            }
        }

        [HttpPost]
        public async Task<IResult> CreateLibraryAsync([FromBody] LibraryRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await service.CreateLibraryAsync(request, cancellationToken);
                IResult response = result.IsFailure
                    ? TypedResults.Conflict(new Error("409", result.Error.Message))
                    : TypedResults.Ok(result.Value);
                return response;
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(new Error("400", ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IResult> UpdateLibraryAsync(Guid id, [FromBody] LibraryRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await service.UpdateLibraryAsync(id, request, cancellationToken);
                IResult response = result.IsFailure
                    ? TypedResults.NotFound(new Error("404", result.Error.Message))
                    : TypedResults.Ok(result.Value);
                return response;
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(new Error("400", ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IResult> DeleteLibraryAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await service.DeleteLibraryAsync(id, cancellationToken);
                IResult response = result.IsFailure
                    ? TypedResults.NotFound(new Error("404", result.Error.Message))
                    : TypedResults.Ok();
                return response;
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(new Error("400", ex.Message));
            }
        }
    }
}
