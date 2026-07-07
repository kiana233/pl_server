namespace PlServer.Session;

public sealed record SessionContextSnapshot(
    string ConnectionId,
    string? AccountName,
    string? CharacterName,
    SessionState State,
    SessionPacketKind? LastPacketKind,
    byte? LastAc,
    byte? LastSubAc);
