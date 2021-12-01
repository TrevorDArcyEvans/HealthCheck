namespace HealthCheck.Custom
{
  using System.Net;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;
  using Microsoft.Extensions.Diagnostics.HealthChecks;

  public sealed class CatHealthCheck : IHealthCheck
  {
    private readonly HttpClient _client;

    public CatHealthCheck(HttpClient client)
    {
      _client = client;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
      var response = await _client.GetAsync("", cancellationToken);

      return response.StatusCode == HttpStatusCode.OK
        ? await Task.FromResult(new HealthCheckResult(
          status: HealthStatus.Healthy,
          description: "The API is healthy （。＾▽＾）"))
        : await Task.FromResult(new HealthCheckResult(
          status: HealthStatus.Unhealthy,
          description: "The API is sick (‘﹏*๑)"));
    }
  }
}
