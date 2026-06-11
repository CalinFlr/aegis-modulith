namespace Aegis.Template.BuildingBlocks.Results;

public sealed record Result(bool Succeeded, string? Error)
{
    public static Result Success() => new(true, null);

    public static Result Failure(string error) => new(false, error);
}
