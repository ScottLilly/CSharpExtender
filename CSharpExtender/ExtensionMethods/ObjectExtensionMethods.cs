using Newtonsoft.Json;
using System;
using System.Reflection;

namespace CSharpExtender.ExtensionMethods
{
    /// <summary>
    /// Scott's extension methods for objects
    /// </summary>
    public static class ObjectExtensionMethods
    {
        public static T Clone<T>(this T source)
        {
            if (source == null)
            {
                return default;
            }

            // Serialize the object to a JSON string.
            var serialized = JsonConvert.SerializeObject(source);

            // Deserialize the JSON string to a new object of the same type.
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static bool IsNumericType(this object obj)
        {
            switch (Type.GetTypeCode(obj.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsIntegerType(this object obj)
        {
            switch (Type.GetTypeCode(obj.GetType()))
            {
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return true;
                default:
                    return false;
            }
        }

        internal static bool HasCustomAttributeOfType(this PropertyInfo obj, Type attributeType)
        {
            return obj.GetCustomAttributes(attributeType, true).Length > 0;
        }
    }
}