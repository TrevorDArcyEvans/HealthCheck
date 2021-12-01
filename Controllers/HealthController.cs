namespace HealthCheck.Controllers
{
  using System.Net;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Diagnostics.HealthChecks;
  using Microsoft.Extensions.Logging;
  using Newtonsoft.Json;

  [ApiController]
  [Route("[controller]")]
  public sealed class HealthController : ControllerBase
  {
    private readonly ILogger<HealthController> _logger;
    private readonly HealthCheckService _service;

    public HealthController(
      ILogger<HealthController> logger,
      HealthCheckService service)
    {
      _logger = logger;
      _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      var report = await _service.CheckHealthAsync();
      var reportToJson = JsonConvert.SerializeObject(report, Formatting.None);

      _logger.LogInformation($"Get Health Information: {reportToJson}");

      return report.Status == HealthStatus.Healthy ? Ok(reportToJson) : StatusCode((int)HttpStatusCode.ServiceUnavailable, reportToJson);
    }
  }
}
