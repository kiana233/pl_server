using PlServer.Network;

namespace PlServer.Network.Tests;

public sealed class SmokeTest
{
    [Fact]
    public void Network_project_is_discoverable()
    {
        Assert.Equal("PlServer.Network", typeof(NetworkAssemblyMarker).Assembly.GetName().Name);
    }
}
