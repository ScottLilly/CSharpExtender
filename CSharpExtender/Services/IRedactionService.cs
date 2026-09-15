using System;

namespace CSharpExtender.Services;

/// <summary>
/// Removes sensitive values from a document of type <typeparamref name="T"/>, for paths matching
/// any of a list of regex patterns.
/// </summary>
/// <typeparam name="T">The document type being redacted.</typeparam>
/// <remarks>
/// Every member rejects a null argument with an <see cref="ArgumentNullException"/>, so a caller
/// holding the interface gets the same answer whichever implementation is behind it.
/// </remarks>
public interface IRedactionService<T>
{
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    T Redact(T obj);

    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
    T Redact(string text);

    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    string RedactToString(T obj);

    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
    string RedactToString(string text);
}
