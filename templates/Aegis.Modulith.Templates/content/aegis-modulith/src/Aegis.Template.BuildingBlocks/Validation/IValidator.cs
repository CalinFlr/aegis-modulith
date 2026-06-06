namespace Aegis.Template.BuildingBlocks.Validation;

public interface IValidator<in T>
{
    IReadOnlyList<string> Validate(T instance);
}
