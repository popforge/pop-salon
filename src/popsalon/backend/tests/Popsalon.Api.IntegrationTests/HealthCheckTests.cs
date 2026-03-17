using System.Net;
using FluentAssertions;
using Xunit;

namespace Popsalon.Api.IntegrationTests;

/// <summary>
/// Tests d'intégration — démarrage de l'API et santé de base.
/// Utilise une base de données en mémoire, aucun Docker requis.
/// </summary>
public class HealthCheckTests(TestWebApplicationFactory factory)
    : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Health_ResponseBody_IsHealthy()
    {
        var response = await _client.GetAsync("/health");
        var body = await response.Content.ReadAsStringAsync();

        body.Should().Be("Healthy");
    }
}
