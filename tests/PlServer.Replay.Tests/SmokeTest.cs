using PlServer.Replay;

namespace PlServer.Replay.Tests;

public sealed class SmokeTest
{
    [Fact]
    public void Replay_project_is_discoverable()
    {
        Assert.Equal("PlServer.Replay", typeof(ReplayAssemblyMarker).Assembly.GetName().Name);
    }
}
