using PlServer.Replay;

namespace PlServer.Replay.Tests;

public sealed class ReplayFrameworkTests
{
    [Fact]
    public void Importer_reads_one_JSONL_trace_event()
    {
        var result = new ReplayJsonLinesImporter().ImportString(CreateJsonLine());

        Assert.False(result.HasErrors);
        var step = Assert.Single(result.ReplayCase.Steps);
        Assert.Equal(ReplayDirection.C2S, step.Direction);
        Assert.True(step.ExpectedAc.HasValue);
        Assert.True(step.ExpectedSubAc.HasValue);
        Assert.Equal((byte)0x63, step.ExpectedAc.Value);
        Assert.Equal((byte)0x04, step.ExpectedSubAc.Value);
        Assert.Equal("LoginRequestCandidate", step.ProtocolName);
    }

    [Fact]
    public void Importer_skips_empty_lines()
    {
        var result = new ReplayJsonLinesImporter().ImportString($"{Environment.NewLine}{CreateJsonLine()}{Environment.NewLine}");

        Assert.False(result.HasErrors);
        Assert.Single(result.ReplayCase.Steps);
    }

    [Fact]
    public void Importer_records_error_for_invalid_JSON()
    {
        var result = new ReplayJsonLinesImporter().ImportString("{not json");

        Assert.Empty(result.ReplayCase.Steps);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void Importer_records_error_when_rawHex_and_decodedHex_are_missing()
    {
        var result = new ReplayJsonLinesImporter().ImportString("""{"direction":"C2S"}""");

        Assert.Empty(result.ReplayCase.Steps);
        Assert.Contains(result.Errors, error => error.Message.Contains("rawHex and decodedHex"));
    }

    [Fact]
    public void Importer_parses_spaced_hex_into_bytes()
    {
        var result = new ReplayJsonLinesImporter().ImportString(CreateJsonLine());

        var step = Assert.Single(result.ReplayCase.Steps);
        Assert.Equal(new byte[] { 0xF4, 0x44, 0x02, 0x00, 0x63, 0x04 }, step.DecodedBytes);
    }

    [Fact]
    public void Importer_preserves_direction_C2S_S2C_and_Internal()
    {
        var jsonLines = string.Join(
            Environment.NewLine,
            CreateJsonLine(direction: "C2S"),
            CreateJsonLine(direction: "S2C"),
            CreateJsonLine(direction: "Internal"));

        var result = new ReplayJsonLinesImporter().ImportString(jsonLines);

        Assert.Equal(
            new[] { ReplayDirection.C2S, ReplayDirection.S2C, ReplayDirection.Internal },
            result.ReplayCase.Steps.Select(step => step.Direction).ToArray());
    }

    [Fact]
    public void Runner_decodes_valid_frame_and_reports_AC_SubAC()
    {
        var importResult = new ReplayJsonLinesImporter().ImportString(CreateJsonLine());

        var runResult = new ReplayRunner().Run(importResult);

        var packetResult = Assert.Single(runResult.Results);
        Assert.True(packetResult.IsValid);
        Assert.True(packetResult.Ac.HasValue);
        Assert.True(packetResult.SubAc.HasValue);
        Assert.Equal((byte)0x63, packetResult.Ac.Value);
        Assert.Equal((byte)0x04, packetResult.SubAc.Value);
    }

    [Fact]
    public void Runner_detects_expected_AC_match()
    {
        var importResult = new ReplayJsonLinesImporter().ImportString(CreateJsonLine(ac: 0x63));

        var packetResult = Assert.Single(new ReplayRunner().Run(importResult).Results);

        Assert.True(packetResult.MatchedExpectedAc);
    }

    [Fact]
    public void Runner_detects_expected_AC_mismatch()
    {
        var importResult = new ReplayJsonLinesImporter().ImportString(CreateJsonLine(ac: 0x06));

        var packetResult = Assert.Single(new ReplayRunner().Run(importResult).Results);

        Assert.False(packetResult.MatchedExpectedAc);
    }

    [Fact]
    public void Runner_preserves_PacketCodec_validation_errors()
    {
        var importResult = new ReplayJsonLinesImporter().ImportString(CreateJsonLine(decodedHex: "F4 44 04 00 63 04"));

        var packetResult = Assert.Single(new ReplayRunner().Run(importResult).Results);

        Assert.False(packetResult.IsValid);
        Assert.NotEmpty(packetResult.ValidationErrors);
    }

    [Fact]
    public void Runner_can_process_multiple_steps()
    {
        var jsonLines = string.Join(Environment.NewLine, CreateJsonLine(), CreateJsonLine(direction: "S2C"));

        var result = new ReplayRunner().Run(new ReplayJsonLinesImporter().ImportString(jsonLines));

        Assert.Equal(2, result.Results.Count);
    }

    [Fact]
    public void ReplayRunResult_summarizes_total_passed_and_failed_counts()
    {
        var jsonLines = string.Join(
            Environment.NewLine,
            CreateJsonLine(ac: 0x63),
            CreateJsonLine(ac: 0x06));

        var result = new ReplayRunner().Run(new ReplayJsonLinesImporter().ImportString(jsonLines));

        Assert.Equal(2, result.TotalSteps);
        Assert.Equal(1, result.PassedSteps);
        Assert.Equal(1, result.FailedSteps);
    }

    [Fact]
    public void Synthetic_replay_is_not_marked_as_trace_client_confirmed()
    {
        var importResult = new ReplayJsonLinesImporter().ImportString(CreateJsonLine(
            sourceLabel: "reference:muayad",
            status: "pending-target-client-trace"));

        var step = Assert.Single(importResult.ReplayCase.Steps);

        Assert.NotEqual("trace:client", step.SourceLabel);
        Assert.NotEqual("confirmed", step.Status);
    }

    [Fact]
    public void Replay_does_not_require_TCP_Host_or_SessionStateMachine()
    {
        var importResult = new ReplayJsonLinesImporter().ImportString(CreateJsonLine());

        var exception = Record.Exception(() => new ReplayRunner().Run(importResult));

        Assert.Null(exception);
    }

    private static string CreateJsonLine(
        string direction = "C2S",
        byte ac = 0x63,
        byte subAc = 0x04,
        string decodedHex = "F4 44 02 00 63 04",
        string rawHex = "F4 44 02 00 63 04",
        string sourceLabel = "reference:muayad",
        string status = "pending-target-client-trace")
    {
        return $$"""
        {"timestamp":"2026-07-06T00:00:00Z","direction":"{{direction}}","connectionId":"conn-1","sessionState":"none","rawHex":"{{rawHex}}","decodedHex":"{{decodedHex}}","ac":{{ac}},"subAc":{{subAc}},"protocolName":"LoginRequestCandidate","behavior":"synthetic replay candidate","sourceLabel":"{{sourceLabel}}","status":"{{status}}","validationErrors":[]}
        """;
    }
}
