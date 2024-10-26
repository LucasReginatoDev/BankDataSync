using BankDataSync.Services;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog((context, services, configuration) => configuration
        .WriteTo.Console() // Loga no console
        .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day) // Loga em arquivo
    )
    .ConfigureServices(services =>
    {
        // Configuração dos serviços de HttpClient
        services.AddHttpClient<BacenApiService>();
        services.AddHttpClient<BrasilApiService>();

        // Adiciona JsonStorageService ao contêiner de DI
        services.AddSingleton<JsonStorageService>();

        // Configurando o Worker como serviço windows service 
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
