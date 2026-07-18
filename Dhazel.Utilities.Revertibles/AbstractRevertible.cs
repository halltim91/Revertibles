using Dhazel.Utilities.Revertibles;


namespace Dhazel.Utilities.Revertibles;

public abstract class AbstractRevertible : IRevertible
{
    private Revertible _revertible;

    public AbstractRevertible()
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
        throw new NotImplementedException("TODO");
    }
}
