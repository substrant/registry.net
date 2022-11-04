using System;
using Microsoft.Win32;

namespace Substrant.Registry;

/// <summary>
///     This attribute allows you to explicitly define the name and type of registry value that corresponds to the
///     property.
/// </summary>
public class RegPropertyAttribute : Attribute
{
    internal static RegPropertyAttribute Default = new();

    /// <summary>
    ///     The type of registry value to (de)serialize.
    /// </summary>
    public readonly RegistryValueKind? Kind;

    /// <summary>
    ///     The name stored in the registry key.
    /// </summary>
    public readonly string Name;

    /// <summary>
    ///     This attribute allows you to explicitly define the name and type of registry value that corresponds to the
    ///     property.
    /// </summary>
    public RegPropertyAttribute()
    {
    }

    /// <summary>
    ///     This attribute allows you to explicitly define the name and type of registry value that corresponds to the
    ///     property.
    /// </summary>
    /// <param name="name">The name of the registry value.</param>
    public RegPropertyAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    ///     This attribute allows you to explicitly define the name and type of registry value that corresponds to the
    ///     property.
    /// </summary>
    /// <param name="name">The name of the registry value.</param>
    /// <param name="kind">The type of registry value.</param>
    public RegPropertyAttribute(string name, RegistryValueKind kind)
    {
        Name = name;
        Kind = kind;
    }
}