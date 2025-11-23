using Azure.Messaging.ServiceBus;
using FCG_Libraries.Application.Shared.Interfaces;
using FCG_Libraries.Infrastructure.Libraries.Repositories;
using FCG_Libraries.Infrastructure.Shared.Context;
using FCG_Libraries.WorkService.Consumers;
using Microsoft.EntityFrameworkCore;

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
                    services.AddHostedService<UserEventsConsumer>();
                    services.AddHostedService<PaymentEventsConsumer>();
                });

            await builder.RunConsoleAsync();
        }
    }
}