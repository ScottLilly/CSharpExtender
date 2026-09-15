using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CSharpExtender.Models;

/// <summary>
/// Base class for observable models (used for simpler property changed notification).
/// </summary>
public abstract class ObservableModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual bool SetProperty<T>(ref T backingField, T value,
        [CallerMemberName] string propertyName = null)
    {
        // EqualityComparer rather than the static object.Equals, which boxed both
        // sides for a value-typed property on every set, including the sets that
        // turn out to change nothing
        if (EqualityComparer<T>.Default.Equals(backingField, value))
        {
            return false;
        }

        backingField = value;

        OnPropertyChanged(propertyName);

        return true;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        // The event args are built here rather than held per property name. Holding
        // them was measured and rejected: a dictionary lookup cost more than the
        // small allocation it saved, and a caller passing a name it had computed
        // would have grown the cache without limit.
        // Nothing is built at all when there is no subscriber, because the
        // null-conditional skips the argument along with the call.
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
