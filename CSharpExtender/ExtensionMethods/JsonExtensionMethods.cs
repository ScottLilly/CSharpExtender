using System;
using System.Text.Json;

namespace CSharpExtender.ExtensionMethods
{
    /// <summary>
    /// Extension methods for JSON
    /// </summary>
    public static class JsonExtensionMethods
    {
        /// <summary>
        /// Retrieves a value from a JSON element using a JSON path.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <param name="path">The case-sensitive JSON path, using "." to identify children.</param>
        /// <returns>The value from the JSON element at the specified path.</returns>
        /// <exception cref="ArgumentNullException">Thrown if json or path is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the property is not found or JSON is invalid.</exception>
        public static T GetValueFromJsonPath<T>(this string json, string path)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new ArgumentNullException(nameof(json), 
                    "JSON string cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path), 
                    "JSON path cannot be null or empty.");
            }

            try
            {
                using JsonDocument doc = JsonDocument.Parse(json);
                JsonElement root = doc.RootElement;

                foreach (var element in path.Split('.'))
                {
                    if (root.ValueKind == JsonValueKind.Object && 
                        root.TryGetProperty(element, out var value))
                    {
                        root = value;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Property '{element}' not found in JSON path '{path}'.");
                    }
                }

                return root.ToString().ConvertFromString<T>();
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Invalid JSON format.", ex);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error processing JSON path '{path}'.", ex);
            }
        }

        /// <summary>
        /// Retrieves a string value from a JSON element using a JSON path.
        /// This is a convenience method for GetValueFromJsonPath&lt;string&gt;.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <param name="path">The case-sensitive JSON path, using "." to identify children.</param>
        /// <returns>The value from the JSON element at the specified path.</returns>
        /// <exception cref="ArgumentNullException">Thrown if json or path is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the property is not found or JSON is invalid.</exception>
        public static string GetValueFromJsonPath(this string json, string path)
        {
            return GetValueFromJsonPath<string>(json, path);
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