using PlServer.Application;
using PlServer.Core;

namespace PlServer.Persistence;

public sealed class InMemoryAccountRepository : IAccountRepository
{
    private readonly object syncRoot = new();
    private readonly Dictionary<AccountId, AccountRecord> accountsById = new();
    private readonly Dictionary<string, AccountRecord> accountsByName;
    private readonly bool useCaseInsensitiveAccountNames;

    public InMemoryAccountRepository(InMemoryAccountRepositoryOptions? options = null)
    {
        options ??= new InMemoryAccountRepositoryOptions();
        useCaseInsensitiveAccountNames = options.UseCaseInsensitiveAccountNames;
        accountsByName = new Dictionary<string, AccountRecord>(
            useCaseInsensitiveAccountNames ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

        foreach (var account in options.SeedAccounts)
        {
            AddAccount(account);
        }
    }

    public Task<AccountLookupResult> FindByNameAsync(
        AccountName accountName,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (syncRoot)
        {
            return Task.FromResult(accountsByName.TryGetValue(NameKey(accountName), out var account)
                ? AccountLookupResult.FoundAccount(account)
                : AccountLookupResult.NotFound());
        }
    }

    public Task<AccountLookupResult> GetByIdAsync(
        AccountId accountId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (syncRoot)
        {
            return Task.FromResult(accountsById.TryGetValue(accountId, out var account)
                ? AccountLookupResult.FoundAccount(account)
                : AccountLookupResult.NotFound());
        }
    }

    public async Task<bool> ExistsByNameAsync(
        AccountName accountName,
        CancellationToken cancellationToken = default)
    {
        var lookupResult = await FindByNameAsync(accountName, cancellationToken).ConfigureAwait(false);
        return lookupResult.Found;
    }

    public Task AddAsync(
        AccountRecord account,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(account);
        cancellationToken.ThrowIfCancellationRequested();

        lock (syncRoot)
        {
            AddAccount(account);
        }

        return Task.CompletedTask;
    }

    private void AddAccount(AccountRecord account)
    {
        var nameKey = NameKey(account.AccountName);

        if (accountsById.ContainsKey(account.AccountId))
        {
            throw new InvalidOperationException("Duplicate account id.");
        }

        if (accountsByName.ContainsKey(nameKey))
        {
            throw new InvalidOperationException("Duplicate account name.");
        }

        accountsById.Add(account.AccountId, account);
        accountsByName.Add(nameKey, account);
    }

    private string NameKey(AccountName accountName)
    {
        return useCaseInsensitiveAccountNames ? accountName.NormalizedValue : accountName.Value;
    }
}
