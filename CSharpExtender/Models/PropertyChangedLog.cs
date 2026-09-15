using System;

namespace CSharpExtender.Models;

/// <summary>
/// Track property values changes.
/// Used in PropertyChangedLogViewModel.
/// Can be extended, to track other properties, like user who changed the property value.
/// </summary>
public class PropertyChangedLog
{
    /// <summary>
    /// The name of the property that changed. Never null: an entry that cannot say
    /// which property it is about records nothing.
    /// </summary>
    public string PropertyName { get; }

    public object? NewValue { get; }

    /// <summary>
    /// When the property was changed, in UTC. <see cref="DateTime.Kind"/> is
    /// <see cref="DateTimeKind.Utc"/>, so call <see cref="DateTime.ToLocalTime"/> to display it.
    /// </summary>
    /// <remarks>
    /// Local time would make the log ambiguous for the hour that repeats every autumn: two entries
    /// stamped 01:30 can be an hour apart, and the order they were written in is not recoverable
    /// from the values. A library also cannot know which time zone the reader is in, so choosing
    /// one is the consumer's call and it needs an unambiguous value to make it from.
    /// </remarks>
    public DateTime ChangeDateTime { get; }

    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="propertyName"/> is null.
    /// </exception>
    public PropertyChangedLog(string propertyName, object? newValue)
    {
        ArgumentNullException.ThrowIfNull(propertyName);

        PropertyName = propertyName;
        NewValue = newValue;
        ChangeDateTime = DateTime.UtcNow;
    }
}
