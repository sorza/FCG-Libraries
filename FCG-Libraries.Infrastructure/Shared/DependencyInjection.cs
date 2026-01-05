using Azure.Messaging.ServiceBus;
using FCG.Shared.Contracts.Events.Store;
using FCG.Shared.Contracts.Interfaces;
using FCG_Libraries.Application.Shared.Interfaces;
using FCG_Libraries.Infrastructure.Libraries.Events;
using FCG_Libraries.Infrastructure.Libraries.Repositories;
using FCG_Libraries.Infrastructure.Shared.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace FCG_Libraries.Infrastructure.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
          

            var connectionString = configuration["ServiceBus:ConnectionString"];
            var topicName = configuration["ServiceBus:Topics:Libraries"];

            services.AddSingleton(new ServiceBusClient(connectionString));

            services.AddScoped<IEventPublisher>(sp =>
            {
                var client = sp.GetRequiredService<ServiceBusClient>();
                return new ServiceBusEventPublisher(client, topicName!);
            });

            var mongoString = configuration["MongoSettings:ConnectionString"];
            var mongoDb = configuration["MongoSettings:Database"];
            var mongoCollection = configuration["MongoSettings:Collection"];

            services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoString));

            services.AddScoped<IEventStore>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return new MongoEventStore(client, mongoDb!, mongoCollection!);
            });

            services.AddScoped<ILibraryRepository, LibraryRepository>();

            return services;
        }
    }
}
