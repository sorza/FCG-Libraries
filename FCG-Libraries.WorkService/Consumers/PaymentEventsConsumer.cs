
using Azure.Messaging.ServiceBus;
using FCG.Shared.Contracts;
using FCG_Libraries.Application.Shared.Interfaces;
using FCG_Libraries.Domain.Libraries.Enums;
using System.Text.Json;

namespace FCG_Libraries.WorkService.Consumers
{
    public class PaymentEventsConsumer : BackgroundService
    {
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceScopeFactory _scopeFactory;

        public PaymentEventsConsumer(ServiceBusClient client, IConfiguration config, IServiceScopeFactory scopeFactory)
        {
            var queueName = config["ServiceBus:Queues:PaymentsEvents"];
            _processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync += async args =>
            {
                var body = args.Message.Body.ToString();
                var subject = args.Message.Subject;

                var evt = JsonSerializer.Deserialize<PaymentProcessedEvent>(body);
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<ILibraryRepository>();

                if(evt is not  null)
                {
                    if (subject == "PaymentFailedEvent")
                    {
                        var order = await repo.GetByIdAsync(evt.OrderId);

                        order!.UpdateStatus(EStatus.Failed);

                        await repo.UpdateAsync(order);

                        Console.WriteLine($"O pagamento do pedido {evt.OrderId} falhou.");
                    }
                    else
                    {
                        if (subject == "PaymentAprovedEvent")
                        {
                            var order = await repo.GetByIdAsync(evt.OrderId);

                            order!.UpdateStatus(EStatus.Owned);

                            await repo.UpdateAsync(order);

                            Console.WriteLine($"O pagamento do pedido {evt.OrderId} foi aprovado!");
                        }
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
    }
}
