namespace Aegis.Template.Api.Pro.Caching;

public sealed class CacheKeyFactory
{
    public string ForModule(string module, string value) => $"{module.Trim().ToLowerInvariant()}:{value.Trim().ToLowerInvariant()}";
}
