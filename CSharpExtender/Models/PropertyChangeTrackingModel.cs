using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CSharpExtender.Models;

/// <summary>
/// Base model, used for property changed notification and tracking changed property values.
/// </summary>
public class PropertyChangeTrackingModel : ObservableModel, IChangeTracking
{
    public bool IsChanged => PropertyChangeLog.Count > 0;

    public ObservableCollection<PropertyChangedLog> PropertyChangeLog { get; } = 
        new ObservableCollection<PropertyChangedLog>();

    public PropertyChangeTrackingModel()
    {
        PropertyChangeLog.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(IsChanged));
        };
    }

    protected override bool SetProperty<T>(ref T field, T value, 
        [CallerMemberName] string propertyName = null)
    {
        bool propertyChanged = base.SetProperty(ref field, value, propertyName);

        if (propertyChanged)
        {
            PropertyChangeLog.Add(new PropertyChangedLog(propertyName, value));
        }

        return propertyChanged;
    }

    public void AcceptChanges()
    {
        PropertyChangeLog.Clear();
    }
}
