using Azure.Security.KeyVault.Secrets;
using HealthCheckr.Body.Common;
using HealthCheckr.Body.Common.FitbitResponses;
using HealthCheckr.Body.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

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

        public async Task<CardioResponseObject> GetV02MaxSummary(string date)
        {
            try
            {
                KeyVaultSecret fitbitAccessToken = await _secretClient.GetSecretAsync("AccessToken");
                _httpClient.DefaultRequestHeaders.Clear();
                Uri getV02MaxSummaryUri = new Uri($"https://api.fitbit.com/1/user/-/cardioscore/date/{date}.json");
                var request = new HttpRequestMessage(HttpMethod.Get, getV02MaxSummaryUri);
                request.Content = new StringContent("", Encoding.UTF8, "application/json");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fitbitAccessToken.Value);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();

                var cardioResponse = JsonConvert.DeserializeObject<CardioResponseObject>(responseString);

                return cardioResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetV02MaxSummary)}: {ex.Message}");
                throw;
            }
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