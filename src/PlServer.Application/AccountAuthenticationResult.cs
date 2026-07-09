using PlServer.Core;

namespace PlServer.Application;

public sealed class AccountAuthenticationResult
{
    private AccountAuthenticationResult(
        AccountAuthenticationStatus status,
        AccountId? accountId,
        AccountName? accountName,
        IReadOnlyList<string>? notes = null)
    {
        Status = status;
        AccountId = accountId;
        AccountName = accountName;
        Notes = notes ?? Array.Empty<string>();
        ResponsePackets = Array.Empty<byte[]>();
    }

    public AccountAuthenticationStatus Status { get; }

    public AccountId? AccountId { get; }

    public AccountName? AccountName { get; }

    public IReadOnlyList<string> Notes { get; }

    public IReadOnlyList<byte[]> ResponsePackets { get; }

    public bool HasResponsePackets => ResponsePackets.Count > 0;

    public bool IsProtocolLoginSuccess => false;

    public static AccountAuthenticationResult AccountNotFound(AccountName accountName)
    {
        return new AccountAuthenticationResult(
            AccountAuthenticationStatus.AccountNotFound,
            null,
            accountName,
            new[] { "account lookup did not find a candidate record" });
    }

    public static AccountAuthenticationResult AccountDisabled(AccountRecord account)
    {
        ArgumentNullException.ThrowIfNull(account);

        return new AccountAuthenticationResult(
            AccountAuthenticationStatus.AccountDisabled,
            account.AccountId,
            account.AccountName,
            new[] { "account record is disabled; no protocol response generated" });
    }

    public static AccountAuthenticationResult InvalidCredentials(AccountRecord account)
    {
        ArgumentNullException.ThrowIfNull(account);

        return new AccountAuthenticationResult(
            AccountAuthenticationStatus.InvalidCredentials,
            account.AccountId,
            account.AccountName,
            new[] { "candidate credential verifier rejected the supplied secret" });
    }

    public static AccountAuthenticationResult CandidateAuthenticated(AccountRecord account)
    {
        ArgumentNullException.ThrowIfNull(account);

        return new AccountAuthenticationResult(
            AccountAuthenticationStatus.CandidateAuthenticated,
            account.AccountId,
            account.AccountName,
            new[]
            {
                "candidate account authentication only",
                "not a protocol login success",
                "no login response packet generated",
                "pending target-client trace"
            });
    }
}
