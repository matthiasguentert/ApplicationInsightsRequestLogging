using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ApplicationInsightsRequestLoggingTests.Utils
{
    public class CustomWebApplicationFactory : WebApplicationFactory<TestStartup>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder();

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddUserSecrets<CustomWebApplicationFactory>();
            });

            builder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<TestStartup>();
                //webBuilder.UseTestServer();
            });

            return builder;
        }
    }
}
