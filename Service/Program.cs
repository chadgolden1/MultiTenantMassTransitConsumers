using Contracts;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Consumers;
using System;

namespace Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped<ITenantInfo, TenantInfo>();
                    services.AddMassTransit(mt =>
                    {
                        mt.UsingInMemory((ctx, mc) =>
                        {
                            EndpointConvention.Map<IMessageWithTenant>(new Uri("queue:tenanted"));
                            mc.ReceiveEndpoint("tenanted", r =>
                            {
                                r.Consumer(new TenantedConsumerFactory(ctx.GetRequiredService<IServiceProvider>()));
                            });
                        });
                    });
                    services.AddMassTransitHostedService();
                    services.AddHostedService<Worker>();
                });
    }
}
