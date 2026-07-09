using PlServer.Core;

namespace PlServer.Application;

public interface IAccountCredentialVerifier
{
    ValueTask<bool> VerifyAsync(
        AccountRecord account,
        ReadOnlyMemory<byte> suppliedSecret,
        CancellationToken cancellationToken = default);
}
