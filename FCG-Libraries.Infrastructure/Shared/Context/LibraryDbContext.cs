using FCG_Libraries.Domain.Libraries.Entities;
using FCG_Libraries.Infrastructure.Libraries.Mappings;
using Microsoft.EntityFrameworkCore;

namespace FCG_Libraries.Infrastructure.Shared.Context
{
    public class LibraryDbContext(DbContextOptions<LibraryDbContext> options) : DbContext(options)
    {
        public DbSet<Library> Libraries { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new LibraryMap());
        }
    }    
}
