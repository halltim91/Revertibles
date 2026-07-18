using CommunityToolkit.Mvvm.ComponentModel;
using Dhazel.Utilities.Revertibles;


namespace Dhazel.Utilities.Mvvm.Validators;

public partial class RevertibleValidator : ObservableValidator, IRevertible
{
    private IRevertible _revertible;

    public RevertibleValidator()
    {
        _revertible = Revertible.Track(this);
    }

    public void AcceptChanges(string? propertyName = null)
    {
        _revertible.AcceptChanges(propertyName);
    }

    public bool HasChanged(string? propertyName = null)
    {
        return _revertible.HasChanged(propertyName);
    }

    public void Revert(string? propertyName = null)
    {
        _revertible.Revert(propertyName);
    }

    public IRevertible WithProperties(List<string> propertyNames)
    {
        return _revertible.WithProperties(propertyNames);
    }
}
