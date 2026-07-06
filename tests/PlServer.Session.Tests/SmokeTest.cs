using PlServer.Session;

namespace PlServer.Session.Tests;

public sealed class SmokeTest
{
    [Fact]
    public void Session_project_is_discoverable()
    {
        Assert.Equal("PlServer.Session", typeof(SessionAssemblyMarker).Assembly.GetName().Name);
    }
}
