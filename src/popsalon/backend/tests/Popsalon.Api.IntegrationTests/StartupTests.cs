using System.Net;
using FluentAssertions;
using Xunit;

namespace Popsalon.Api.IntegrationTests;

/// <summary>
/// Valide que l'API démarre correctement et que les endpoints OData de base répondent.
/// </summary>
public class StartupTests(TestWebApplicationFactory factory)
    : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Api_Starts_WithoutException()
    {
        // Si l'app plante au démarrage, CreateClient() lève une exception.
        // Ce test passe uniquement si le démarrage s'est fait sans erreur.
        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Appointments_Endpoint_IsReachable()
    {
        var response = await _client.GetAsync("/api/v1/appointments");

        // 200 OK ou 204 No Content — pas une 500
        ((int)response.StatusCode).Should().BeLessThan(500);
    }

    [Fact]
    public async Task Customers_Endpoint_IsReachable()
    {
        var response = await _client.GetAsync("/api/v1/customers");

        ((int)response.StatusCode).Should().BeLessThan(500);
    }

    [Fact]
    public async Task Swagger_Endpoint_IsReachable()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
