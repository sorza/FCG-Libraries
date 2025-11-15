using Azure.Messaging.ServiceBus;
using FCG_Libraries.Application.Shared.Interfaces;
using FCG_Libraries.Infrastructure.Libraries.Repositories;
using FCG_Libraries.Infrastructure.Shared.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FCG_Libraries.WorkService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<LibraryDbContext>(options =>
                        options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

                    var connectionString = context.Configuration["ServiceBus:ConnectionString"];
                    services.AddSingleton(new ServiceBusClient(connectionString));

                    services.AddScoped<ILibraryRepository, LibraryRepository>();
                    services.AddHostedService<GameEventsConsumer>();
                });

            await builder.RunConsoleAsync();
        }
    }
}