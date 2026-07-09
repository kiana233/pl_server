namespace PlServer.Core;

public sealed class AccountRecord
{
    public AccountRecord(
        AccountId accountId,
        AccountName accountName,
        AccountStatus status,
        DateTimeOffset createdAtUtc,
        DateTimeOffset? updatedAtUtc = null,
        string? notes = null)
    {
        AccountId = accountId;
        AccountName = accountName;
        Status = status;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        Notes = notes;
    }

    public AccountId AccountId { get; }

    public AccountName AccountName { get; }

    public AccountStatus Status { get; }

    public DateTimeOffset CreatedAtUtc { get; }

    public DateTimeOffset? UpdatedAtUtc { get; }

    public string? Notes { get; }
}
