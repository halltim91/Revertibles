namespace Dhazel.Revertibles;

[AttributeUsage(AttributeTargets.Class)]
public sealed class RevertibleObjectAttribute : Attribute
{
    /// <summary> When false, pristine values are NOT captured automatically. Manually calling AcceptChanges() will be required to begin change tracking </summary>
    public bool AcceptPristineValues { get; set; } = true;
}
