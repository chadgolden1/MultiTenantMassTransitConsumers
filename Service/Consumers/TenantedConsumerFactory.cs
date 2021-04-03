using Contracts;
using GreenPipes;
using MassTransit;
using MassTransit.Context;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Service.Consumers
{
    public class TenantedConsumerFactory : IConsumerFactory<TenantedConsumer>
    {
        private readonly IServiceProvider _serviceProvider;

        public TenantedConsumerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Send<TMessage>(ConsumeContext<TMessage> context, IPipe<ConsumerConsumeContext<TenantedConsumer, TMessage>> next)
            where TMessage : class
        {
            TenantedConsumer consumer = null;
            try
            {
                using var scope = _serviceProvider.CreateScope();

                var tenant = scope.ServiceProvider.GetRequiredService<ITenantInfo>();
                tenant.TenantName = (context.Message as IMessageWithTenant).Tenant;

                consumer = ActivatorUtilities.CreateInstance<TenantedConsumer>(scope.ServiceProvider);

                await next.Send(new ConsumerConsumeContextScope<TenantedConsumer, TMessage>(context, consumer)).ConfigureAwait(false);
            }
            finally
            {
                switch (consumer)
                {
                    case IAsyncDisposable asyncDisposable:
                        await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                        break;
                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                }
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateConsumerFactoryScope<TenantedConsumer>("delegate");
        }
    }
}