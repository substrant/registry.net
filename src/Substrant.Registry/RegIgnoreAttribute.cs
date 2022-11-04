using System;

namespace Substrant.Registry;

/// <summary>
///     <see cref="RegConvert" />'s methods will ignore properties that this attribute is applied to.
/// </summary>
public class RegIgnoreAttribute : Attribute
{
    /// <summary>
    ///     <see cref="RegConvert" />'s methods will ignore properties that this attribute is applied to.
    /// </summary>
    public RegIgnoreAttribute()
    {
    }
}