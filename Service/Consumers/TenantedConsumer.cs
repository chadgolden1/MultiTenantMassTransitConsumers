using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Service.Consumers
{
    public class TenantedConsumer : IConsumer<IMessageWithTenant>
    {
        private readonly ILogger _logger;
        private readonly ITenantInfo _tenantInfo;

        public TenantedConsumer(ILogger<TenantedConsumer> logger, ITenantInfo tenantInfo)
        {
            _logger = logger;
            _tenantInfo = tenantInfo;
        }

        public Task Consume(ConsumeContext<IMessageWithTenant> context)
        {
            _logger.LogInformation($"Processing message with Tenant={_tenantInfo.TenantName} and OrderId={context.Message.OrderId}");
            return Task.CompletedTask;
        }
    }
}
