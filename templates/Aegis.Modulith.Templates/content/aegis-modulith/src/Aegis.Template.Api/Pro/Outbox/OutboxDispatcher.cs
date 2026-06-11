namespace Aegis.Template.Api.Pro.Outbox;

public sealed class OutboxDispatcher
{
    public object Describe() => new
    {
        status = "ready",
        mode = "outbox-ready skeleton",
        note = "Persist module integration events to a module-owned outbox table before wiring a broker."
    };
}
