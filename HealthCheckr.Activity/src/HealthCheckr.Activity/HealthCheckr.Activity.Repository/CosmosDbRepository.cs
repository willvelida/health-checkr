﻿using HealthCheckr.Activity.Common;
using HealthCheckr.Activity.Common.Envelopes;
using HealthCheckr.Activity.Repository.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HealthCheckr.Activity.Repository
{
    public class CosmosDbRepository : ICosmosDbRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;
        private readonly Settings _settings;
        private readonly ILogger<CosmosDbRepository> _logger;

        public CosmosDbRepository(CosmosClient cosmosClient, IOptions<Settings> options, ILogger<CosmosDbRepository> logger)
        {
            _settings = options.Value;
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(_settings.DatabaseName, _settings.ContainerName);
            _logger = logger;
        }

        public async Task CreateActivityDocument(ActivityEnvelope activityEnvelope)
        {
            try
            {
                ItemRequestOptions itemRequestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false
                };

                await _container.CreateItemAsync(activityEnvelope, new PartitionKey(activityEnvelope.DocumentType), itemRequestOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateActivityDocument)}: {ex.Message}");
                throw;
            }
        }

        public async Task CreateHeartRateDocument(HeartRateEnvelope heartRateEnvelope)
        {
            try
            {
                ItemRequestOptions itemRequestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false
                };

                await _container.CreateItemAsync(heartRateEnvelope, new PartitionKey(heartRateEnvelope.DocumentType), itemRequestOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateHeartRateDocument)}: {ex.Message}");
                throw;
            }
        }
    }
}
