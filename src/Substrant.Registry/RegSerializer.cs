using System;
using Microsoft.Win32;

namespace Substrant.Registry;

/// <summary>
///     Contains (de)serialization methods for registry values.
/// </summary>
public class RegSerializer
{
    /// <summary>
    ///     Deserializes the given value from the registry.
    /// </summary>
    /// <param name="value">The registry value.</param>
    /// <param name="kind">The type of registry value.</param>
    /// <returns>The deserialized value.</returns>
    public delegate object DeserializeCallback(object value, RegistryValueKind kind);

    /// <summary>
    ///     Serializes the given value to be stored in the registry.
    /// </summary>
    /// <param name="value">The property value.</param>
    /// <param name="kind">The type of registry value.</param>
    /// <returns>The serialized value.</returns>
    public delegate object SerializeCallback(object value, out RegistryValueKind kind);

    internal DeserializeCallback Deserialize;

    internal Type PropertyType;
    internal SerializeCallback Serialize;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RegSerializer" /> class.
    /// </summary>
    /// <param name="propertyType">The property type to bind to.</param>
    public RegSerializer(Type propertyType)
    {
        PropertyType = propertyType;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RegSerializer" /> class.
    /// </summary>
    /// <param name="propertyType">The property type to bind to.</param>
    /// <param name="serializeCallback">The serialization callback.</param>
    /// <param name="deserializeCallback">The deserialization callback.</param>
    public RegSerializer(Type propertyType, SerializeCallback serializeCallback,
        DeserializeCallback deserializeCallback)
    {
        PropertyType = propertyType;
        Serialize = serializeCallback;
        Deserialize = deserializeCallback;
    }
}