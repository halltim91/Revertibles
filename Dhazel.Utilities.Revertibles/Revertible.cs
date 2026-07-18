
using System.Reflection;

namespace Dhazel.Utilities.Revertibles;

/// <summary>
/// Can track any given object and detect changes to that object's properties.
/// </summary>
/// <param name="trackedObject"></param>
public class Revertible(Object trackedObject) : IRevertible
{
    private readonly Dictionary<string, object> _pristineValues = [];
    private List<string> _propertiesToTrack = [];
    private bool _isChangeTrackingActive;

    private Object TrackedObject { get; set; } = trackedObject;

    public static Revertible Track(Object myObject)
    {
        var revertible = new Revertible(myObject);
        revertible.EnableChangeTracking();

        return revertible;
    }

    /// <summary>
    /// Enable change tracking
    /// </summary>
    public void EnableChangeTracking()
    {
        _isChangeTrackingActive = true;
        _propertiesToTrack = GetPropertiesToTrack(TrackedObject);
        AcceptChanges();
    }

    /// <summary>
    /// Gets the properties that should be tracked for changes.
    /// </summary>
    /// <exception cref="RevertibleException"></exception>
    protected virtual List<string> GetPropertiesToTrack(Object trackedObject)
    {
        var propertiesToTrack = trackedObject.GetType().GetProperties()
            .Where(pi => pi.CanWrite)
            .Where(prop => Attribute.IsDefined(prop, typeof(RevertibleAttribute)))
            .Select(pi => pi.Name)
            .ToList();

        if (!propertiesToTrack.Any())
            throw new RevertibleException("There are no properties marked to be tracked. Please add `[Revertible]` attributes to the class properties that you want to track.");

        return propertiesToTrack;
    }

    /// <inheritdoc/>
    public bool HasChanged(string? propertyName = null)
    {
        GuardChangeTracking();
        GuardPropertyName(propertyName);

        if (propertyName is null)
        {
            return _propertiesToTrack.Any(p => HasChanged(p));
        }

        var currentValue = GetValue(propertyName);

        if (_pristineValues.TryGetValue(propertyName, out var pristineValue))
        {
            return !Equals(currentValue, pristineValue);
        }
        return false;
    }

    /// <inheritdoc/>
    public void Revert(string? propertyName = null)
    {
        GuardChangeTracking();
        GuardPropertyName(propertyName);

        if (propertyName is null)
        {
            foreach (var name in _propertiesToTrack)
            {
                Revert(name);
            }
            return;
        }

        if (_pristineValues.TryGetValue(propertyName, out var pristineValue))
        {
            SetValue(propertyName, pristineValue);
        }
    }

    /// <inheritdoc/>
    public void AcceptChanges(string? propertyName = null)
    {
        GuardChangeTracking();
        GuardPropertyName(propertyName);

        if (propertyName is null)
        {
            foreach (var name in _propertiesToTrack)
            {
                AcceptChanges(name);
            }
        }
        else
        {
            var currentValue = GetValue(propertyName);

            _pristineValues[propertyName] = currentValue!;
        }
    }

    protected object GetValue(string propertyName)
    {
        GuardPropertyName(propertyName);
        return GetPropertyInfo(propertyName).GetValue(TrackedObject);
    }

    protected void SetValue(string propertyName, object pristineValue)
    {
        GuardPropertyName(propertyName);
        GetPropertyInfo(propertyName).SetValue(TrackedObject, pristineValue);
    }

    /// <summary>
    /// Guard to ensure that the property name is valid
    /// </summary>
    /// <param name="propertyName"></param>
    /// <exception cref="RevertibleException"></exception>
    protected void GuardPropertyName(string? propertyName)
    {
        if (propertyName != null && !_propertiesToTrack.Contains(propertyName))
        {
            throw new RevertibleException($"The given property name, {propertyName}, is not tracked for changes.");
        }
    }

    /// <summary>
    /// Guard to ensure that change tracking is enabled
    /// </summary>
    /// <exception cref="RevertibleException"></exception>
    protected void GuardChangeTracking()
    {
        if (!_isChangeTrackingActive)
            throw new RevertibleException("Change tracking must be enabled.");
    }

    protected PropertyInfo GetPropertyInfo(string? propertyName)
    {
        var propertyInfo = TrackedObject.GetType().GetProperty(propertyName);

        if (propertyInfo is null)
        {
            throw new ArgumentException($"The requested property, `{propertyName}`, does not exist on the object, `{TrackedObject.GetType().Name}`.", nameof(propertyName));
        }

        return propertyInfo;
    }

    public IRevertible WithProperties(List<string> propertyNames)
    {
        _isChangeTrackingActive = true;
        _propertiesToTrack = propertyNames;
        AcceptChanges();
        return this;
    }
}
