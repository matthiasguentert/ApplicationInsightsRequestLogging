using Azureblue.ApplicationInsights.RequestLogging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationInsightsRequestLoggingTests.Utils
{
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddAppInsightsHttpBodyLogging(o =>
            {
                o.SensitiveDataRegexes.Add("AccessToken");
                o.SensitiveDataRegexes.Add("Password");
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseAppInsightsHttpBodyLogging();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPost("/", async ctx =>
                {
                    ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await ctx.Response.WriteAsync("Hello from integration test");
                });
            });
        }
    }
}
