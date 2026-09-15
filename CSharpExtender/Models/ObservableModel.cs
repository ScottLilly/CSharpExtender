using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CSharpExtender.Models;

/// <summary>
/// Base class for observable models (used for simpler property changed notification).
/// </summary>
public abstract class ObservableModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual bool SetProperty<T>(ref T backingField, T value,
        [CallerMemberName] string? propertyName = null)
    {
        // EqualityComparer compares a value-typed property without boxing either side
        if (EqualityComparer<T>.Default.Equals(backingField, value))
        {
            return false;
        }

        backingField = value;

        OnPropertyChanged(propertyName);

        return true;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        // The null-conditional skips the argument along with the call, so nothing is
        // built at all when there is no subscriber
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
