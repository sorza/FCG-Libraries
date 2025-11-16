using Azure.Messaging.ServiceBus;
using FCG.Shared.Contracts;
using FCG_Libraries.Application.Shared.Interfaces;
using System.Text.Json;

namespace FCG_Libraries.WorkService.Consumers
{
    public class UserEventsConsumer : BackgroundService
    {
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceScopeFactory _scopeFactory;

        public UserEventsConsumer(ServiceBusClient client, IConfiguration config, IServiceScopeFactory scopeFactory)
        {
            var queueName = config["ServiceBus:Queues:UsersEvents"];
            _processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync += async args =>
            {
                var body = args.Message.Body.ToString();
                var subject = args.Message.Subject;

                if (subject == "UserDeleted")
                {
                    var evt = JsonSerializer.Deserialize<UserDeletedEvent>(body);

                    if (evt is not null)
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var repo = scope.ServiceProvider.GetRequiredService<ILibraryRepository>();

                        Console.WriteLine($"Removendo itens da biblioteca do usuário: {evt.id}");
                        await repo.DeleteAsync(library => library.UserId == evt.id, stoppingToken);
                    }
                }

                await args.CompleteMessageAsync(args.Message);
            };

            _processor.ProcessErrorAsync += args =>
            {
                Console.WriteLine($"Erro no consumer: {args.Exception.Message}");
                return Task.CompletedTask;
            };

            await _processor.StartProcessingAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopProcessingAsync();
            await base.StopAsync(cancellationToken);
        }

    }
}
