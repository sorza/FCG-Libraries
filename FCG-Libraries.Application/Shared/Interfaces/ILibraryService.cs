using FCG.Shared.Contracts.Enums;
using FCG.Shared.Contracts.Results;
using FCG_Libraries.Application.Libraries.Requests;
using FCG_Libraries.Application.Libraries.Responses;

namespace FCG_Libraries.Application.Shared.Interfaces
{
    public interface ILibraryService
    {
        Task<Result<LibraryResponse>> CreateLibraryItemAsync(LibraryRequest request, string correlationId,CancellationToken cancellationToken = default);
        Task<Result<LibraryResponse>> GetLibraryByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<LibraryResponse>>> GetAllLibrariesAsync(CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<LibraryResponse>>> GetAcquiredItemsByUser(Guid userId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<LibraryResponse>>> GetRequestedItemsByUser(Guid userId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<LibraryResponse>>> GetLibrariesByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);
        Task<Result<LibraryResponse>> UpdateStatusAsync(Guid id, EOrderStatus status, string correlationId, CancellationToken cancellationToken = default);        
        Task<Result> DeleteLibraryItemAsync(Guid id, string correlationId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<LibraryResponse>>> GetLibrariesByPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default);
    }
}
