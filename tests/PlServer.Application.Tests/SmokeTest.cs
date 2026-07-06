using PlServer.Application;

namespace PlServer.Application.Tests;

public sealed class SmokeTest
{
    [Fact]
    public void Application_project_is_discoverable()
    {
        Assert.Equal("PlServer.Application", typeof(ApplicationAssemblyMarker).Assembly.GetName().Name);
    }
}
