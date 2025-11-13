using FCG_Libraries.Application.Libraries.Requests;
using FCG_Libraries.Application.Libraries.Services;
using FCG_Libraries.Application.Libraries.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FCG_Libraries.Application.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ILibraryService, LibraryService>();
            services.AddScoped<IValidator<LibraryRequest>, LibraryRequestValidator>();

            return services;
        }
    }
}
