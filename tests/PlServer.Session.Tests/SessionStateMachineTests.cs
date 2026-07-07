using PlServer.Diagnostics;
using PlServer.Protocol;
using PlServer.Session;

namespace PlServer.Session.Tests;

public sealed class SessionStateMachineTests
{
    private readonly SessionPacketClassifier _classifier = new();

    [Fact]
    public void Initial_state_is_connected()
    {
        var stateMachine = new SessionStateMachine();

        Assert.Equal(SessionState.Connected, stateMachine.CurrentState);
    }

    [Fact]
    public void Ac0_classifies_as_handshake_candidate()
    {
        var classification = _classifier.Classify(0x00, 0x01);

        AssertClassification(
            classification,
            SessionPacketKind.HandshakeCandidate,
            ProtocolTraceSourceLabel.ReferenceMuayad);
    }

    [Fact]
    public void Ac63_subac4_classifies_as_login_request_candidate()
    {
        var classification = _classifier.Classify(0x63, 0x04);

        AssertClassification(
            classification,
            SessionPacketKind.LoginRequestCandidate,
            ProtocolTraceSourceLabel.ReferenceMuayad);
    }

    [Fact]
    public void Ac63_subac2_classifies_as_character_select_candidate()
    {
        var classification = _classifier.Classify(0x63, 0x02);

        AssertClassification(
            classification,
            SessionPacketKind.CharacterSelectCandidate,
            ProtocolTraceSourceLabel.ReferenceMuayad);
    }

    [Fact]
    public void Ac06_subac1_classifies_as_movement_candidate()
    {
        var classification = _classifier.Classify(0x06, 0x01);

        AssertClassification(
            classification,
            SessionPacketKind.MovementCandidate,
            ProtocolTraceSourceLabel.ReferenceMuayad);
    }

    [Fact]
    public void Invalid_packet_classifies_as_invalid_packet()
    {
        var validationError = new PacketValidationError(
            PacketValidationErrorCode.InvalidHeader,
            "Synthetic invalid packet for session classifier test.");
        var decodeResult = new PacketDecodeResult(
            Array.Empty<byte>(),
            Array.Empty<byte>(),
            null,
            0,
            Array.Empty<byte>(),
            null,
            null,
            new[] { validationError });

        var classification = _classifier.Classify(decodeResult);

        AssertClassification(
            classification,
            SessionPacketKind.InvalidPacket,
            ProtocolTraceSourceLabel.Assumption);
        Assert.Single(classification.ValidationErrors!);
    }

    [Fact]
    public void Unknown_ac_classifies_as_unknown()
    {
        var classification = _classifier.Classify(0xff, 0x01);

        AssertClassification(
            classification,
            SessionPacketKind.Unknown,
            ProtocolTraceSourceLabel.Assumption);
    }

    [Fact]
    public void Connected_handshake_candidate_transitions_to_handshake_done()
    {
        var stateMachine = new SessionStateMachine();

        var result = stateMachine.Apply(_classifier.Classify(0x00, 0x00));

        Assert.True(result.Allowed);
        Assert.Equal(SessionState.HandshakeDone, result.CurrentState);
        Assert.Equal(SessionState.HandshakeDone, stateMachine.CurrentState);
    }

    [Fact]
    public void Handshake_done_login_request_candidate_transitions_to_login_pending()
    {
        var stateMachine = AdvanceToHandshakeDone();

        var result = stateMachine.Apply(_classifier.Classify(0x63, 0x04));

        Assert.True(result.Allowed);
        Assert.Equal(SessionState.LoginPending, result.CurrentState);
    }

    [Fact]
    public void Login_pending_login_accepted_candidate_transitions_to_login_accepted()
    {
        var stateMachine = AdvanceToLoginPending();

        var result = stateMachine.Apply(Explicit(SessionPacketKind.LoginAcceptedCandidate));

        Assert.True(result.Allowed);
        Assert.Equal(SessionState.LoginAccepted, result.CurrentState);
    }

    [Fact]
    public void Login_accepted_character_list_candidate_transitions_to_character_list_shown()
    {
        var stateMachine = AdvanceToLoginAccepted();

        var result = stateMachine.Apply(_classifier.Classify(0x63, 0x01));

        Assert.True(result.Allowed);
        Assert.Equal(SessionState.CharacterListShown, result.CurrentState);
    }

    [Fact]
    public void Character_list_shown_character_select_candidate_transitions_to_character_selected()
    {
        var stateMachine = AdvanceToCharacterListShown();

        var result = stateMachine.Apply(_classifier.Classify(0x63, 0x02));

        Assert.True(result.Allowed);
        Assert.Equal(SessionState.CharacterSelected, result.CurrentState);
    }

    [Fact]
    public void Character_selected_enter_map_candidate_transitions_to_entering_map()
    {
        var stateMachine = AdvanceToCharacterSelected();

        var result = stateMachine.Apply(_classifier.Classify(0x12, 0x00));

        Assert.True(result.Allowed);
        Assert.Equal(SessionState.EnteringMap, result.CurrentState);
    }

    [Fact]
    public void Entering_map_in_map_ready_candidate_transitions_to_in_map()
    {
        var stateMachine = AdvanceToEnteringMap();

        var result = stateMachine.Apply(Explicit(SessionPacketKind.InMapReadyCandidate));

        Assert.True(result.Allowed);
        Assert.Equal(SessionState.InMap, result.CurrentState);
    }

