using LetWeCook.Web;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LetWeCook.IntegrationTests.Fixtures;

public class IntegrationTestFixture : IAsyncLifetime
{
    public IServiceProvider ServiceProvider { get; private set; } = null!;
    private WebApplicationFactory<Program> _factory = null!;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Configure your test services here
                    // For example, you can replace the database context with an in-memory database
                });
            });

        ServiceProvider = _factory.Services;
    }

    public async Task DisposeAsync()
    {
        // Dispose of any resources here if needed
        _factory.Dispose();
    }
}