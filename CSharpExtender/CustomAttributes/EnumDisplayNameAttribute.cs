using System;

namespace CSharpExtender.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class EnumDisplayNameAttribute : Attribute
    {
        public string DisplayName { get; }

        public EnumDisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}