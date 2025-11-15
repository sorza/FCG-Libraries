using FCG_Libraries.Application.Libraries.Requests;
using FCG_Libraries.Application.Libraries.Responses;
using FCG_Libraries.Application.Shared.Results;

namespace FCG_Libraries.Application.Libraries.Services
{
    public interface ILibraryService
    {
        Task<Result<LibraryResponse>> CreateLibraryAsync(LibraryRequest request, CancellationToken cancellationToken = default);
        Task<Result<LibraryResponse>> GetLibraryByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<LibraryResponse>>> GetAllLibrariesAsync(CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<LibraryResponse>>> GetLibrariesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<LibraryResponse>>> GetLibrariesByGameIdAsync(Guid gameId, CancellationToken cancellationToken = default);
        Task<Result<LibraryResponse>> UpdateLibraryAsync(Guid id, LibraryRequest request,  CancellationToken cancellationToken = default);        
        Task<Result> DeleteLibraryAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
