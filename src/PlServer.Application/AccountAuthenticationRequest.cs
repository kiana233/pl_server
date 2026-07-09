using PlServer.Core;

namespace PlServer.Application;

public sealed class AccountAuthenticationRequest
{
    public AccountAuthenticationRequest(AccountName accountName, ReadOnlyMemory<byte> suppliedSecret)
    {
        AccountName = accountName;
        SuppliedSecret = suppliedSecret;
    }

    public AccountName AccountName { get; }

    public ReadOnlyMemory<byte> SuppliedSecret { get; }
}
