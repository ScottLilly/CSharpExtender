using System;
using System.Text.Json;

namespace CSharpExtender.ExtensionMethods;

/// <summary>
/// Extension methods for JSON
/// </summary>
public static class JsonExtensionMethods
{
    // Shared, because System.Text.Json caches the converter and type metadata it
    // builds on the options instance. A new instance per call throws that away.
    // Nothing hands this out or mutates it, and it is read-only after first use.
    private static readonly JsonSerializerOptions s_indentedOptions =
        new JsonSerializerOptions { WriteIndented = true };

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

            // Walked as spans. JsonElement can be asked for a property by span, so
            // splitting the path first would allocate an array and a string per
            // segment that nothing else ever reads.
            ReadOnlySpan<char> remaining = path.AsSpan();

            while (true)
            {
                int separator = remaining.IndexOf('.');

                ReadOnlySpan<char> element =
                    separator < 0 ? remaining : remaining.Slice(0, separator);

                if (root.ValueKind == JsonValueKind.Object &&
                    root.TryGetProperty(element, out var value))
                {
                    root = value;
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Property '{new string(element)}' not found in JSON path '{path}'.");
                }

                if (separator < 0)
                {
                    break;
                }

                remaining = remaining.Slice(separator + 1);
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
    /// <param name="jsonSerializerOptions">The JSON serializer options (optional).</param>
    /// <returns>A JSON string representation of the object.</returns>
    public static string AsSerializedJson<T>(this T value, JsonSerializerOptions jsonSerializerOptions = null) where T : class
    {
        return JsonSerializer.Serialize(value, jsonSerializerOptions);
    }

    /// <summary>
    /// Deserialize the JSON string to an object of the specified type.
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

        return JsonSerializer.Serialize(doc, s_indentedOptions);
    }

    /// <summary>
    /// Serializes the specified object to a JSON string with indented formatting.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="jsonSerializerOptions">The JSON serializer options (optional). The caller's instance is not modified.</param>
    /// <returns>A formatted JSON string representation of the object.</returns>
    public static string PrettyPrintJson(this object obj,
        JsonSerializerOptions jsonSerializerOptions = null)
    {
        // Copy rather than set WriteIndented on the caller's instance. A
        // JsonSerializerOptions becomes read-only once it has been used to
        // serialize, so writing to it would throw for any cached instance.
        JsonSerializerOptions options =
            jsonSerializerOptions == null
            ? s_indentedOptions
            : new JsonSerializerOptions(jsonSerializerOptions) { WriteIndented = true };

        return JsonSerializer.Serialize(obj, options);
    }
}