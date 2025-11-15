using Azure.Messaging.ServiceBus;
using FCG.Shared.Contracts;
using FCG_Libraries.Application.Shared.Interfaces;
using System.Text.Json;

namespace FCG_Libraries.WorkService
{
    public class GameEventsConsumer : BackgroundService
    {
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceScopeFactory _scopeFactory;

        public GameEventsConsumer(ServiceBusClient client, IConfiguration config, IServiceScopeFactory scopeFactory)
        {
            var queueName = config["ServiceBus:Queues:GamesEvents"];
            _processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync += async args =>
            {
                var body = args.Message.Body.ToString();
                var subject = args.Message.Subject;

                if (subject == "GameDeleted")
                {
                    var evt = JsonSerializer.Deserialize<GameDeletedEvent>(body);

                    if (evt is not null)
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var repo = scope.ServiceProvider.GetRequiredService<ILibraryRepository>();

                        Console.WriteLine($"Removendo jogos da biblioteca do jogo {evt.Id}");
                        await repo.DeleteAsync(library => library.GameId == evt.Id, stoppingToken);
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
