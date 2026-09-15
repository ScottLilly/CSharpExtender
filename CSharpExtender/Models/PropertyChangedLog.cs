using System;

namespace CSharpExtender.Models;

/// <summary>
/// Track property values changes.
/// Used in PropertyChangedLogViewModel.
/// Can be extended, to track other properties, like user who changed the property value.
/// </summary>
public class PropertyChangedLog
{
    public string? PropertyName { get; }
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

    public PropertyChangedLog(string? propertyName, object? newValue)
    {
        PropertyName = propertyName;
        NewValue = newValue;
        ChangeDateTime = DateTime.UtcNow;
    }
}
