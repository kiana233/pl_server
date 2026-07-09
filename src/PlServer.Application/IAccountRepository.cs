using PlServer.Core;

namespace PlServer.Application;

public interface IAccountRepository
{
    Task<AccountLookupResult> FindByNameAsync(
        AccountName accountName,
        CancellationToken cancellationToken = default);

    Task<AccountLookupResult> GetByIdAsync(
        AccountId accountId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        AccountName accountName,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        AccountRecord account,
        CancellationToken cancellationToken = default);
}
