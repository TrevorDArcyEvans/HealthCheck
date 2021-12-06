namespace HealthCheck
{
  using System;
  using Custom;
  using HealthChecks.UI.Client;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Diagnostics.HealthChecks;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Microsoft.OpenApi.Models;

  public sealed class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddHttpClient<CatHealthCheck>(client => { client.BaseAddress = new Uri("https://http.cat/401"); });
      services
        .AddHealthChecks()
        .AddCheck<CatHealthCheck>("Cat Health Checks");
      services.AddControllers();
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
          Title = "HealthCheck", Version = "v1"
        });
      });

      // Here is the GUI setup and history storage
      services.AddHealthChecksUI(options =>
      {
        options.SetEvaluationTimeInSeconds(5); // Sets the time interval in which HealthCheck will be triggered
        options.MaximumHistoryEntriesPerEndpoint(10); // Sets the maximum number of records displayed in history
        options.AddHealthCheckEndpoint("Health Checks API", "/health"); // Sets the Health Check endpoint
      }).AddInMemoryStorage(); // Here is the memory bank configuration
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthCheck v1"));
      }

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();

        // Sets the health endpoint
        endpoints.MapHealthChecks("/health");
      });

      //Sets Health Check dashboard options
      app.UseHealthChecks("/health", new HealthCheckOptions
      {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
      });

      // Sets the Health Check dashboard configuration
      app.UseHealthChecksUI(options => { options.UIPath = "/dashboard"; });
    }
  }
}
