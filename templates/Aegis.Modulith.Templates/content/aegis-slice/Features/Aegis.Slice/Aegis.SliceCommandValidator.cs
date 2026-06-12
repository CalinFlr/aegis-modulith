using AegisBuildingBlocksNamespace.Validation;

namespace AegisItemRootNamespace.Modules.AegisSliceModule.Features.Aegis.Slice;

public sealed class Aegis.SliceCommandValidator : IValidator<Aegis.SliceCommand>
{
    public IReadOnlyList<string> Validate(Aegis.SliceCommand instance)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(instance.Name))
        {
            errors.Add("Name is required.");
        }

        return errors;
    }
}
