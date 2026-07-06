using PlServer.Resources;

namespace PlServer.Resources.Tests;

public sealed class SmokeTest
{
    [Fact]
    public void Resources_project_is_discoverable()
    {
        Assert.Equal("PlServer.Resources", typeof(ResourcesAssemblyMarker).Assembly.GetName().Name);
    }
}
