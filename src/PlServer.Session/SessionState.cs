namespace PlServer.Session;

public enum SessionState
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
