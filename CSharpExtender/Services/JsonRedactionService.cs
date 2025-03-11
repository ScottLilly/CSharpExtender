using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CSharpExtender.Services;

public class JsonRedactionService(List<string> redactedPaths, bool ignoreCase = false)
    : BaseRedactionService(redactedPaths, ignoreCase), IRedactionService<JsonObject>
{
    /// <summary>
    /// Redacts values in the given JsonObject based on the specified redacted paths.
    /// </summary>
    /// <param name="obj">The JsonObject to redact.</param>
    /// <returns>The redacted JsonObject.</returns>
    public JsonObject Redact(JsonObject obj)
    {
        RedactJsonNode(obj);

        return obj;
    }

    /// <summary>
    /// Converts a raw JSON string into a JsonObject, applies redaction, and returns the redacted JsonObject.
    /// </summary>
    /// <param name="text">The raw JSON string to convert and redact.</param>
    /// <returns>The redacted JsonObject.</returns>
    public JsonObject Redact(string text)
    {
        var jsonObject = JsonSerializer.Deserialize<JsonObject>(text);

        RedactJsonNode(jsonObject);

        return jsonObject;
    }

    /// <summary>
    /// Redact a JsonObject and returns the redacted JSON as a string.
    /// </summary>
    /// <param name="obj">The JsonObject to redact.</param>
    /// <returns>The redacted JSON as a string.</returns>
    public string RedactToString(JsonObject obj)
    {
        RedactJsonNode(obj);

        return obj.ToString();
    }

    /// <summary>
    /// Redact a raw JSON string by deserializing it to a JsonObject and applying redactions.
    /// </summary>
    /// <param name="text">The JSON string to redact.</param>
    /// <returns>The redacted JSON string.</returns>
    public string RedactToString(string text)
    {
        var jsonObject = JsonSerializer.Deserialize<JsonObject>(text);

        RedactJsonNode(jsonObject);

        return jsonObject.ToString();
    }

    #region Private Methods

    /// <summary>
    /// Walks through the JSON nodes once and applies the redaction based on regex matching.
    /// </summary>
    private void RedactJsonNode(JsonNode node, string currentPath = "")
    {
        // The current node is null or the pattern is empty, no need to process further
        if (node == null || _isEmptyPattern)
        {
            return;
        }

        // Check if the current node path matches any of the redacted paths
        if (_redactedPathRegex.IsMatch(currentPath))
        {
            // Apply redaction if the current path matches any redacted path
            node = GetDefaultValue(node);
        }

        // Recursively process child nodes (objects or arrays)
        if (node is JsonObject jObject)
        {
            var keysToRedact = new List<string>();

            foreach (var property in jObject)
            {
                var newPath = string.IsNullOrEmpty(currentPath) 
                    ? property.Key 
                    : $"{currentPath}.{property.Key}";

                if (_redactedPathRegex.IsMatch(newPath))
                {
                    // If the property key matches, add it to the list of keys to redact
                    keysToRedact.Add(property.Key);
                }
                else
                {
                    // Recursively redact child nodes
                    RedactJsonNode(property.Value, newPath);
                }
            }

            // Redact the properties after iterating to avoid modifying the collection during iteration
            foreach (var key in keysToRedact)
            {
                jObject[key] = GetDefaultValue(jObject[key]);
            }
        }
        else if (node is JsonArray jArray)
        {
            for (int i = 0; i < jArray.Count; i++)
            {
                // Recursively redact array items
                RedactJsonNode(jArray[i], $"{currentPath}[{i}]");
            }
        }
    }


    private static JsonNode GetDefaultValue(JsonNode node)
    {
        // Default to null, empty string, or zero based on the node type
        return node switch
        {
            JsonValue jValue when jValue.TryGetValue(out string _) => string.Empty,
            JsonValue jValue when jValue.TryGetValue(out int _) => 0,
            JsonValue jValue when jValue.TryGetValue(out bool _) => false,
            _ => null
        };
    }

    #endregion
}
