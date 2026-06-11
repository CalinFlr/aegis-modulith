using Aegis.Template.Modules.Modules.Tasks.Events;

namespace Aegis.Template.Modules.Modules.Tasks.Domain;

public sealed class TaskItem
{
    private readonly List<TaskCreatedDomainEvent> domainEvents = [];

    private TaskItem()
    {
    }

    private TaskItem(Guid id, Guid projectId, string title)
    {
        Id = id;
        ProjectId = projectId;
        Title = title;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        domainEvents.Add(new TaskCreatedDomainEvent(id, projectId, CreatedAtUtc));
    }

    public Guid Id { get; private set; }

    public Guid ProjectId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public IReadOnlyList<TaskCreatedDomainEvent> DomainEvents => domainEvents;

    public static TaskItem Create(Guid projectId, string title)
    {
        if (projectId == Guid.Empty)
        {
            throw new ArgumentException("Project id is required.", nameof(projectId));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Task title is required.", nameof(title));
        }

        return new TaskItem(Guid.NewGuid(), projectId, title.Trim());
    }
}
