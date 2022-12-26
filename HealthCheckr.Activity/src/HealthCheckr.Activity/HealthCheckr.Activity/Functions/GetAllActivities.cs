using HealthCheckr.Activity.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace HealthCheckr.Activity.Functions
{
    public class GetAllActivities
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<GetAllActivities> _logger;

        public GetAllActivities(IActivityService activityService, ILogger<GetAllActivities> logger)
        {
            _activityService = activityService;
            _logger = logger;
        }

        [Function(nameof(GetAllActivities))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Activity")] HttpRequestData req)
        {
            IActionResult result;

            try
            {
                var activities = await _activityService.GetAllActivityRecords();

                if (activities is null)
                {
                    result = new NotFoundResult();
                    return result;
                }

                result = new OkObjectResult(activities);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetAllActivities)}: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
