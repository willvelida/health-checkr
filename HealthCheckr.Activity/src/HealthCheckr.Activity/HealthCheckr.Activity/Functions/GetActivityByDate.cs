using HealthCheckr.Activity.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace HealthCheckr.Activity.Functions
{
    public class GetActivityByDate
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<GetActivityByDate> _logger;

        public GetActivityByDate(IActivityService activityService, ILogger<GetActivityByDate> logger)
        {
            _activityService = activityService;
            _logger = logger;
        }

        [Function(nameof(GetActivityByDate))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Activity/{activityDate}")] HttpRequestData req, string activityDate)
        {
            IActionResult result;

            try
            {
                var activityResponse = await _activityService.GetActivityRecordByDate(activityDate);
                if (activityResponse == null)
                {
                    result = new NotFoundResult();
                    return result;
                }

                result = new OkObjectResult(activityResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivityByDate)}: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
