using FCG.Shared.Contracts.Results;
using FCG_Libraries.Application.Libraries.Requests;
using FCG_Libraries.Application.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FCG_Libraries.Api.Controllers
{
    [ApiController]
    [Route("libraries")]
    public class LibraryController(ILibraryService service) : ControllerBase
    {
        /// <summary>
        /// Solicita a adição de um novo item na biblioteca.
        /// </summary>
        /// <param name="request">Informações necessárias para cadastrar um item na biblioteca</param>
        /// <param name="cancellationToken">Token que monitora o cancelamento do processo.</param>
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IResult> CreateLibraryAsync([FromBody] LibraryRequest request, CancellationToken cancellationToken = default)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString();

            var result = await service.CreateLibraryItemAsync(request, correlationId!, cancellationToken);
            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "404" => TypedResults.NotFound(new Error("404", result.Error.Message)),
                    "402" => TypedResults.StatusCode(StatusCodes.Status402PaymentRequired),
                    "409" => TypedResults.Conflict(new Error("409", result.Error.Message)),
                    _ => TypedResults.BadRequest(new Error("400", result.Error.Message))
                };
            }

            return TypedResults.Accepted($"/libraries/{result.Value.ItemId}", new { Item = result.Value, CorrelationId = correlationId });

        }

        /// <summary>
        /// Busca todos os itens de todas as bibliotecas
        /// </summary>
        /// <param name="cancellationToken">Token que monitora o cancelamento do processo.</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("all")]
        public async Task<IResult> GetAllLibrariesAsync(CancellationToken cancellationToken = default)
            => TypedResults.Ok((await service.GetAllLibrariesAsync(cancellationToken)).Value);

        /// <summary>
        /// Busca os itens de um pagamento.
        /// </summary>
        /// <param name="paymentId">Id do pagamento a ser pesquisado.</param>
        /// <param name="cancellationToken">Token que monitora o cancelamento do processo.</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("payments/{paymentId}")]
        public async Task<IResult> GetLibrariesByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            var result = await service.GetLibrariesByPaymentAsync(paymentId, cancellationToken);
            
            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "404" => TypedResults.NotFound(new Error("404", result.Error.Message)),
                    _ => TypedResults.BadRequest(new Error("400", result.Error.Message))
                };
            }

            return TypedResults.Ok(result.Value);
        }

        /// <summary>
        /// Busca os itens em todas as bibliotecas que contenham um jogo específico.
        /// </summary>
        /// <param name="gameId">Id do jogo a ser pesquisado.</param>
        /// <param name="cancellationToken">Token que monitora o cancelamento do processo.</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("game/{gameId}")]
        public async Task<IResult> GetLibrariesByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            var result = await service.GetLibrariesByGameIdAsync(gameId, cancellationToken);

            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "404" => TypedResults.NotFound(new Error("404", result.Error.Message)),
                    _ => TypedResults.BadRequest(new Error("400", result.Error.Message))
                };
            }

            return TypedResults.Ok(result.Value);
        }

        /// <summary>
        /// Busca os itens adquiridos da biblioteca de um usuário específico.
        /// </summary>
        /// <param name="userId">Id do usuário</param>
        /// <param name="cancellationToken">Token que monitora o cancelamento do processo.</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("acquireds/{userId}")]
        public async Task<IResult> GetAcquiredItems(Guid userId, CancellationToken cancellationToken = default)
        {
            var result = await service.GetAcquiredItemsByUser(userId, cancellationToken);
            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "404" => TypedResults.NotFound(new Error("404", result.Error.Message)),
                    _ => TypedResults.BadRequest(new Error("400", result.Error.Message))
                };
            }

            return TypedResults.Ok(result.Value);
        }

        /// <summary>
        /// Busca os itens já requisitados por um usuário específico.
        /// </summary>
        /// <param name="userId">Id do usuário</param>
        /// <param name="cancellationToken">Token que monitora o cancelamento do processo.</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("requesteds/{userId}")]
        public async Task<IResult> GetLibrariesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var result = await service.GetRequestedItemsByUser(userId, cancellationToken);
            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "404" => TypedResults.NotFound(new Error("404", result.Error.Message)),
                    _ => TypedResults.BadRequest(new Error("400", result.Error.Message))
                };
            }

            return TypedResults.Ok(result.Value);
        }

        /// <summary>
        /// Busca um item de biblioteca específico pelo seu Id.
        /// </summary>
        /// <param name="id">Id do item da biblioteca</param>
        /// <param name="cancellationToken">Token que monitora o cancelamento do processo.</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}")]
        public async Task<IResult> GetLibraryByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {            
            var result = await service.GetLibraryByIdAsync(id, cancellationToken);

            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "404" => TypedResults.NotFound(new Error("404", result.Error.Message)),                   
                    _ => TypedResults.BadRequest(new Error("400", result.Error.Message))
                };
            }

            return TypedResults.Ok(result.Value);
        }

        
       

        /// <summary>
        /// Solicita a exclusão de um item da biblioteca.
        /// </summary>        
        /// <param name="id">Id do item que será removido</param>
        /// <param name="cancellationToken">Token que monitora o cancelamento do processo.</param>
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id}")]
        public async Task<IResult> DeleteLibraryAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var correlationId = HttpContext.Items["CorrelationId"]!.ToString();

            var result = await service.DeleteLibraryItemAsync(id, correlationId!, cancellationToken);

            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "404" => TypedResults.NotFound(new Error("404", result.Error.Message)),                    
                    _ => TypedResults.BadRequest(new Error("400", result.Error.Message))
                };
            }

            return TypedResults.Accepted($"/libraries/status/{correlationId}", new { ItemId = id, CorrelationId = correlationId });

        }
    }
}
