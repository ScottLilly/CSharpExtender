using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

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
        /// <param name="path">The case-sensitive JSON path, using "." to identify children.</param>
        /// <returns>The value from the JSON string at the specified path.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the JSON path is not found.</exception>
        public static string GetValueFromJsonPath(this string json, string path)
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(json);

                JsonElement root = doc.RootElement;

                foreach (var element in path.Split('.'))
                {
                    if (root.TryGetProperty(element, out var value))
                    {
                        root = value;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Property '{element}' not found in JSON path '{path}'.");
                    }
                }

                return root.ToString();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unable to retrieve value from JSON path '{path}'.", ex);
            }
        }

        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static string AsSerializedJson<T>(this T value) where T : class
        {
            return JsonSerializer.Serialize(value);
        }

        /// <summary>
        /// Deserializes the JSON string to an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>An object of the specified type.</returns>
        public static T AsDeserializedJson<T>(this string json) where T : class
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        /// <summary>
        /// Formats the JSON string with indented formatting.
        /// </summary>
        /// <param name="json">The JSON string to format.</param>
        /// <returns>A formatted JSON string.</returns>
        public static string PrettyPrintJson(this string json)
        {
            using JsonDocument doc = JsonDocument.Parse(json);

            JsonSerializerOptions options = 
                new JsonSerializerOptions { WriteIndented = true };

            return JsonSerializer.Serialize(doc, options);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string with indented formatting.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="jsonSerializerOptions">The JSON serializer options (optional).</param>
        /// <returns>A formatted JSON string representation of the object.</returns>
        public static string PrettyPrintJson(this object obj, 
            JsonSerializerOptions jsonSerializerOptions = null)
        {
            if (jsonSerializerOptions == null)
            {
                jsonSerializerOptions =
                    new JsonSerializerOptions { WriteIndented = true };
            }
            else
            {
                if (!jsonSerializerOptions.WriteIndented)
                {
                    jsonSerializerOptions.WriteIndented = true;
                }
            }

            return JsonSerializer.Serialize(obj, jsonSerializerOptions);
        }
    }
}