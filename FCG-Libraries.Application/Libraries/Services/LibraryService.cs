using FCG.Shared.Contracts;
using FCG.Shared.Contracts.Enums;
using FCG_Libraries.Application.Libraries.Requests;
using FCG_Libraries.Application.Libraries.Responses;
using FCG_Libraries.Application.Shared.Interfaces;
using FCG_Libraries.Application.Shared.Results;
using FCG_Libraries.Domain.Libraries.Entities;
using FCG_Libraries.Domain.Libraries.Enums;
using FluentValidation;

namespace FCG_Libraries.Application.Libraries.Services
{
    public class LibraryService(ILibraryRepository repository,
                                IValidator<LibraryRequest> validator,
                                IEventPublisher publisher,
                                IHttpClientFactory httpClient) : ILibraryService
    {
        public async Task<Result<LibraryResponse>> CreateLibraryAsync(LibraryRequest request, CancellationToken cancellationToken = default)
        {
            var validation = validator.Validate(request);
            if (!validation.IsValid)
                return Result.Failure<LibraryResponse>(new Error("400", string.Join("; ", validation.Errors.Select(e => e.ErrorMessage))));

            var existe = await repository.ExistsAsync(request, cancellationToken);

            if(existe)            
                return Result.Failure<LibraryResponse>(new Error("409", "Este item já existe na biblioteca do usuário."));

            var gamesClient = httpClient.CreateClient("GamesApi");
            var gameResponse = await gamesClient.GetAsync($"games/{request.GameId}", cancellationToken);

            if (!gameResponse.IsSuccessStatusCode)
                return Result.Failure<LibraryResponse>(new Error("404", "Jogo não cadastrado."));

            var userClient = httpClient.CreateClient("UsersApi");
            var userResponse = await userClient.GetAsync($"users/{request.UserId}", cancellationToken);

            if (!userResponse.IsSuccessStatusCode)
                return Result.Failure<LibraryResponse>(new Error("404", "Usuário não cadastrado."));

            var library = Library.Create(request.UserId, request.GameId, request.PricePaid, request.PaymentType);

            if(library.PaymentType == EPaymentType.Free)
            {
                var evt_itemCreated = new LibraryItemCreatedEvent(library.Id, library.UserId, library.GameId, EStatus.Owned.ToString(), 0, EPaymentType.Free.ToString());
                await publisher.PublishAsync(evt_itemCreated, "LibraryItemCreated");
               
            }
            else
            {
                var evt_order = new LibraryOrderEvent(library.Id, library.UserId, library.GameId, EStatus.Requested.ToString(), 0, request.PaymentType.ToString());
                await publisher.PublishAsync(evt_order, "OrderCreated");

            }

            await repository.AddAsync(library, cancellationToken);

            return Result.Success(new LibraryResponse(
                ItemId: library.Id,
                UserId: library.UserId,
                GameId: library.GameId,
                Status: library.Status,
                PricePaid: library.PricePaid,
                PaymentType: library.PaymentType));
        }

        public async Task<Result> DeleteLibraryAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await repository.GetByIdAsync(id, cancellationToken);
            if (result is null)
                return Result.Failure(new Error("404", "Item da biblioteca não encontrado."));

            await repository.DeleteAsync(id, cancellationToken);
            return Result.Success(new LibraryResponse(result.Id, result.UserId, result.GameId, result.Status, result.PricePaid, result.PaymentType));
        }

        public async Task<Result<IEnumerable<LibraryResponse>>> GetAllLibrariesAsync(CancellationToken cancellationToken = default)
        {
            var result = await repository.GetAllAsync(cancellationToken);

            var libraries = result.Select(library => new LibraryResponse(
                ItemId: library.Id,
                UserId: library.UserId,
                GameId: library.GameId,
                Status: library.Status,
                PricePaid: library.PricePaid,
                PaymentType: library.PaymentType));

            return Result.Success(libraries);
        }

        public async Task<Result<IEnumerable<LibraryResponse>>> GetLibrariesByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            var result = await repository.GetAllAsync(cancellationToken);

            var libraries = result
                .Where(library => library.GameId == gameId)
                .Select(library => new LibraryResponse(
                    ItemId: library.Id,
                    UserId: library.UserId,
                    GameId: library.GameId,
                    Status: library.Status,
                    PricePaid: library.PricePaid,
                    PaymentType: library.PaymentType));

            return Result.Success(libraries);
        }

        public async Task<Result<IEnumerable<LibraryResponse>>> GetLibrariesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var result = await repository.GetAllAsync(cancellationToken);

            var libraries = result
                .Where(library => library.UserId == userId)
                .Select(library => new LibraryResponse(
                    ItemId: library.Id,
                    UserId: library.UserId,
                    GameId: library.GameId,
                    Status: library.Status,
                    PricePaid: library.PricePaid,
                    PaymentType: library.PaymentType));

            return Result.Success(libraries);
        }

        public async Task<Result<LibraryResponse>> GetLibraryByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await repository.GetByIdAsync(id, cancellationToken);
            if (result is null)
                return Result.Failure<LibraryResponse>(new Error("404", "Item da biblioteca não encontrado."));

            var libraryResponse = new LibraryResponse(
                ItemId: result.Id,
                UserId: result.UserId,
                GameId: result.GameId,
                Status: result.Status,
                PricePaid: result.PricePaid, 
                PaymentType: result.PaymentType);

            return Result.Success(libraryResponse);
        }       

        public async Task<Result<LibraryResponse>> UpdateStatusAsync(Guid id, EStatus status, CancellationToken cancellationToken = default)
        {          
            if(!Enum.IsDefined(typeof(EStatus), status))            
                return Result.Failure<LibraryResponse>(new Error("400", "Status inválido."));
            
            var libraryItem = await repository.GetByIdAsync(id, cancellationToken);
            if (libraryItem is null)
                return Result.Failure<LibraryResponse>(new Error("404", "Item da biblioteca não encontrado."));

            libraryItem.UpdateStatus(status);

            await repository.UpdateAsync(libraryItem, cancellationToken);

            return Result.Success(new LibraryResponse(
                ItemId: libraryItem.Id,
                UserId: libraryItem.UserId,
                GameId: libraryItem.GameId,
                Status: libraryItem.Status,
                PricePaid: libraryItem.PricePaid,
                PaymentType: libraryItem.PaymentType));
        }
    }
}
