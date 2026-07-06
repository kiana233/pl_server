using PlServer.LegacyProtocol;

namespace PlServer.LegacyProtocol.Tests;

public sealed class SmokeTest
{
    [Fact]
    public void LegacyProtocol_project_is_discoverable()
    {
        Assert.Equal("PlServer.LegacyProtocol", typeof(LegacyProtocolAssemblyMarker).Assembly.GetName().Name);
    }
}
