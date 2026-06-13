using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Aegis.Template.Api.Pro.Auth;

public static class AegisJwtAuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddAegisJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwt = configuration.GetSection(AegisJwtOptions.SectionName).Get<AegisJwtOptions>() ?? new AegisJwtOptions();

        services.Configure<AegisJwtOptions>(configuration.GetSection(AegisJwtOptions.SectionName));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = jwt.RequireHttpsMetadata;
                options.MapInboundClaims = false;
                options.TokenValidationParameters = jwt.IsConfigured
                    ? CreateConfiguredValidationParameters(jwt)
                    : CreateRejectAllValidationParameters();
            });

        services.AddAuthorization();

        return services;
    }

    private static TokenValidationParameters CreateConfiguredValidationParameters(AegisJwtOptions jwt)
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey!)),
            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    }

    private static TokenValidationParameters CreateRejectAllValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://aegis.local/not-configured",
            ValidateAudience = true,
            ValidAudience = "aegis-not-configured",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(RandomNumberGenerator.GetBytes(64)),
            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ClockSkew = TimeSpan.Zero
        };
    }
}
