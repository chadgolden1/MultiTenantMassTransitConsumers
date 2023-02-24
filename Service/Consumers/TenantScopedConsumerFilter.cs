using Contracts;
using MassTransit;

namespace Service.Consumers;

public class TenantScopedConsumerFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    private readonly ITenantInfo _tenantInfo;

    public TenantScopedConsumerFilter(ITenantInfo tenantInfo)
    {
        _tenantInfo = tenantInfo;
    }

    public void Probe(ProbeContext context)
    {
    }

    public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        var tenantedMessage = context.Message as IMessageWithTenant;

        if (tenantedMessage is not null)
        {
            _tenantInfo.TenantName = tenantedMessage.Tenant;
        }

        return next.Send(context);
    }
}