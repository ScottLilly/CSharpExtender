using System;

namespace CSharpExtender.Models
{
    /// <summary>
    /// Track property values changes.
    /// Used in PropertyChangedLogViewModel.
    /// Can be extended, to track other properties, like user who changed the property value.
    /// </summary>
    public class PropertyChangedLog
    {
        public string PropertyName { get; }
        public object NewValue { get; }
        public DateTime ChangeDateTime { get; }

        public PropertyChangedLog(string propertyName, object newValue)
        {
            PropertyName = propertyName;
            NewValue = newValue;
            ChangeDateTime = DateTime.Now;
        }
    }
}