    [Fact]
    public void In_map_movement_candidate_remains_in_map()
    {
        var stateMachine = AdvanceToInMap();

        var result = stateMachine.Apply(_classifier.Classify(0x06, 0x01));

        Assert.True(result.Allowed);
        Assert.Equal(SessionState.InMap, result.CurrentState);
        Assert.False(result.StateChanged);
    }

    [Fact]
    public void Movement_before_in_map_is_rejected()
    {
        var stateMachine = new SessionStateMachine();

        var result = stateMachine.Apply(_classifier.Classify(0x06, 0x01));

        Assert.False(result.Allowed);
        Assert.Equal(SessionState.Connected, result.CurrentState);
        Assert.Contains(result.Errors, error => error.Code == SessionTransitionErrorCode.MovementBeforeInMap);
    }

    [Fact]
    public void Unknown_packet_does_not_change_state()
    {
        var stateMachine = new SessionStateMachine();

        var result = stateMachine.Apply(_classifier.Classify(0xff, 0x01));

        Assert.True(result.Allowed);
        Assert.Equal(SessionState.Connected, result.CurrentState);
        Assert.Contains(result.Errors, error => error.Code == SessionTransitionErrorCode.UnknownPacket);
    }

    [Fact]
    public void Invalid_packet_does_not_change_state_and_records_error()
    {
        var stateMachine = new SessionStateMachine();

        var result = stateMachine.Apply(Explicit(SessionPacketKind.InvalidPacket));

        Assert.False(result.Allowed);
        Assert.Equal(SessionState.Connected, result.CurrentState);
        Assert.Contains(result.Errors, error => error.Code == SessionTransitionErrorCode.InvalidPacket);
    }

    [Fact]
    public void Disconnect_candidate_transitions_to_disconnected()
    {
        var stateMachine = AdvanceToInMap();

        var result = stateMachine.Apply(Explicit(SessionPacketKind.DisconnectCandidate));

        Assert.True(result.Allowed);
        Assert.Equal(SessionState.Disconnected, result.CurrentState);
    }

    [Fact]
    public void State_guard_rejects_movement_candidate_in_connected_state()
    {
        var guard = new SessionStateGuard();

        var result = guard.CanAccept(SessionState.Connected, SessionPacketKind.MovementCandidate);

        Assert.True(result.Rejected);
        Assert.Equal(SessionState.Connected, result.CurrentState);
        Assert.Equal(SessionPacketKind.MovementCandidate, result.PacketKind);
    }

    [Fact]
    public void State_guard_allows_movement_candidate_in_in_map_state()
    {
        var guard = new SessionStateGuard();

        var result = guard.CanAccept(SessionState.InMap, SessionPacketKind.MovementCandidate);

        Assert.True(result.Allowed);
        Assert.False(result.Rejected);
        Assert.Equal(SessionState.InMap, result.CurrentState);
        Assert.Equal(SessionPacketKind.MovementCandidate, result.PacketKind);
    }

    private SessionStateMachine AdvanceToHandshakeDone()
    {
        var stateMachine = new SessionStateMachine();
        stateMachine.Apply(_classifier.Classify(0x00, 0x00));
        return stateMachine;
    }

    private SessionStateMachine AdvanceToLoginPending()
    {
        var stateMachine = AdvanceToHandshakeDone();
        stateMachine.Apply(_classifier.Classify(0x63, 0x04));
        return stateMachine;
    }

    private SessionStateMachine AdvanceToLoginAccepted()
    {
        var stateMachine = AdvanceToLoginPending();
        stateMachine.Apply(Explicit(SessionPacketKind.LoginAcceptedCandidate));
        return stateMachine;
    }

    private SessionStateMachine AdvanceToCharacterListShown()
    {
        var stateMachine = AdvanceToLoginAccepted();
        stateMachine.Apply(_classifier.Classify(0x63, 0x01));
        return stateMachine;
    }

    private SessionStateMachine AdvanceToCharacterSelected()
    {
        var stateMachine = AdvanceToCharacterListShown();
        stateMachine.Apply(_classifier.Classify(0x63, 0x02));
        return stateMachine;
    }

    private SessionStateMachine AdvanceToEnteringMap()
    {
        var stateMachine = AdvanceToCharacterSelected();
        stateMachine.Apply(_classifier.Classify(0x12, 0x00));
        return stateMachine;
    }

    private SessionStateMachine AdvanceToInMap()
    {
        var stateMachine = AdvanceToEnteringMap();
        stateMachine.Apply(Explicit(SessionPacketKind.InMapReadyCandidate));
        return stateMachine;
    }

    private static SessionPacketClassification Explicit(SessionPacketKind kind)
    {
        return SessionPacketClassifier.Explicit(kind, "Explicit synthetic session event for state-machine test.");
    }

    private static void AssertClassification(
        SessionPacketClassification classification,
        SessionPacketKind expectedKind,
        ProtocolTraceSourceLabel expectedSource)
    {
        Assert.Equal(expectedKind, classification.Kind);
        Assert.Equal(expectedSource, classification.SourceLabel);
        Assert.Equal(ProtocolTraceStatus.PendingTargetClientTrace, classification.Status);
    }
}
