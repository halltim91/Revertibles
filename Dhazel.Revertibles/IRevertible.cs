

namespace Dhazel.Revertibles;

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

    /// <summary>
    /// Optional method to define the properties that the Revertible will track.
    /// This is useful for tracking objects that may be from a third-party source,
    /// or otherwise do not have `[Revertible]` attributes assigned to their 
    /// properties.
    /// </summary>
    /// <param name="propertyNames">The names of the properties to track</param>
    /// <returns>The Revertible object, chainable</returns>
    IRevertible WithProperties(List<string> propertyNames);
}
