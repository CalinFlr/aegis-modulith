namespace Aegis.Template.BuildingBlocks.Events;

public abstract record IntegrationEvent(Guid Id, DateTimeOffset OccurredAtUtc);

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class IntegrationEventContractAttribute(string type, int version) : Attribute
{
    public string Type { get; } = string.IsNullOrWhiteSpace(type)
        ? throw new ArgumentException("Integration event contract type is required.", nameof(type))
        : type;

    public int Version { get; } = version > 0
        ? version
        : throw new ArgumentOutOfRangeException(nameof(version), "Integration event contract version must be positive.");
}

public static class IntegrationEventContractMetadata
{
    public static string TypeName<TEvent>()
        where TEvent : IntegrationEvent
    {
        return TypeName(typeof(TEvent));
    }

    public static string TypeName(Type eventType)
    {
        return GetAttribute(eventType).Type;
    }

    public static int Version(Type eventType)
    {
        return GetAttribute(eventType).Version;
    }

    private static IntegrationEventContractAttribute GetAttribute(Type eventType)
    {
        if (!typeof(IntegrationEvent).IsAssignableFrom(eventType))
        {
            throw new ArgumentException($"{eventType.FullName} is not an integration event.", nameof(eventType));
        }

        return eventType
            .GetCustomAttributes(typeof(IntegrationEventContractAttribute), inherit: false)
            .OfType<IntegrationEventContractAttribute>()
            .SingleOrDefault()
            ?? throw new InvalidOperationException($"{eventType.FullName} is missing IntegrationEventContractAttribute.");
    }
}
