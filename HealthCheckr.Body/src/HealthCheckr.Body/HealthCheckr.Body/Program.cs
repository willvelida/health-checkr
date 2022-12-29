using HealthCheckr.Body.Repository.Interfaces;
using HealthCheckr.Body.Repository;
using HealthCheckr.Body.Services.Interfaces;
using HealthCheckr.Body.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly.Extensions.Http;
using Polly;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using HealthCheckr.Body.Common;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(config =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory())
        .AddEnvironmentVariables();
        config.AddAzureAppConfiguration(options =>
        {
            options
            .Connect(new Uri(Environment.GetEnvironmentVariable("AzureAppConfigEndpoint")), new DefaultAzureCredential())
            .Select(KeyFilter.Any, LabelFilter.Null);
        });
    })
    .ConfigureServices(s =>
    {
        s.AddHttpClient();
        s.AddLogging();
        s.AddOptions<Settings>()
        .Configure<IConfiguration>((settings, configuration) =>
        {
            configuration.GetSection("HealthCheckr").Bind(settings);
        });
        s.AddSingleton(sp =>
        {
            IConfiguration configuration = sp.GetService<IConfiguration>();
            return new SecretClient(new Uri(configuration["KeyVaultUri"]), new DefaultAzureCredential());
        });
        s.AddSingleton(sp =>
        {
            IConfiguration configuration = sp.GetService<IConfiguration>();
            return new ServiceBusClient(configuration["ServiceBusConnection"], new DefaultAzureCredential());
        });
        s.AddSingleton(sp =>
        {
            IConfiguration configuration = sp.GetService<IConfiguration>();
            CosmosClientOptions cosmosClientOptions = new CosmosClientOptions
            {
                MaxRetryAttemptsOnRateLimitedRequests = 5,
                MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(10),
            };
            return new CosmosClient(configuration["CosmosDbEndpoint"], new DefaultAzureCredential(), cosmosClientOptions);
        });
        s.AddTransient<ICosmosDbRepository, CosmosDbRepository>();
        s.AddTransient<IBodyService, BodyService>();
        s.AddHttpClient<IFitbitService, FitbitService>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(15))
            .AddPolicyHandler(GetRetryPolicy());
    })
    .Build();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                                                                            retryAttempt)));
}

host.Run();
