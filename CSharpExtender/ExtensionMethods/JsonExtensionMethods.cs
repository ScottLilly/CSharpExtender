using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSharpExtender.ExtensionMethods
{
    /// <summary>
    /// Scott's extension methods for JSON
    /// </summary>
    public static class JsonExtensionMethods
    {
        public static string GetValueFromJsonPath(this string json, string path) =>
            TryGetValueFromJsonPath(json, path, out string value) ? value
            : throw new InvalidOperationException($"JSON path '{path}' not found.");

        private static bool TryGetValueFromJsonPath(this string json, string path, out string value)
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

        public static string AsSerializedJson<T>(this T value) where T : class
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}