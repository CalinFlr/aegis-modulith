using System.Text;

namespace Aegis.Template.Api.Pro.Auth;

public sealed class AegisJwtOptions
{
    public const string SectionName = "Authentication:Jwt";
    public const int MinimumSigningKeyBytes = 32;

    public string? Issuer { get; init; }

    public string? Audience { get; init; }

    public string? SigningKey { get; init; }

    public bool RequireHttpsMetadata { get; init; } = true;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Issuer) &&
        !string.IsNullOrWhiteSpace(Audience) &&
        HasSufficientSigningKey;

    public bool HasSufficientSigningKey =>
        Encoding.UTF8.GetByteCount(SigningKey ?? string.Empty) >= MinimumSigningKeyBytes;
}
