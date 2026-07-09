using PlServer.Core;
using PlServer.LegacyProtocol;
using PlServer.Protocol;
using PlServer.Session;

namespace PlServer.Application.Tests;

public sealed class AccountLoginCandidateServiceTests
{
    [Fact]
    public async Task Missing_account_returns_account_not_found()
    {
        var service = new AccountLoginCandidateService(
            new FakeAccountRepository(null),
            new FixedCredentialVerifier(true));

        var result = await service.AuthenticateCandidateAsync(CreateRequest("missing"));

        Assert.Equal(AccountAuthenticationStatus.AccountNotFound, result.Status);
        Assert.False(result.IsProtocolLoginSuccess);
        Assert.False(result.HasResponsePackets);
    }

    [Fact]
    public async Task Disabled_account_returns_account_disabled_without_verifying_secret()
    {
        var account = CreateAccount("disabled", AccountStatus.Disabled);
        var verifier = new FixedCredentialVerifier(true);
        var service = new AccountLoginCandidateService(new FakeAccountRepository(account), verifier);

        var result = await service.AuthenticateCandidateAsync(CreateRequest("disabled"));

        Assert.Equal(AccountAuthenticationStatus.AccountDisabled, result.Status);
        Assert.Equal(0, verifier.VerifyCount);
        Assert.False(result.IsProtocolLoginSuccess);
        Assert.Empty(result.ResponsePackets);
    }

    [Fact]
    public async Task Rejected_fake_verifier_returns_invalid_credentials()
    {
        var account = CreateAccount("candidate", AccountStatus.Active);
        var service = new AccountLoginCandidateService(
            new FakeAccountRepository(account),
            new FixedCredentialVerifier(false));

        var result = await service.AuthenticateCandidateAsync(CreateRequest("candidate"));

        Assert.Equal(AccountAuthenticationStatus.InvalidCredentials, result.Status);
        Assert.False(result.IsProtocolLoginSuccess);
        Assert.False(result.HasResponsePackets);
    }

    [Fact]
    public async Task Accepted_fake_verifier_returns_candidate_authenticated_only()
    {
        var account = CreateAccount("candidate", AccountStatus.Active);
        var service = new AccountLoginCandidateService(
            new FakeAccountRepository(account),
            new FixedCredentialVerifier(true));

        var result = await service.AuthenticateCandidateAsync(CreateRequest("candidate"));

        Assert.Equal(AccountAuthenticationStatus.CandidateAuthenticated, result.Status);
        Assert.False(result.IsProtocolLoginSuccess);
        Assert.False(result.HasResponsePackets);
        Assert.Contains(result.Notes, note => note.Contains("not a protocol login success", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(result.Notes, note => note.Contains("pending target-client trace", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Authentication_result_does_not_expose_secret_or_token_fields()
    {
        var propertyNames = typeof(AccountAuthenticationResult)
            .GetProperties()
            .Select(property => property.Name)
            .ToArray();

        Assert.DoesNotContain(propertyNames, name => name.Contains("password", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(propertyNames, name => name.Contains("token", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(propertyNames, name => name.Contains("hash", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(propertyNames, name => name.Contains("salt", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Login_request_candidate_handler_keeps_payload_opaque_and_does_not_invoke_repository()
    {
        var router = new ActionRouter(
            LegacyProtocolContractCatalog.CreateDefaultRegistry(),
            CandidateActionHandlerCatalog.CreateDefaultRegistry());
        var request = new ActionRouteRequest(
            "candidate-connection",
            SessionState.HandshakeDone,
            ValidLoginCandidatePacket(),
            LegacyPacketDirection.C2S,
            null,
            null);

        var result = await router.RouteAsync(request);

        Assert.Equal(ActionRouteStatus.CandidateHandled, result.Status);
        Assert.Contains(result.HandlerNotes, note => note.Contains("payload kept opaque", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(result.HandlerNotes, note => note.Contains("account repository not invoked", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(result.HandlerNotes, note => note.Contains("pending target-client trace", StringComparison.OrdinalIgnoreCase));
        Assert.False(result.HandlerResult?.HasResponsePackets);
    }

    private static AccountAuthenticationRequest CreateRequest(string name)
    {
        return new AccountAuthenticationRequest(
            new AccountName(name),
            new byte[] { 0x01, 0x02, 0x03 });
    }

    private static AccountRecord CreateAccount(string name, AccountStatus status)
    {
        return new AccountRecord(
            new AccountId($"account-{name}"),
            new AccountName(name),
            status,
            DateTimeOffset.UnixEpoch);
    }

    private static PacketDecodeResult ValidLoginCandidatePacket()
    {
        return new PacketDecodeResult(
            new byte[] { 0x63 },
            new byte[] { 0x63 },
            null,
            0,
            Array.Empty<byte>(),
            0x63,
            0x04,
            Array.Empty<PacketValidationError>());
    }

    private sealed class FakeAccountRepository : IAccountRepository
    {
        private readonly AccountRecord? account;

        public FakeAccountRepository(AccountRecord? account)
        {
            this.account = account;
        }

        public Task<AccountLookupResult> FindByNameAsync(
            AccountName accountName,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(account is not null && account.AccountName == accountName
                ? AccountLookupResult.FoundAccount(account)
                : AccountLookupResult.NotFound());
        }

        public Task<AccountLookupResult> GetByIdAsync(
            AccountId accountId,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(account is not null && account.AccountId == accountId
                ? AccountLookupResult.FoundAccount(account)
                : AccountLookupResult.NotFound());
        }

        public Task<bool> ExistsByNameAsync(
            AccountName accountName,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(account is not null && account.AccountName == accountName);
        }

        public Task AddAsync(
            AccountRecord account,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("Fake repository is read-only for service tests.");
        }
    }

    private sealed class FixedCredentialVerifier : IAccountCredentialVerifier
    {
        private readonly bool result;

        public FixedCredentialVerifier(bool result)
        {
            this.result = result;
        }

        public int VerifyCount { get; private set; }

        public ValueTask<bool> VerifyAsync(
            AccountRecord account,
            ReadOnlyMemory<byte> suppliedSecret,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(account);
            cancellationToken.ThrowIfCancellationRequested();
            VerifyCount++;
            return ValueTask.FromResult(result);
        }
    }
}
