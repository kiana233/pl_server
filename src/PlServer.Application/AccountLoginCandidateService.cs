using PlServer.Core;

namespace PlServer.Application;

public sealed class AccountLoginCandidateService
{
    private readonly IAccountRepository accountRepository;
    private readonly IAccountCredentialVerifier credentialVerifier;

    public AccountLoginCandidateService(
        IAccountRepository accountRepository,
        IAccountCredentialVerifier credentialVerifier)
    {
        this.accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        this.credentialVerifier = credentialVerifier ?? throw new ArgumentNullException(nameof(credentialVerifier));
    }

    public async ValueTask<AccountAuthenticationResult> AuthenticateCandidateAsync(
        AccountAuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        cancellationToken.ThrowIfCancellationRequested();

        var lookupResult = await accountRepository
            .FindByNameAsync(request.AccountName, cancellationToken)
            .ConfigureAwait(false);

        if (!lookupResult.Found || lookupResult.Account is null)
        {
            return AccountAuthenticationResult.AccountNotFound(request.AccountName);
        }

        var account = lookupResult.Account;

        if (account.Status == AccountStatus.Disabled)
        {
            return AccountAuthenticationResult.AccountDisabled(account);
        }

        var verified = await credentialVerifier
            .VerifyAsync(account, request.SuppliedSecret, cancellationToken)
            .ConfigureAwait(false);

        return verified
            ? AccountAuthenticationResult.CandidateAuthenticated(account)
            : AccountAuthenticationResult.InvalidCredentials(account);
    }
}
