using Azure.Messaging.ServiceBus;
using FCG.Shared.Contracts.Enums;
using FCG.Shared.Contracts.Events.Domain.Payments;
using FCG_Libraries.Application.Shared.Interfaces;
using System.Text.Json;

namespace FCG_Libraries.Consumer.Consumers
{
    public class PaymentsTopicConsumer : IHostedService
    {
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PaymentsTopicConsumer> _logger;

        public PaymentsTopicConsumer(ServiceBusClient client, IConfiguration cfg, IServiceScopeFactory scopeFactory, ILogger<PaymentsTopicConsumer> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            var topicName = cfg["ServiceBus:Topics:Payments"];
            var subscriptionName = cfg["ServiceBus:Subscriptions:Payments"];

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
                case "PaymentAprovedEvent":
                    await HandlePaymentAprovedEvent(body);
                    break;
                case "PaymentFailedEvent":
                    await HandlePaymentFailedEvent(body);
                    break;

                default:
                    _logger.LogWarning("Evento desconhecido: {Subject}", subject);
                    break;
            }

            await args.CompleteMessageAsync(args.Message);
        }

        private async Task HandlePaymentFailedEvent(string body)
        {
            var evt = JsonSerializer.Deserialize<PaymentProcessedEvent>(body);

            if (evt is not null)
            {
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<ILibraryRepository>();

                var item = await repo.GetByIdAsync(Guid.Parse(evt.OrderId.ToString()));

                if (item is not null)
                {
                    item.UpdateStatus(EOrderStatus.Failed);
                    await repo.UpdateAsync(item);
                }
            }
        }

        private async Task HandlePaymentAprovedEvent(string body)
        {
            var evt = JsonSerializer.Deserialize<PaymentProcessedEvent>(body);

            if (evt is not null)
            {
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<ILibraryRepository>();

                var item = await repo.GetByIdAsync(Guid.Parse(evt.OrderId.ToString()));

                if (item is not null)
                {
                    item.UpdateStatus(EOrderStatus.Owned);
                    await repo.UpdateAsync(item);
                }
            }

        }

        private Task OnErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Erro no consumer: {EntityPath}", args.EntityPath);
            return Task.CompletedTask;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consumer iniciado para {Topic}/{Subscription}", _processor.EntityPath, "libraries-users-sub");
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
