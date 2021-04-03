using Contracts;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Service
{
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

                await _bus.Send<IMessageWithTenant>(message, cancellationToken: stoppingToken);

                _logger.LogInformation($"Sending message with OrderId {message.OrderId} and tenant {message.Tenant}");

                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}
