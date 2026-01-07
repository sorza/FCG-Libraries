using FCG_Libraries.Application.Libraries.Requests;
using FCG_Libraries.Application.Shared.Interfaces;
using FCG_Libraries.Domain.Libraries.Entities;
using FCG_Libraries.Infrastructure.Shared.Context;
using FCG_Libraries.Infrastructure.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FCG_Libraries.Infrastructure.Libraries.Repositories
{
    public class LibraryRepository : GenericRepository<Library>, ILibraryRepository
    {
        private readonly LibraryDbContext _context;
        public LibraryRepository(LibraryDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Library>> GetLibrariesAsync(Func<Library, bool> predicate, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_context.Libraries
                .AsNoTracking()
                .Where(predicate)
                .ToList());
        }

        public async Task<bool> ExistsAsync(LibraryRequest item, CancellationToken cancellationToken = default)
        => await _context.Libraries
                .AsNoTracking()
                .AnyAsync(g => g.UserId == item.UserId &&
                               g.GameId == item.GameId,                            
                               cancellationToken);
    }
}
