using FCG.Shared.Contracts.Enums;
using FCG.Shared.Contracts.Events.Domain.Libraries;
using FCG.Shared.Contracts.Interfaces;
using FCG.Shared.Contracts.Results;
using FCG_Libraries.Application.Libraries.Requests;
using FCG_Libraries.Application.Libraries.Responses;
using FCG_Libraries.Application.Shared.Interfaces;
using FCG_Libraries.Domain.Libraries.Entities;
using FluentValidation;
using System.Net.Http.Json;
using System.Text.Json;

namespace FCG_Libraries.Application.Libraries.Services
{
    public class LibraryService(ILibraryRepository repository,
                                IValidator<LibraryRequest> validator,
                                IEventPublisher publisher,
                                IEventStore eventStore,
                                IHttpClientFactory httpClient) : ILibraryService
    {
        public async Task<Result<LibraryResponse>> CreateLibraryItemAsync(LibraryRequest request, string correlationId, CancellationToken cancellationToken = default)
        {
            var validation = validator.Validate(request);
            if (!validation.IsValid)
                return Result.Failure<LibraryResponse>(new Error("400", string.Join("; ", validation.Errors.Select(e => e.ErrorMessage))));

            var existe = await repository.ExistsAsync(request, cancellationToken);

            if(existe)            
                return Result.Failure<LibraryResponse>(new Error("409", "Este item já existe na biblioteca do usuário."));

            var gamesClient = httpClient.CreateClient("GamesApi");
            var gameResponse = await gamesClient.GetAsync($"api/{request.GameId}", cancellationToken);

            if (!gameResponse.IsSuccessStatusCode)
                return Result.Failure<LibraryResponse>(new Error("404", "Jogo não cadastrado."));

            var userClient = httpClient.CreateClient("UsersApi");
            var userResponse = await userClient.GetAsync($"api/{request.UserId}", cancellationToken);

            if (!userResponse.IsSuccessStatusCode)
                return Result.Failure<LibraryResponse>(new Error("404", "Usuário não cadastrado."));

            var gameJson = await gameResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            decimal? price = null;
            if (gameJson.ValueKind == JsonValueKind.Object && gameJson.TryGetProperty("price", out var priceProp))
            {
                if (priceProp.ValueKind == JsonValueKind.Number)
                {
                    if (priceProp.TryGetDecimal(out var p))
                        price = p;
                }
                else if (priceProp.ValueKind == JsonValueKind.String)
                {
                    var s = priceProp.GetString();
                    if (decimal.TryParse(s, out var p))
                        price = p;
                }
            }

            var library = Library.Create(request.UserId, request.GameId, price);
                        
            var evt = new LibraryItemCreatedEvent(library.Id.ToString(), library.UserId, library.GameId, EOrderStatus.Requested, library.PricePaid);

            await eventStore.AppendAsync(evt.AggregateId, evt, 0, correlationId);

            await publisher.PublishAsync(evt, "LibraryItemCreated", correlationId);  

            return Result.Success(new LibraryResponse(
                ItemId: library.Id,
                UserId: library.UserId,
                GameId: library.GameId,
                Status: library.Status,
                PricePaid: library.PricePaid));
        }

        public async Task<Result> DeleteLibraryItemAsync(Guid id, string correlationId, CancellationToken cancellationToken = default)
        {
            var result = await repository.GetByIdAsync(id, cancellationToken);
            if (result is null)
                return Result.Failure(new Error("404", "Item da biblioteca não encontrado."));

            var events = await eventStore.GetEventsAsync(id.ToString());
            var currentVersion = events.Count;

            var evt = new LibraryItemDeletedEvent(id.ToString(), result.UserId, result.GameId);

            await eventStore.AppendAsync(id.ToString(), evt, currentVersion, correlationId);

            await publisher.PublishAsync(evt, "LibraryItemDeleted", correlationId);

            return Result.Success(new LibraryResponse(result.Id, result.UserId, result.GameId, result.Status, result.PricePaid));
        }

        public async Task<Result<IEnumerable<LibraryResponse>>> GetAllLibrariesAsync(CancellationToken cancellationToken = default)
        {
            var result = await repository.GetAllAsync(cancellationToken);

            var libraries = result.Select(library => new LibraryResponse(
                ItemId: library.Id,
                UserId: library.UserId,
                GameId: library.GameId,
                Status: library.Status,
                PricePaid: library.PricePaid));

            return Result.Success(libraries);
        }

        public async Task<Result<IEnumerable<LibraryResponse>>> GetLibrariesByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            var result = await repository.GetLibrariesAsync(library => library.GameId == gameId);
            return Result.Success(result.Select(l => new LibraryResponse(l.Id, l.UserId, l.GameId, l.Status, l.PricePaid)));
        }

        public async Task<Result<IEnumerable<LibraryResponse>>> GetAcquiredItemsByUser(Guid userId, CancellationToken cancellationToken = default)
        {
            var result = await repository.GetLibrariesAsync(library => library.UserId == userId && library.Status == EOrderStatus.Owned);
            return Result.Success(result.Select(l => new LibraryResponse(l.Id, l.UserId, l.GameId, l.Status, l.PricePaid)));
        }
        
        public async Task<Result<IEnumerable<LibraryResponse>>> GetRequestedItemsByUser(Guid userId, CancellationToken cancellationToken = default)
        {
            var result = await repository.GetLibrariesAsync(library => library.UserId == userId && library.Status != EOrderStatus.Owned);
            return Result.Success(result.Select(l => new LibraryResponse(l.Id, l.UserId, l.GameId, l.Status, l.PricePaid)));
        }

        public async Task<Result<LibraryResponse>> GetLibraryByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await repository.GetByIdAsync(id);
            if (result is null)
                return Result.Failure<LibraryResponse>(new Error("404", "Item não encontrado"));

            return Result.Success(new LibraryResponse(result.Id, result.UserId, result.GameId, result.Status, result.PricePaid));
        }       

        public async Task<Result<LibraryResponse>> UpdateStatusAsync(Guid id, EOrderStatus status, string correlationId, CancellationToken cancellationToken = default)
        {          
            if(!Enum.IsDefined(typeof(EOrderStatus), status))            
                return Result.Failure<LibraryResponse>(new Error("400", "Status inválido."));
            
            var libraryItem = await repository.GetByIdAsync(id, cancellationToken);
            if (libraryItem is null)
                return Result.Failure<LibraryResponse>(new Error("404", "Item da biblioteca não encontrado."));

            libraryItem.UpdateStatus(status);

            var events = await eventStore.GetEventsAsync(id.ToString());
            var currentVersion = events.Count;

            var evt = new LibraryItemUpdatedEvent(id.ToString(), status);

            await eventStore.AppendAsync(id.ToString(), evt, currentVersion, correlationId);

            await publisher.PublishAsync(evt, "LibraryItemUpdated", correlationId);

            return Result.Success(new LibraryResponse(
                ItemId: libraryItem.Id,
                UserId: libraryItem.UserId,
                GameId: libraryItem.GameId,
                Status: libraryItem.Status,
                PricePaid: libraryItem.PricePaid));
        }
    
        public async Task<Result<IEnumerable<LibraryResponse>>> GetLibrariesByPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            var result = await repository.GetLibrariesAsync(library => library.PaymentId == paymentId);
            return Result.Success(result.Select(l => new LibraryResponse(l.Id, l.UserId, l.GameId, l.Status, l.PricePaid)));

        }
    }
}
