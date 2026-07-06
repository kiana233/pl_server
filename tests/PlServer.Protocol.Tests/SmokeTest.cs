using PlServer.Protocol;

namespace PlServer.Protocol.Tests;

public sealed class SmokeTest
{
    [Fact]
    public void Protocol_project_is_discoverable()
    {
        Assert.Equal("PlServer.Protocol", typeof(ProtocolAssemblyMarker).Assembly.GetName().Name);
    }
}
