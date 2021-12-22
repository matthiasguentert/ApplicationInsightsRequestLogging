using Xunit;
using ApplicationInsightsRequestLoggingTests.Utils;
using System.Net;
using FluentAssertions;

namespace ApplicationInsightsRequestLoggingTests
{
    public class BodyLoggerMiddlewareIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public BodyLoggerMiddlewareIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async void BodyLoggerMiddleware_Should_Send_Data()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var requestBody = new
            {
                Name = "Bommelmaier",
                Password = "Abracadabra!",
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJsb2dnZWRJbkFzIjoiYWRtaW4iLCJpYXQiOjE0MjI3Nzk2Mzh9.gzSraSYS8EXBxLN_oWnFSRgCzcmJmMjLiuyu5CSpyHI"
            }; 
            var response = await client.PostAsync("/", new JsonContent(requestBody));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
