using System.Collections.Concurrent;

namespace Aegis.Template.Api.Pro.Idempotency;

public sealed class InMemoryIdempotencyStore
{
    private readonly ConcurrentDictionary<string, DateTimeOffset> keys = new(StringComparer.Ordinal);

    public bool TryReserve(string key) => keys.TryAdd(key, DateTimeOffset.UtcNow);
}
