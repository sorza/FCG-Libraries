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
        /// <summary>
        /// Busca todos os itens de todas as bibliotecas
        /// </summary>
        /// <param name="cancellationToken">Token que monitora o cancelamento do processo.</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("all")]
        public async Task<IResult> GetAllLibrariesAsync(CancellationToken cancellationToken = default)
            => TypedResults.Ok((await service.GetAllLibrariesAsync(cancellationToken)).Value);


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
        /// Busca os itens na biblioteca de um usuário específico.
        /// </summary>
        /// <param name="userId">Id do usuário</param>
        /// <param name="cancellationToken">Token que monitora o cancelamento do processo.</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("user/{userId}")]
        public async Task<IResult> GetLibrariesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
                var result = await service.GetLibrariesByUserIdAsync(userId, cancellationToken);
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
        /// Cria um novo item na biblioteca.
        /// </summary>
        /// <param name="request">Informações necessárias para cadastrar um item na biblioteca</param>
        /// <param name="cancellationToken">Token que monitora o cancelamento do processo.</param>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IResult> CreateLibraryAsync([FromBody] LibraryRequest request, CancellationToken cancellationToken = default)
        {
            var result = await service.CreateLibraryAsync(request, cancellationToken);
            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "404" => TypedResults.NotFound(new Error("404", result.Error.Message)),
                    "409" => TypedResults.Conflict(new Error("409", result.Error.Message)),
                    _ => TypedResults.BadRequest(new Error("400", result.Error.Message))
                };
            }             
            
            return TypedResults.Created($"/Library/{result.Value}", result.Value);
          
        }

        /// <summary>
        /// Atualiza um item na biblioteca.
        /// </summary>
        /// <param name="request">Novos dados para atualização</param>
        /// <param name="id">Id do item da biblioteca que será atualizado</param>
        /// <param name="cancellationToken">Token que monitora o cancelamento do processo.</param>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id}")]
        public async Task<IResult> UpdateLibraryAsync(Guid id, [FromBody] LibraryRequest request, CancellationToken cancellationToken = default)
        {
           
            var result = await service.UpdateLibraryAsync(id, request, cancellationToken);
            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "404" => TypedResults.NotFound(new Error("404", result.Error.Message)),
                    "409" => TypedResults.Conflict(new Error("409", result.Error.Message)),
                    _ => TypedResults.BadRequest(new Error("400", result.Error.Message))
                };
            }
            return TypedResults.Created($"/Library/{result.Value}", result.Value);
        }

        /// <summary>
        /// Remove um item da biblioteca.
        /// </summary>        
        /// <param name="id">Id do item que será removido</param>
        /// <param name="cancellationToken">Token que monitora o cancelamento do processo.</param>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id}")]
        public async Task<IResult> DeleteLibraryAsync(Guid id, CancellationToken cancellationToken = default)
        {
            
            var result = await service.DeleteLibraryAsync(id, cancellationToken);

            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "404" => TypedResults.NotFound(new Error("404", result.Error.Message)),                    
                    _ => TypedResults.BadRequest(new Error("400", result.Error.Message))
                };
            }

            return TypedResults.NoContent();

        }
    }
}
