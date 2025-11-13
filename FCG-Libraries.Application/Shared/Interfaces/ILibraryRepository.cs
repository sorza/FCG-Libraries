using FCG_Libraries.Application.Libraries.Requests;
using FCG_Libraries.Domain.Libraries.Entities;

namespace FCG_Libraries.Application.Shared.Interfaces
{
    public interface ILibraryRepository : IRepository<Library>
    {
        Task<bool> ExistsAsync(LibraryRequest request, CancellationToken cancellationToken = default);
    }
}
