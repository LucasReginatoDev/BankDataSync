using BankDataSync.Services;
using System;
using System.Net.Http;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using Polly;
using Polly.Extensions.Http;

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() // Lida com erros de rede e respostas 5XX
        .Or<TaskCanceledException>() // Inclui timeouts
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))); // Delay exponencial entre tentativas
}

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog((context, services, configuration) => configuration
        .WriteTo.Console()
        .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    )
    .ConfigureServices(services =>
    {
        // Configuração do HttpClient para BacenApiService com Polly e Timeout
        services.AddHttpClient<BacenApiService>(client =>
        {
            client.Timeout = TimeSpan.FromMinutes(1);
        })
        .AddPolicyHandler(GetRetryPolicy());

        services.AddHttpClient<BrasilApiService>(client =>
        {
            client.Timeout = TimeSpan.FromMinutes(1);
        })
        .AddPolicyHandler(GetRetryPolicy());

        // Adiciona o Worker como um serviço hospedado
        services.AddHostedService<Worker>();

        // Adiciona o JsonStorageService
        services.AddSingleton<JsonStorageService>();
    })
    .Build();

await host.RunAsync();
