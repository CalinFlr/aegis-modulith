namespace AegisItemRootNamespace.Modules.Aegis.Module.Domain;

public sealed class Aegis.ModuleEntity
{
    private Aegis.ModuleEntity()
    {
    }

    public Aegis.ModuleEntity(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;
}
