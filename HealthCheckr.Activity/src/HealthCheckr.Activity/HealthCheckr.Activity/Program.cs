using AutoMapper;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Security.KeyVault.Secrets;
using HealthCheckr.Activity.Common;
using HealthCheckr.Activity.Repository;
using HealthCheckr.Activity.Repository.Interfaces;
using HealthCheckr.Activity.Services;
using HealthCheckr.Activity.Services.Interfaces;
using HealthCheckr.Activity.Services.Mappers;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;

var mappingConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new MapHeartRateZoneToActivityHeartRateZonesRecord());
    cfg.AddProfile(new MapDistanceToActivityDistancesRecord());
    cfg.AddProfile(new MapSummaryToActivitySummaryRecord());
    cfg.AddProfile(new MapActivityToActivityRecord());
});

var mapper = mappingConfig.CreateMapper();

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
        s.AddAutoMapper(typeof(Program));
        s.AddSingleton(mapper);
        s.AddSingleton(sp =>
        {
            IConfiguration configuration = sp.GetService<IConfiguration>();
            return new SecretClient(new Uri(configuration["KeyVaultUri"]), new DefaultAzureCredential());
        });
        s.AddSingleton(sp =>
        {
            IConfiguration configuration = sp.GetService<IConfiguration>();
            return new ServiceBusClient(configuration["ServiceBusEndpoint"], new DefaultAzureCredential());
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
        s.AddDbContext<ActivityContext>(opt => opt.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString")));
        s.AddTransient<ICosmosDbRepository, CosmosDbRepository>();
        s.AddTransient<IActivityRepository, ActivityRepository>();
        s.AddTransient<IActivityService, ActivityService>();
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
