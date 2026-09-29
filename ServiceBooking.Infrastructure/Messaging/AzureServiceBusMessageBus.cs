using System.Text.Json;
using Azure.Messaging.ServiceBus;
using ServiceBooking.Application.Common.Interfaces;

namespace ServiceBooking.Infrastructure.Messaging;

public class AzureServiceBusMessageBus : IMessageBus
{
    private readonly ServiceBusClient _client;

    public AzureServiceBusMessageBus(ServiceBusClient client)
    {
        _client = client;
    }

    public async Task PublishAsync<T>(T message, string queueOrTopicName, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"---> [Gate A4] Sending to Azure Service Bus Queue: '{queueOrTopicName}'");

        var sender = _client.CreateSender(queueOrTopicName);

        var jsonPayload = JsonSerializer.Serialize(message);
        var serviceBusMessage = new ServiceBusMessage(jsonPayload)
        {
            ContentType = "application/json",
            MessageId = Guid.NewGuid().ToString()
        };

        await sender.SendMessageAsync(serviceBusMessage, cancellationToken);
        Console.WriteLine("---> [Gate A5] Message successfully sent to Azure Service Bus!");
    }
}