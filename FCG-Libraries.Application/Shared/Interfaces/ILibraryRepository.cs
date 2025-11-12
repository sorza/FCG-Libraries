using FCG_Libraries.Domain.Libraries.Entities;

namespace FCG_Libraries.Application.Shared.Interfaces
{
    public interface ILibraryRepository : IRepository<Library>
    {
        Task<bool> Exists(Library game, CancellationToken cancellationToken = default);
    }
}
