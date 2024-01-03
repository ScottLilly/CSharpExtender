using System.Reflection;
using System.Text.Json;

namespace CSharpExtender.ExtensionMethods;

public static class ObjectExtensionMethods
{
    public static T Clone<T>(this T source)
    {
        return JsonSerializer.Deserialize<T>((string?)JsonSerializer.Serialize(source));
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