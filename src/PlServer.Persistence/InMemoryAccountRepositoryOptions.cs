using PlServer.Core;

namespace PlServer.Persistence;

public sealed class InMemoryAccountRepositoryOptions
{
    public IReadOnlyCollection<AccountRecord> SeedAccounts { get; init; } = Array.Empty<AccountRecord>();

    public bool UseCaseInsensitiveAccountNames { get; init; } = true;
}
