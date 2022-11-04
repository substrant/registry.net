using System;

namespace Substrant.Registry;

/// <summary>
///     Thrown when a <see cref="RegSerializer" /> throws an exception.
/// </summary>
public sealed class RegSerializeException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RegSerializeException" /> class.
    /// </summary>
    /// <param name="serializer">The serializer object.</param>
    /// <param name="inner">The exception that the serializer threw.</param>
    internal RegSerializeException(RegSerializer serializer, Exception inner) : base(
        serializer.GetType().Name + " threw an exception: " + inner, inner)
    {
    }
}