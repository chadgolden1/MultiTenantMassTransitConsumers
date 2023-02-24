using Contracts;
using MassTransit;

namespace Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IBus _bus;

    private int _sendCount = 0;

    public Worker(ILogger<Worker> logger, IBus endpoint)
    {
        _logger = logger;
        _bus = endpoint;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var message = new
            {
                OrderId = _sendCount++,
                Tenant = Guid.NewGuid().ToString().Substring(0, 4)
            };

            await _bus.Publish<IMessageWithTenant>(message, cancellationToken: stoppingToken);

            _logger.LogInformation("Sending message with OrderId {OrderId} and tenant {Tenant}", message.OrderId, message.Tenant);

            await Task.Delay(3000, stoppingToken);
        }
    }
}
