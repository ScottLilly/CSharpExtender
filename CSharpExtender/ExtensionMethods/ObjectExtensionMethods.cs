using System;
using System.Reflection;

namespace CSharpExtender.ExtensionMethods
{
    /// <summary>
    /// Extension methods for objects
    /// </summary>
    public static class ObjectExtensionMethods
    {
        /// <summary>
        /// Creates a deep clone of the source object.
        /// </summary>
        /// <typeparam name="T">The type of the source object.</typeparam>
        /// <param name="source">The source object to clone.</param>
        /// <returns>A deep clone of the source object.</returns>
        public static T DeepClone<T>(this T source) where T : class
        {
            if (source == null)
            {
                return default;
            }

            // Serialize the object to a JSON string.
            var serialized = source.AsSerializedJson();

            // Deserialize the JSON string to a new object of the same type.
            return serialized.AsDeserializedJson<T>();
        }

        /// <summary>
        /// Checks if the object is of a numeric type.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object is of a numeric type, false otherwise.</returns>
        public static bool IsNumericType(this object obj)
        {
            var typeCode = Type.GetTypeCode(obj.GetType());

            switch (typeCode)
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

        /// <summary>
        /// Checks if the object is of an integer type.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object is of an integer type, false otherwise.</returns>
        public static bool IsIntegerType(this object obj)
        {
            var typeCode = Type.GetTypeCode(obj.GetType());

            switch (typeCode)
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

        /// <summary>
        /// Checks if the object is of a floating point type.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object is of a floating point type, false otherwise.</returns>
        public static bool IsFloatingPointType(this object obj)
        {
            var typeCode = Type.GetTypeCode(obj.GetType());

            switch (typeCode)
            {
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if the property info has a custom attribute of a specific type.
        /// </summary>
        /// <param name="obj">The property info to check.</param>
        /// <param name="attributeType">The type of the attribute to check for.</param>
        /// <returns>True if the property info has a custom attribute of the specified type, false otherwise.</returns>
        public static bool HasCustomAttributeOfType(this PropertyInfo obj, Type attributeType)
        {
            return obj.IsDefined(attributeType, true);
        }

        /// <summary>
        /// Checks if the object is null.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object is null, false otherwise.</returns>
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        /// <summary>
        /// Checks if the object is not null.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object is not null, false otherwise.</returns>
        public static bool IsNotNull(this object obj)
        {
            return obj != null;
        }

        /// <summary>
        /// Checks if the object is of a specific type.
        /// </summary>
        /// <typeparam name="T">The type to check for.</typeparam>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object is of the specified type, false otherwise.</returns>
        public static bool IsOfType<T>(this object obj)
        {
            return obj is T;
        }

        /// <summary>
        /// Checks if the object is of a specific type.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <param name="type">The type to check for.</param>
        /// <returns>True if the object is of the specified type, false otherwise.</returns>
        public static bool IsOfType(this object obj, Type type)
        {
            return obj.GetType() == type;
        }

        /// <summary>
        /// Checks if the object is not of a specific type.
        /// </summary>
        /// <typeparam name="T">The type to check against.</typeparam>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object is not of the specified type, false otherwise.</returns>
        public static bool IsNotOfType<T>(this object obj)
        {
            return !(obj is T);
        }

        /// <summary>
        /// Checks if the object is not of a specific type.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <param name="type">The type to check against.</param>
        /// <returns>True if the object is not of the specified type, false otherwise.</returns>
        public static bool IsNotOfType(this object obj, Type type)
        {
            return obj.GetType() != type;
        }

        /// <summary>
        /// Checks if the object is of a specific type or a subclass of that type.
        /// </summary>
        /// <typeparam name="T">The type to check for.</typeparam>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object is of the specified type or a subclass of that type, false otherwise.</returns>
        public static bool IsOfTypeOrSubclass<T>(this object obj)
        {
            return obj is T;
        }

        /// <summary>
        /// Checks if the object is of a specific type or a subclass of that type.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <param name="type">The type to check for.</param>
        /// <returns>True if the object is of the specified type or a subclass of that type, false otherwise.</returns>
        public static bool IsOfTypeOrSubclass(this object obj, Type type)
        {
            return type.IsAssignableFrom(obj.GetType());
        }

        /// <summary>
        /// Checks if the object is not of a specific type and not a subclass of that type.
        /// </summary>
        /// <typeparam name="T">The type to check against.</typeparam>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object is not of the specified type and not a subclass of that type, false otherwise.</returns>
        public static bool IsNotOfTypeOrSubclass<T>(this object obj)
        {
            return !(obj is T);
        }

        /// <summary>
        /// Checks if the object is not of a specific type and not a subclass of that type.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <param name="type">The type to check against.</param>
        /// <returns>True if the object is not of the specified type and not a subclass of that type, false otherwise.</returns>
        public static bool IsNotOfTypeOrSubclass(this object obj, Type type)
        {
            return !type.IsAssignableFrom(obj.GetType());
        }

        /// <summary>
        /// Checks if the type is of a specific type or a subclass of that type.
        /// </summary>
        /// <typeparam name="T">The type to check for.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is of the specified type or a subclass of that type, false otherwise.</returns>
        public static bool IsOfTypeOrSubclass<T>(this Type type)
        {
            return type.IsSubclassOf(typeof(T));
        }

        /// <summary>
        /// Checks if the type is of a specific type or a subclass of that type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="baseType">The base type to check for.</param>
        /// <returns>True if the type is of the specified base type or a subclass of that base type, false otherwise.</returns>
        public static bool IsOfTypeOrSubclass(this Type type, Type baseType)
        {
            return type.IsSubclassOf(baseType);
        }

        /// <summary>
        /// Checks if the type is not of a specific type and not a subclass of that type.
        /// </summary>
        /// <typeparam name="T">The type to check against.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is not of the specified type and not a subclass of that type, false otherwise.</returns>
        public static bool IsNotOfTypeOrSubclass<T>(this Type type)
        {
            return !type.IsSubclassOf(typeof(T));
        }

        /// <summary>
        /// Checks if the type is not of a specific type and not a subclass of that type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="baseType">The base type to check against.</param>
        /// <returns>True if the type is not of the specified base type and not a subclass of that base type, false otherwise.</returns>
        public static bool IsNotOfTypeOrSubclass(this Type type, Type baseType)
        {
            return !type.IsSubclassOf(baseType);
        }

        /// <summary>
        /// Checks if the type is of a specific type.
        /// </summary>
        /// <typeparam name="T">The type to check for.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is of the specified type, false otherwise.</returns>
        public static bool IsOfType<T>(this Type type)
        {
            return type == typeof(T);
        }

        /// <summary>
        /// Checks if the type is of a specific type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="baseType">The base type to check for.</param>
        /// <returns>True if the type is of the specified base type, false otherwise.</returns>
        public static bool IsOfType(this Type type, Type baseType)
        {
            return type == baseType;
        }

        /// <summary>
        /// Checks if the type is not of a specific type.
        /// </summary>
        /// <typeparam name="T">The type to check against.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is not of the specified type, false otherwise.</returns>
        public static bool IsNotOfType<T>(this Type type)
        {
            return type != typeof(T);
        }

        /// <summary>
        /// Checks if the type is not of a specific type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="baseType">The base type to check against.</param>
        /// <returns>True if the type is not of the specified base type, false otherwise.</returns>
        public static bool IsNotOfType(this Type type, Type baseType)
        {
            return type != baseType;
        }
    }
}