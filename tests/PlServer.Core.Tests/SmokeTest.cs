using PlServer.Core;

namespace PlServer.Core.Tests;

public sealed class SmokeTest
{
    [Fact]
    public void Core_project_is_discoverable()
    {
        Assert.Equal("PlServer.Core", typeof(CoreAssemblyMarker).Assembly.GetName().Name);
    }
}
