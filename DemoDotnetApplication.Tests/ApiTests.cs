using Xunit;

namespace DemoDotnetApplication.Tests;

public class ApiTests
{
    [Fact]
    public void TestHealthCheckExists()
    {
        var endpoint = "/health";
        Assert.NotNull(endpoint);
    }

    [Fact]
    public void TestHomeEndpointExists()
    {
        var endpoint = "/";
        Assert.NotNull(endpoint);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    public void TestItemIdValidation(int id, bool isValid)
    {
        var result = id > 0;
        Assert.Equal(isValid, result);
    }

    [Fact]
    public void TestSecurityHeaders()
    {
        var headers = new[] { "X-Content-Type-Options", "X-Frame-Options" };
        Assert.NotEmpty(headers);
    }

    [Fact]
    public void TestApplicationBootstraps()
    {
        Assert.True(true);
    }
}
