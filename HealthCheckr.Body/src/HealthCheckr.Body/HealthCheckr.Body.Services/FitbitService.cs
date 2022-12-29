using Azure.Security.KeyVault.Secrets;
using HealthCheckr.Body.Common;
using HealthCheckr.Body.Common.FitbitResponses;
using HealthCheckr.Body.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HealthCheckr.Body.Services
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

        public async Task<WeightResponseObject> GetWeightResponse(string startDate, string endDate)
        {
            try
            {
                KeyVaultSecret fitbitAccessToken = await _secretClient.GetSecretAsync("AccessToken");
                _httpClient.DefaultRequestHeaders.Clear();
                Uri getMonthlyWeightLogs = new Uri($"https://api.fitbit.com/1/user/-/body/log/weight/date/{startDate}/{endDate}.json");
                var request = new HttpRequestMessage(HttpMethod.Get, getMonthlyWeightLogs);
                request.Content = new StringContent("");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fitbitAccessToken.Value);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();

                var weightResponse = JsonConvert.DeserializeObject<WeightResponseObject>(responseString);

                return weightResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetWeightResponse)}: {ex.Message}");
                throw;
            }
        }
    }
}