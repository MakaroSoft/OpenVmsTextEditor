namespace OpenVmsTextEditor.Infrastructure;

public record JwtIssueOptions
{
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public string KeyId { get; init; } = "kid-1";
}