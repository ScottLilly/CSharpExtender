using System;

namespace CSharpExtender.Services;

/// <summary>
/// Builds the path to the node a redaction service is looking at, in one buffer
/// that is reused for the whole document.
/// </summary>
/// <remarks>
/// A path is only ever extended at its end, so walking back up to a parent is a
/// matter of setting the length. The buffer is handed to the regex as a span, so
/// no part of a path is ever turned into a string.
/// Holds the state of a single document walk, so an instance is not shared and is
/// not thread-safe.
/// </remarks>
internal sealed class RedactionPathBuilder
{
    private char[] _buffer = new char[256];

    internal int Length { get; private set; }

    internal ReadOnlySpan<char> AsSpan() => _buffer.AsSpan(0, Length);

    internal void Append(char value)
    {
        EnsureRoom(1);

        _buffer[Length++] = value;
    }

    internal void Append(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        EnsureRoom(value.Length);

        value.AsSpan().CopyTo(_buffer.AsSpan(Length));

        Length += value.Length;
    }

    /// <summary>
    /// Appends an array index, as "[0]", without turning the number into a string.
    /// </summary>
    internal void AppendIndex(int index)
    {
        // Two brackets, and enough room for int.MinValue
        EnsureRoom(13);

        _buffer[Length++] = '[';

        index.TryFormat(_buffer.AsSpan(Length), out int written);

        Length += written;

        _buffer[Length++] = ']';
    }

    /// <summary>
    /// Drops everything after the given length, which is how the walk steps back
    /// up to a parent.
    /// </summary>
    internal void TruncateTo(int length) => Length = length;

    private void EnsureRoom(int additional)
    {
        if (Length + additional <= _buffer.Length)
        {
            return;
        }

        int capacity = _buffer.Length * 2;

        while (capacity < Length + additional)
        {
            capacity *= 2;
        }

        Array.Resize(ref _buffer, capacity);
    }
}
