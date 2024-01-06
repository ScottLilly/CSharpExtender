using CSharpExtender.CustomAttributes;
using System;
using System.Reflection;

namespace CSharpExtender.ExtensionMethods
{
    public static class EnumExtensionMethods
    {
        public static string GetEnumDisplayName(this Enum value) =>
            value.GetType()
            .GetField(value.ToString())?
            .GetCustomAttribute<EnumDisplayNameAttribute>()?.DisplayName
            ?? value.ToString();
    }
}