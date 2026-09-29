using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceBooking.Application.Common.Interfaces;
using ServiceBooking.Application.Common.Models;
using System.Text.Json;

namespace ServiceBooking.Infrastructure.Messaging;

public class BookingCreatedEmailConsumer : BackgroundService
{
    private readonly ServiceBusClient _client;
    private readonly ILogger<BookingCreatedEmailConsumer> _logger;
    private readonly IEmailService _emailService;
    private ServiceBusProcessor? _processor;
    private const string QueueName = "booking-created-queue";

    public BookingCreatedEmailConsumer(
        ServiceBusClient client,
        IEmailService emailService,
        ILogger<BookingCreatedEmailConsumer> logger)
    {
        _client = client;
        _emailService = emailService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor = _client.CreateProcessor(QueueName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false,
            MaxConcurrentCalls = 1
        });

        _processor.ProcessMessageAsync += HandleMessageAsync;
        _processor.ProcessErrorAsync += ErrorHandlerAsync;

        await _processor.StartProcessingAsync(stoppingToken);

        // Keep running until cancellation is requested
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleMessageAsync(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        var bookingData = JsonSerializer.Deserialize<BookingCreatedIntegrationEvent>(body);

        if (bookingData != null)
        {
            _logger.LogInformation("Sending confirmation email to: {Email} for Booking {BookingId}",
                bookingData.CustomerEmail, bookingData.BookingId);

            // TODO: Call your Email Provider (e.g., Azure Communication Services, SendGrid, Mailkit)
            // await _emailSender.SendEmailAsync(bookingData.CustomerEmail, "Booking Confirmed", ...);

            var subject = $"Booking Confirmation - #{bookingData.BookingId}";
            var messageBody = $"Hello,\n\nYour booking for service {bookingData.ServiceId} on {bookingData.BookingDateTime:f} is confirmed!";

            // Send the email
            await _emailService.SendEmailAsync(
                bookingData.CustomerEmail,
                subject,
                messageBody,
                args.CancellationToken);
        }

        // Acknowledge message removal from Azure Service Bus queue
        await args.CompleteMessageAsync(args.Message);
    }

    private Task ErrorHandlerAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error occurred while processing message from Service Bus.");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor != null)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
        }
        await base.StopAsync(cancellationToken);
    }
}