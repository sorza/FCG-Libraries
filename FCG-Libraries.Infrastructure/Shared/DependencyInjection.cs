using FCG_Libraries.Application.Shared.Interfaces;
using FCG_Libraries.Infrastructure.Libraries.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FCG_Libraries.Infrastructure.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<ILibraryRepository, LibraryRepository>();

            return services;
        }
    }
}
