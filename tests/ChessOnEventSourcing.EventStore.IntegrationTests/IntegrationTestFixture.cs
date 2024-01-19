using Microsoft.Extensions.Configuration;

namespace ChessOnEventSourcing.EventStore.IntegrationTests;

public sealed class IntegrationTestFixture
{
    public IConfigurationRoot Configuration { get; }

    public IntegrationTestFixture()
    {
        var environmentName = Environment.GetEnvironmentVariable("DOTNETCORE_ENVIRONMENT");
        Configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json", optional: false)
           .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
           .AddEnvironmentVariables()
           .Build();
    }
}
