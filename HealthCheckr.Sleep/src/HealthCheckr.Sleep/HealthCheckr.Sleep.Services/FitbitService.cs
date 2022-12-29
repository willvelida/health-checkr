using Azure.Security.KeyVault.Secrets;
using HealthCheckr.Sleep.Common;
using HealthCheckr.Sleep.Common.FitbitResponses;
using HealthCheckr.Sleep.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HealthCheckr.Sleep.Services
{
    public class FitbitService : IFitbitService
    {
        private readonly Settings _settings;
        private readonly SecretClient _secretClient;
        private readonly HttpClient _httpClient;
        private readonly ILogger<FitbitService> _logger;

        public FitbitService(
            IOptions<Settings> options,
            SecretClient secretClient,
            HttpClient httpClient,
            ILogger<FitbitService> logger)
        {
            _settings = options.Value;
            _secretClient = secretClient;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<SleepResponseObject> GetSleepResponse(string date)
        {
            try
            {
                KeyVaultSecret fitbitAccessToken = await _secretClient.GetSecretAsync("AccessToken");
                _httpClient.DefaultRequestHeaders.Clear();
                Uri getDailySleepLogUri = new Uri($"https://api.fitbit.com/1/user/-/sleep/date/{date}.json");
                var request = new HttpRequestMessage(HttpMethod.Get, getDailySleepLogUri);
                request.Content = new StringContent("");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fitbitAccessToken.Value);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();

                var sleepResponse = JsonConvert.DeserializeObject<SleepResponseObject>(responseString);

                return sleepResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetSleepResponse)}: {ex.Message}");
                throw;
            }
        }
    }
}
