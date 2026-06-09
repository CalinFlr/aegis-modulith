namespace Aegis.Template.Api.Pro.Auth;

public sealed class AegisJwtOptions
{
    public const string SectionName = "Authentication:Jwt";

    public string? Issuer { get; init; }

    public string? Audience { get; init; }

    public string? SigningKey { get; init; }

    public bool RequireHttpsMetadata { get; init; } = true;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Issuer) &&
        !string.IsNullOrWhiteSpace(Audience) &&
        !string.IsNullOrWhiteSpace(SigningKey);
}
