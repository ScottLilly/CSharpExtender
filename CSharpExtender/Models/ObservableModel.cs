using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CSharpExtender.Models;

/// <summary>
/// Base class for observable models (used for simpler property changed notification).
/// </summary>
public abstract class ObservableModel : INotifyPropertyChanged
{
    /// <summary>
    /// Raised when a property's value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Writes a value to a property's backing field and raises
    /// <see cref="PropertyChanged"/>, doing neither when the value is the one already
    /// there.
    /// </summary>
    /// <typeparam name="T">The property's type.</typeparam>
    /// <param name="backingField">The field holding the property's value.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="propertyName">
    /// The property's name, supplied by the compiler when called from the property's own
    /// setter.
    /// </param>
    /// <returns>True when the value changed, false when it was already that value.</returns>
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

    /// <summary>
    /// Raises <see cref="PropertyChanged"/> for a property, for the cases
    /// <see cref="SetProperty"/> does not cover, such as a calculated property whose
    /// value depends on one that just changed.
    /// </summary>
    /// <param name="propertyName">
    /// The property's name, supplied by the compiler when called from the property
    /// itself. A null means every property changed, which is what
    /// <see cref="INotifyPropertyChanged"/> defines it as.
    /// </param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        // The null-conditional skips the argument along with the call, so nothing is
        // built at all when there is no subscriber
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
