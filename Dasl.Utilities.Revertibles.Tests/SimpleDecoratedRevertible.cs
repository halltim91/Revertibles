using Dasl.Utilities.Revertibles;


namespace Dasl.Utilities.Revertibles.Tests;

public class SimpleDecoratedRevertible : IRevertible
{
    private Revertible _revertible;

    public SimpleDecoratedRevertible()
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

    [Revertible]
    public int Id { get; set; }

    [Revertible]
    public string MyString { get; set; }

    [Revertible]
    public bool MyBool { get; set; }
}
