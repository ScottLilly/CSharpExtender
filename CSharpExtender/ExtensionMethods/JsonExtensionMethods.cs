using System.Text.Json;

namespace CSharpExtender.ExtensionMethods;

public static class JsonExtensionMethods
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions =
       new() { PropertyNameCaseInsensitive = true };

    public static string GetValueFromJsonPath(this string json, string path,
        JsonSerializerOptions? options = null) =>
        JsonSerializer.Deserialize<JsonElement>(json, options ?? _jsonSerializerOptions)
        .GetJsonElement(path)
        .GetJsonElementValue() ??
        throw new InvalidOperationException($"JSON path '{path}' not found.");

    private static readonly char[] separator = new char[] { '.', ':' };

    public static JsonElement GetJsonElement(this JsonElement jsonElement, string path)
    {
        if (jsonElement.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
        {
            return default;
        }

        string[] segments =
            path.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        foreach (var segment in segments)
        {
            if (int.TryParse(segment, out var index) &&
                jsonElement.ValueKind == JsonValueKind.Array)
            {
                jsonElement = jsonElement.EnumerateArray().ElementAtOrDefault(index);

                if (jsonElement.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
                {
                    return default;
                }

                continue;
            }

            jsonElement = jsonElement.TryGetProperty(segment, out var value) ? value : default;

            if (jsonElement.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
            {
                return default;
            }
        }

        return jsonElement;
    }

    public static string? GetJsonElementValue(this JsonElement jsonElement) =>
        jsonElement.ValueKind != JsonValueKind.Null &&
        jsonElement.ValueKind != JsonValueKind.Undefined
        ? jsonElement.ToString()
        : default;

    public static string? AsSerializedJson<T>(
        this T? value, JsonSerializerOptions? options = null) where T : class
    {
        return value == null
            ? null
            : JsonSerializer.Serialize(value, options);
    }
}