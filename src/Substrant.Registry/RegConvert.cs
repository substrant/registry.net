using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace Substrant.Registry;

/// <summary>
///     Provides methods for serializing and deserializing objects to and from the registry.
/// </summary>
public static class RegConvert
{
    static readonly List<RegSerializer> Serializers = new();

    static readonly object NonExistentId = Guid.NewGuid();

    static RegConvert()
    {
        UseSerializer(new RegSerializer(typeof(string))
        {
            Deserialize = (value, kind) =>
            {
                if (kind is not RegistryValueKind.String)
                    throw new ArgumentException("Invalid registry value kind");

                return value;
            },
            Serialize = (object value, out RegistryValueKind kind) =>
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value), "Cannot serialize a null string");

                kind = RegistryValueKind.String;
                return value.ToString();
            }
        });

        UseSerializer(new RegSerializer(typeof(bool))
        {
            Deserialize = (value, kind) =>
            {
                if (kind is not RegistryValueKind.DWord)
                    throw new ArgumentException("Invalid registry value kind");

                return (int)value != 0;
            },
            Serialize = (object value, out RegistryValueKind kind) =>
            {
                kind = RegistryValueKind.DWord;
                return (bool)value ? 1 : 0;
            }
        });

        UseSerializer(new RegSerializer(typeof(int))
        {
            Deserialize = (value, kind) =>
            {
                if (kind is not RegistryValueKind.DWord)
                    throw new ArgumentException("Invalid registry value kind");

                return (int)value;
            },
            Serialize = (object value, out RegistryValueKind kind) =>
            {
                kind = RegistryValueKind.DWord;
                return (int)value;
            }
        });

        UseSerializer(new RegSerializer(typeof(uint))
        {
            Deserialize = (value, kind) =>
            {
                if (kind is not RegistryValueKind.DWord)
                    throw new ArgumentException("Invalid registry value kind");

                return (uint)value;
            },
            Serialize = (object value, out RegistryValueKind kind) =>
            {
                kind = RegistryValueKind.DWord;
                return (uint)value;
            }
        });

        UseSerializer(new RegSerializer(typeof(long))
        {
            Deserialize = (value, kind) =>
            {
                if (kind is not RegistryValueKind.QWord)
                    throw new ArgumentException("Invalid registry value kind");

                return (long)value;
            },
            Serialize = (object value, out RegistryValueKind kind) =>
            {
                kind = RegistryValueKind.QWord;
                return (long)value;
            }
        });

        UseSerializer(new RegSerializer(typeof(ulong))
        {
            Deserialize = (value, kind) =>
            {
                if (kind is not RegistryValueKind.QWord)
                    throw new ArgumentException("Invalid registry value kind");

                return (ulong)value;
            },
            Serialize = (object value, out RegistryValueKind kind) =>
            {
                kind = RegistryValueKind.QWord;
                return (ulong)value;
            }
        });

        UseSerializer(new RegSerializer(typeof(double))
        {
            Deserialize = (value, kind) =>
            {
                if (kind is not RegistryValueKind.QWord)
                    throw new ArgumentException("Invalid registry value kind");

                return BitConverter.Int64BitsToDouble((long)value);
            },
            Serialize = (object value, out RegistryValueKind kind) =>
            {
                kind = RegistryValueKind.QWord;
                return BitConverter.DoubleToInt64Bits((double)value);
            }
        });
    }

    /// <summary>
    ///     Use a custom type serializer.
    /// </summary>
    /// <param name="serializer">The serializer instance.</param>
    public static void UseSerializer(RegSerializer serializer)
    {
        Serializers.Add(serializer);
    }

    /// <summary>
    ///     Deserializes an object from a <see cref="RegistryKey" />.
    /// </summary>
    /// <param name="key">The source registry key.</param>
    /// <param name="type">The type of the object.</param>
    /// <exception cref="RegSerializeException">Thrown when a serializer throws an exception.</exception>
    public static object DeserializeObject(RegistryKey key, Type type)
    {
        var obj = Activator.CreateInstance(type);

        foreach (var prop in type.GetProperties())
        {
            if (!prop.CanWrite)
                continue;

            if (prop.GetCustomAttributes(typeof(RegIgnoreAttribute), true).Length > 0)
                continue;

            var attr = (RegPropertyAttribute)prop.GetCustomAttributes(typeof(RegPropertyAttribute), true)
                .FirstOrDefault() ?? RegPropertyAttribute.Default;
            var name = attr.Name ?? prop.Name;

            var serializer = Serializers.FirstOrDefault(e => e.PropertyType == prop.PropertyType);
            if (serializer?.Deserialize is null)
                throw new Exception(
                    $"There is no serializer given that corresponds to property '{prop.PropertyType.Name}' of type '{type.Name}'");

            var value = key.GetValue(name, NonExistentId);
            if (value == NonExistentId)
                continue;

            prop.SetValue(obj, serializer.Deserialize(value, key.GetValueKind(name)));
        }

        return obj;
    }

    /// <summary>
    ///     Deserializes an object from a <see cref="RegistryKey" />.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="key">The source registry key.</param>
    /// <exception cref="RegSerializeException">Thrown when a serializer throws an exception.</exception>
    public static T DeserializeObject<T>(RegistryKey key)
    {
        return (T)DeserializeObject(key, typeof(T));
    }

    /// <summary>
    ///     Serializes an object to a <see cref="RegistryKey" />.
    /// </summary>
    /// <typeparam name="T">The type of the given object.</typeparam>
    /// <param name="key">The destination registry key.</param>
    /// <param name="obj">The object to serialize.</param>
    /// <exception cref="RegSerializeException">Thrown when a serializer throws an exception.</exception>
    public static void SerializeObject<T>(RegistryKey key, T obj)
    {
        var type = obj.GetType();

        foreach (var prop in type.GetProperties())
        {
            if (!prop.CanRead)
                continue;

            if (prop.GetCustomAttributes(typeof(RegIgnoreAttribute), true).Length > 0)
                continue;

            var attr = (RegPropertyAttribute)prop.GetCustomAttributes(typeof(RegPropertyAttribute), true)
                .FirstOrDefault() ?? RegPropertyAttribute.Default;
            var name = attr.Name ?? prop.Name;

            var serializer = Serializers.FirstOrDefault(e => e.PropertyType == prop.PropertyType);
            if (serializer?.Serialize is null)
                throw new Exception(
                    $"There is no serializer given that corresponds to property '{prop.PropertyType.Name}' of type '{type.Name}'");

            try
            {
                key.SetValue(name, serializer.Serialize(prop.GetValue(obj), out var kind), kind);
            }
            catch (Exception ex)
            {
                throw new RegSerializeException(serializer, ex);
            }
        }
    }
}