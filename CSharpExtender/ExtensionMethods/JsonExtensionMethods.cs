using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSharpExtender.ExtensionMethods
{
    /// <summary>
    /// Extension methods for JSON
    /// </summary>
    public static class JsonExtensionMethods
    {
        /// <summary>
        /// Retrieves a value from a JSON string using a JSON path.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <param name="path">The JSON path.</param>
        /// <returns>The value from the JSON string at the specified path.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the JSON path is not found.</exception>
        public static string GetValueFromJsonPath(this string json, string path) =>
            TryGetValueFromJsonPath(json, path, out string value) ? value
            : throw new InvalidOperationException($"JSON path '{path}' not found.");

        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static string AsSerializedJson<T>(this T value) where T : class
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Deserializes the JSON string to an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>An object of the specified type.</returns>
        public static T AsDeserializedJson<T>(this string json) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Formats the JSON string with indented formatting.
        /// </summary>
        /// <param name="json">The JSON string to format.</param>
        /// <returns>A formatted JSON string.</returns>
        public static string PrettyPrintJson(this string json)
        {
            var token = JToken.Parse(json);

            return token.ToString(Formatting.Indented);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string with indented formatting.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A formatted JSON string representation of the object.</returns>
        public static string PrettyPrintJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        #region Private Methods

        private static bool TryGetValueFromJsonPath(this string json,
            string path, out string value)
        {
            try
            {
                var token = JToken.Parse(json).SelectToken(path);

                if (token != null && token.Type != JTokenType.Null)
                {
                    value = token.ToString();

                    return true;
                }
            }
            catch (JsonException)
            {
                // Handle or log parsing exceptions
            }

            value = default;

            return false;
        }

        #endregion
    }
}