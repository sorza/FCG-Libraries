using Azure.Messaging.ServiceBus;
using FCG.Shared.Contracts.Events.Domain.Libraries;
using FCG_Libraries.Application.Libraries.Requests;
using FCG_Libraries.Application.Shared.Interfaces;
using FCG_Libraries.Domain.Libraries.Entities;
using System.Text.Json;

namespace FCG_Libraries.Consumer.Consumers
{
    public class LibrariesTopicConsumer : IHostedService
    {
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<LibrariesTopicConsumer> _logger;

        public LibrariesTopicConsumer(ServiceBusClient client, IConfiguration cfg, IServiceScopeFactory scopeFactory, ILogger<LibrariesTopicConsumer> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            var topicName = cfg["ServiceBus:Topics:Libraries"];
            var subscriptionName = cfg["ServiceBus:Subscriptions:Libraries"];

            _processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 4,
                PrefetchCount = 20
            });

            _processor.ProcessMessageAsync += OnMessageAsync;
            _processor.ProcessErrorAsync += OnErrorAsync;
        }

        private async Task OnMessageAsync(ProcessMessageEventArgs args)
        {
            var subject = args.Message.Subject;
            var body = args.Message.Body.ToString();

            _logger.LogInformation("Mensagem recebida: Subject={Subject}, CorrelationId={CorrelationId}", subject, args.Message.CorrelationId);

           switch (subject)
            {
                case "LibraryItemCreated":
                    await HandleLibraryItemCreatedEvent(body);
                    break;                
                case "LibraryItemDeleted":
                    await HandleLibraryItemDeletedEvent(body);
                    break;
                default:
                    _logger.LogWarning("Evento desconhecido: {Subject}", subject);
                    break;
            }

            await args.CompleteMessageAsync(args.Message);
        }

        private async Task HandleLibraryItemDeletedEvent(string body)
        {
            var evt = JsonSerializer.Deserialize<LibraryItemDeletedEvent>(body);

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ILibraryRepository>();

            var item = await repo.GetByIdAsync(Guid.Parse(evt!.AggregateId));

            if (item is not null)
                await repo.DeleteAsync(item.Id);

        }

        private async Task HandleLibraryItemCreatedEvent(string body)
        {
            var evt = JsonSerializer.Deserialize<LibraryItemCreatedEvent>(body);

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ILibraryRepository>();

            var item = Library.Create(evt!.UserId, evt.GameId, evt.PricePaid);

            if (!await repo.ExistsAsync(new LibraryRequest(item.UserId, item.GameId)))            
                await repo.AddAsync(item);           
                
        }

        private Task OnErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Erro no consumer: {EntityPath}", args.EntityPath);
            return Task.CompletedTask;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consumer iniciado para {Topic}/{Subscription}", _processor.EntityPath, "libraries-api-sub");
            await _processor.StartProcessingAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consumer parado");
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
        }
    }
}
