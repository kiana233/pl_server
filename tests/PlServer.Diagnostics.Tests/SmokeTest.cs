using PlServer.Diagnostics;

namespace PlServer.Diagnostics.Tests;

public sealed class SmokeTest
{
    [Fact]
    public void Diagnostics_project_is_discoverable()
    {
        Assert.Equal("PlServer.Diagnostics", typeof(DiagnosticsAssemblyMarker).Assembly.GetName().Name);
    }
}
