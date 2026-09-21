using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CSharpExtender.Services;

/// <summary>
/// Redacts values in JSON, for paths matching any of the supplied regex patterns.
/// </summary>
/// <remarks>
/// Paths are property names from the root, separated by ".", with array entries indexed, so the
/// "ssn" property in {"user": {"ssn": "..."}} has the path "user.ssn" and the first entry of an
/// "items" array has the path "items[0]". Patterns are matched anywhere in a path rather than
/// against the whole of it, so a pattern that matches an object also matches everything below it.
/// A redacted string becomes empty, a number becomes zero, a boolean becomes false, and an object
/// or array becomes null.
/// </remarks>
/// <param name="redactedPaths">Regex patterns for the paths whose values are redacted.</param>
/// <param name="ignoreCase">Match the patterns without regard to case.</param>
public class JsonRedactionService(List<string> redactedPaths, bool ignoreCase = false)
    : BaseRedactionService(redactedPaths, ignoreCase), IRedactionService<JsonObject>
{
    /// <summary>
    /// Redact values in the given JsonObject based on the specified redacted paths.
    /// </summary>
    /// <param name="obj">The JsonObject to redact.</param>
    /// <returns>The redacted JsonObject.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    public JsonObject Redact(JsonObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

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
        var jsonObject = ParseObject(text);

        RedactJsonNode(jsonObject);

        return jsonObject;
    }

    /// <summary>
    /// Redact a JsonObject and returns the redacted JSON as a string.
    /// </summary>
    /// <param name="obj">The JsonObject to redact.</param>
    /// <returns>The redacted JSON as a string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    public string RedactToString(JsonObject obj)
    {
        return Redact(obj).ToString();
    }

    /// <summary>
    /// Redact a raw JSON string by deserializing it to a JsonObject and applying redactions.
    /// </summary>
    /// <param name="text">The JSON string to redact.</param>
    /// <returns>The redacted JSON string.</returns>
    public string RedactToString(string text)
    {
        var jsonObject = ParseObject(text);

        RedactJsonNode(jsonObject);

        return jsonObject.ToString();
    }

    // Deserialize hands back a null for the literal "null", which is valid JSON but
    // not a document there is anything to redact in
    private static JsonObject ParseObject(string text) =>
        JsonSerializer.Deserialize<JsonObject>(text)
        ?? throw new ArgumentException("The JSON is null rather than an object.", nameof(text));

    /// <summary>
    /// Walks through the JSON nodes once and applies the redaction based on regex matching.
    /// </summary>
    private void RedactJsonNode(JsonNode? node) =>
        RedactJsonNode(node, new RedactionPathBuilder());

    private void RedactJsonNode(JsonNode? node, RedactionPathBuilder path)
    {
        // The current node is null or the pattern is empty, no need to process further
        if (node == null || !_matcher.HasPatterns)
        {
            return;
        }

        // A node is redacted by its parent, below, which is the only place the
        // replacement can be written back into the document. The root has no
        // parent, so it is out of scope: a pattern matching the root's empty
        // path does not blank the whole document.

        // Recursively process child nodes (objects or arrays)
        if (node is JsonObject jObject)
        {
            // Left null until something matches, which for most objects is never
            List<string>? keysToRedact = null;

            foreach (var property in jObject)
            {
                int parentLength = path.Length;

                if (parentLength > 0)
                {
                    path.Append('.');
                }

                path.Append(property.Key);

                if (_matcher.MatchesAny(path.AsSpan()))
                {
                    // If the property key matches, add it to the list of keys to redact
                    keysToRedact ??= new List<string>();
                    keysToRedact.Add(property.Key);
                }
                else
                {
                    // Recursively redact child nodes
                    RedactJsonNode(property.Value, path);
                }

                path.TruncateTo(parentLength);
            }

            if (keysToRedact == null)
            {
                return;
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
                int parentLength = path.Length;

                path.AppendIndex(i);

                // Recursively redact array items
                RedactJsonNode(jArray[i], path);

                path.TruncateTo(parentLength);
            }
        }
    }


    private static JsonNode? GetDefaultValue(JsonNode? node)
    {
        // Default to null, empty string, or zero based on the node type
        return node switch
        {
            JsonValue jValue when jValue.TryGetValue(out string? _) => string.Empty,
            JsonValue jValue when jValue.TryGetValue(out int _) => 0,
            JsonValue jValue when jValue.TryGetValue(out bool _) => false,
            _ => null
        };
    }
}
