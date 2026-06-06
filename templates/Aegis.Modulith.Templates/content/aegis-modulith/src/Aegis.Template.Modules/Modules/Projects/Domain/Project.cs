using Aegis.Template.Modules.Modules.Projects.Events;

namespace Aegis.Template.Modules.Modules.Projects.Domain;

public sealed class Project
{
    private readonly List<ProjectCreatedDomainEvent> domainEvents = [];

    private Project()
    {
    }

    private Project(Guid id, string name)
    {
        Id = id;
        Name = name;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        domainEvents.Add(new ProjectCreatedDomainEvent(id, CreatedAtUtc));
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public IReadOnlyList<ProjectCreatedDomainEvent> DomainEvents => domainEvents;

    public static Project Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Project name is required.", nameof(name));
        }

        return new Project(Guid.NewGuid(), name.Trim());
    }
}
