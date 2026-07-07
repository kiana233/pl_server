namespace PlServer.LegacyProtocol;

public enum LegacyProtocolSessionRequirement
{
    Disconnected,
    Connected,
    HandshakeDone,
    LoginPending,
    LoginAccepted,
    CharacterListShown,
    CharacterSelected,
    EnteringMap,
    InMap,
    Rejected
}
