using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CSharpExtender.Models;

/// <summary>
/// Base model, used for property changed notification and tracking changed property values.
/// </summary>
public class PropertyChangeTrackingModel : ObservableModel, IChangeTracking
{
    /// <summary>
    /// True when any property has changed since the model was created or
    /// <see cref="AcceptChanges"/> was last called.
    /// </summary>
    public bool IsChanged => PropertyChangeLog.Count > 0;

    /// <summary>
    /// One entry per property change, in the order the changes happened.
    /// </summary>
    public ObservableCollection<PropertyChangedLog> PropertyChangeLog { get; } =
        new ObservableCollection<PropertyChangedLog>();

    /// <summary>
    /// Instance constructor.
    /// </summary>
    public PropertyChangeTrackingModel()
    {
        PropertyChangeLog.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(IsChanged));
        };
    }

    /// <summary>
    /// Writes a value to a property's backing field, raises
    /// <see cref="ObservableModel.PropertyChanged"/>, and adds an entry to
    /// <see cref="PropertyChangeLog"/>.
    /// </summary>
    /// <typeparam name="T">The property's type.</typeparam>
    /// <param name="field">The field holding the property's value.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="propertyName">
    /// The property's name, supplied by the compiler when called from the property's own
    /// setter. A null still notifies, and logs nothing, because the log records changes
    /// to one named property at a time.
    /// </param>
    /// <returns>True when the value changed, false when it was already that value.</returns>
    protected override bool SetProperty<T>(ref T field, T value,
        [CallerMemberName] string? propertyName = null)
    {
        bool propertyChanged = base.SetProperty(ref field, value, propertyName);

        // A null name is INotifyPropertyChanged's "every property changed", which a log
        // of individual property changes has no way to record. The notification still
        // goes out; there is just no one property to write down.
        if (propertyChanged && propertyName != null)
        {
            PropertyChangeLog.Add(new PropertyChangedLog(propertyName, value));
        }

        return propertyChanged;
    }

    /// <summary>
    /// Clears <see cref="PropertyChangeLog"/>, so the model's current values become the
    /// unchanged ones and <see cref="IsChanged"/> returns false.
    /// </summary>
    public void AcceptChanges()
    {
        PropertyChangeLog.Clear();
    }
}
