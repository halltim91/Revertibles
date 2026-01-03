

namespace Dasl.Utilities.Revertibles;

public interface IRevertible
{
    /// <summary>
    /// Check for changes in any property or just the given property.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    bool HasChanged(string? propertyName = null);

    /// <summary>
    /// Reset all properties, or the given property, to their pristine value
    /// </summary>
    /// <param name="propertyName">If null, then all properties of the object are
    /// affected; otherwise, only the given property is affected.</param>
    void Revert(string? propertyName = null);

    /// <summary>
    /// Update the change-tracking pristine value for all properties, or just the
    /// given property, so that change tracking is restarted from the current state.
    /// </summary>
    /// <param name="propertyName">If null, then all properties of the object are
    /// affected; otherwise, only the given property is affected.</param>
    void AcceptChanges(string? propertyName = null);
}
