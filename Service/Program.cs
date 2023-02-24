using MassTransit;
using Service;
using Service.Consumers;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddScoped<ITenantInfo, TenantInfo>();
        services.AddMassTransit(mt =>
        {
            mt.AddConsumers(typeof(Program).Assembly);
            mt.UsingInMemory((ctx, mc) =>
            {
                mc.UseConsumeFilter(typeof(TenantScopedConsumerFilter<>), ctx);
                mc.ConfigureEndpoints(ctx);
            });
        });
        services.AddHostedService<Worker>();
    });

await host.Build().RunAsync();
