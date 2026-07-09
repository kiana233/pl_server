using PlServer.Core;

namespace PlServer.Application;

public sealed class AccountLookupResult
{
    private AccountLookupResult(bool found, AccountRecord? account)
    {
        Found = found;
        Account = account;
    }

    public bool Found { get; }

    public AccountRecord? Account { get; }

    public static AccountLookupResult FoundAccount(AccountRecord account)
    {
        ArgumentNullException.ThrowIfNull(account);
        return new AccountLookupResult(true, account);
    }

    public static AccountLookupResult NotFound()
    {
        return new AccountLookupResult(false, null);
    }
}
