using PlServer.Core;

namespace PlServer.Persistence.Tests;

public sealed class InMemoryAccountRepositoryTests
{
    [Fact]
    public async Task Seeded_account_can_be_found_by_name()
    {
        var account = CreateAccount("seeded-account");
        var repository = CreateRepository(account);

        var result = await repository.FindByNameAsync(new AccountName("seeded-account"));

        Assert.True(result.Found);
        Assert.Same(account, result.Account);
    }

    [Fact]
    public async Task Name_lookup_is_case_insensitive_by_default()
    {
        var account = CreateAccount("CaseCandidate");
        var repository = CreateRepository(account);

        var result = await repository.FindByNameAsync(new AccountName("casecandidate"));

        Assert.True(result.Found);
        Assert.Same(account, result.Account);
    }

    [Fact]
    public void Duplicate_account_name_is_rejected()
    {
        var first = CreateAccount("duplicate", "id-1");
        var second = CreateAccount("DUPLICATE", "id-2");

        Assert.Throws<InvalidOperationException>(() => CreateRepository(first, second));
    }

    [Fact]
    public async Task Missing_account_returns_not_found()
    {
        var repository = CreateRepository(CreateAccount("existing"));

        var result = await repository.FindByNameAsync(new AccountName("missing"));

        Assert.False(result.Found);
        Assert.Null(result.Account);
    }

    [Fact]
    public async Task Account_can_be_found_by_id()
    {
        var account = CreateAccount("by-id", "id-by-id");
        var repository = CreateRepository(account);

        var result = await repository.GetByIdAsync(new AccountId("id-by-id"));

        Assert.True(result.Found);
        Assert.Same(account, result.Account);
    }

    [Fact]
    public async Task Add_async_rejects_duplicate_name()
    {
        var repository = CreateRepository(CreateAccount("existing", "id-existing"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            repository.AddAsync(CreateAccount("EXISTING", "id-new")));
    }

    [Fact]
    public void Repository_type_is_in_memory_only()
    {
        Assert.DoesNotContain(
            typeof(InMemoryAccountRepository).GetProperties().Select(property => property.Name),
            name => name.Contains("connection", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Account_record_does_not_store_real_credential_material()
    {
        var propertyNames = typeof(AccountRecord)
            .GetProperties()
            .Select(property => property.Name)
            .ToArray();

        Assert.DoesNotContain(propertyNames, name => name.Contains("password", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(propertyNames, name => name.Contains("token", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(propertyNames, name => name.Contains("cookie", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(propertyNames, name => name.Contains("sessionkey", StringComparison.OrdinalIgnoreCase));
    }

    private static InMemoryAccountRepository CreateRepository(params AccountRecord[] accounts)
    {
        return new InMemoryAccountRepository(new InMemoryAccountRepositoryOptions
        {
            SeedAccounts = accounts
        });
    }

    private static AccountRecord CreateAccount(string name, string? id = null)
    {
        return new AccountRecord(
            new AccountId(id ?? $"id-{name}"),
            new AccountName(name),
            AccountStatus.Active,
            DateTimeOffset.UnixEpoch,
            notes: "fake test account only");
    }
}
