using Aegis.Template.Modules.Modules.WorkItems.Events;

namespace Aegis.Template.Modules.Modules.WorkItems.Domain;

public sealed class WorkItem
{
    private readonly List<WorkItemCreatedDomainEvent> domainEvents = [];

    private WorkItem()
    {
    }

    private WorkItem(Guid id, string title)
    {
        Id = id;
        Title = title;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        domainEvents.Add(new WorkItemCreatedDomainEvent(id, CreatedAtUtc));
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public IReadOnlyList<WorkItemCreatedDomainEvent> DomainEvents => domainEvents;

    public static WorkItem Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        return new WorkItem(Guid.NewGuid(), title.Trim());
    }
}
