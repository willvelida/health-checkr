using Azure.Security.KeyVault.Secrets;
using HealthCheckr.Activity.Common.FitbitResponses;
using HealthCheckr.Activity.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HealthCheckr.Activity.Services
{
    public class FitbitService : IFitbitService
    {
        private readonly SecretClient _secretClient;
        private readonly HttpClient _httpClient;
        private readonly ILogger<FitbitService> _logger;

        public FitbitService(
            SecretClient secretClient,
            HttpClient httpClient,
            ILogger<FitbitService> logger)
        {
            _secretClient = secretClient;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ActivityResponse> GetActivityResponse(string date)
        {
            try
            {
                KeyVaultSecret fitbitAccessToken = await _secretClient.GetSecretAsync("AccessToken");
                _httpClient.DefaultRequestHeaders.Clear();
                Uri getDailyActivityLogUri = new Uri($"https://api.fitbit.com/1/user/-/activities/date/{date}.json");
                var request = new HttpRequestMessage(HttpMethod.Get, getDailyActivityLogUri);
                request.Content = new StringContent("");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fitbitAccessToken.Value);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var activityResponse = JsonConvert.DeserializeObject<ActivityResponse>(responseString);

                return activityResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivityResponse)}: {ex.Message}");
                throw;
            }
        }

        public async Task<HeartRateTimeSeriesResponse> GetHeartRateTimeSeriesByDate(string date)
        {
            try
            {
                KeyVaultSecret fitbitAccessToken = await _secretClient.GetSecretAsync("AccessToken");
                _httpClient.DefaultRequestHeaders.Clear();
                Uri getDailyHeartRateTimeSeriesLogUri = new Uri($"https://api.fitbit.com/1/user/-/activities/heart/date/{date}/1d.json");
                var request = new HttpRequestMessage(HttpMethod.Get, getDailyHeartRateTimeSeriesLogUri);
                request.Content = new StringContent("", Encoding.UTF8, "application/json");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fitbitAccessToken.Value);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var heartRateResponse = JsonConvert.DeserializeObject<HeartRateTimeSeriesResponse>(responseString);

                return heartRateResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetHeartRateTimeSeriesByDate)}: {ex.Message}");
                throw;
            }
        }
    }
}
