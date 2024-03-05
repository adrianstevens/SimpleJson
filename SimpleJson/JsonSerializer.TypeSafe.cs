using System;
using System.Collections;
using System.Linq;

namespace SimpleJsonSerializer;

public partial class JsonSerializer
{
    public static T[] DeserializeArray<T>(ArrayList array)
        where T : new()
    {
        var result = new T[array.Count];
        var index = 0;

        foreach (string item in array)
        {
            result[index++] = Deserialize<T>(item);
        }

        return result;
    }

    private static void DeserializeArray(ArrayList array, Type type, ref Array instance)
    {
        var index = 0;

        foreach (Hashtable item in array)
        {
            if (type == typeof(string))
            {
                var e = item.GetEnumerator();
                e.MoveNext();
                instance.SetValue(((DictionaryEntry)e.Current).Value, index);
                index++;
            }
            else
            {
                var arrayItem = Activator.CreateInstance(type);
                Deserialize(item, type, ref arrayItem);
                instance.SetValue(arrayItem, index++);
            }
        }
    }

    public static T Deserialize<T>(string json)
    {
        var type = typeof(T);
        if (type.IsArray)
        {
            var etype = type.GetElementType();

            var rootArray = DeserializeString(json) as ArrayList;
            var targetArray = Array.CreateInstance(etype, rootArray.Count);

            var index = 0;
            foreach (Hashtable item in rootArray)
            {
                object einstance = Activator.CreateInstance(etype);
                Deserialize(item, etype, ref einstance);
                targetArray.SetValue(einstance, index++);
            }
            return (T)targetArray.Clone();
        }
        else
        {
            object instance = Activator.CreateInstance(type);
            Deserialize(json, typeof(T), ref instance);

            return (T)instance;
        }
        //            object instance = new T(); // <-- MUST be declared as object to box potential structs

    }

    private static void Deserialize(string json, Type type, ref object instance)
    {
        var root = DeserializeString(json) as Hashtable;

        Deserialize(root, type, ref instance);
    }

    private static void Deserialize(Hashtable root, Type type, ref object instance)
    {
        var values = root ?? throw new ArgumentException();

        var props = type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).ToList();

        foreach (string v in values.Keys)
        {
            var prop = props.FirstOrDefault(p => string.Compare(p.Name, v, true) == 0);

            if (prop != null && prop.CanWrite)
            {
                switch (true)
                {
                    case bool _ when prop.PropertyType.IsEnum:
                        var enumValue = Enum.Parse(prop.PropertyType, values[v].ToString());
                        prop.SetValue(instance, enumValue);
                        break;
                    case bool _ when prop.PropertyType == typeof(ulong):
                        prop.SetValue(instance, Convert.ToUInt64(values[v]));
                        break;
                    case bool _ when prop.PropertyType == typeof(long):
                        prop.SetValue(instance, Convert.ToInt64(values[v]));
                        break;
                    case bool _ when prop.PropertyType == typeof(uint):
                        prop.SetValue(instance, Convert.ToUInt32(values[v]));
                        break;
                    case bool _ when prop.PropertyType == typeof(int):
                        prop.SetValue(instance, Convert.ToInt32(values[v]));
                        break;
                    case bool _ when prop.PropertyType == typeof(ushort):
                        prop.SetValue(instance, Convert.ToUInt16(values[v]));
                        break;
                    case bool _ when prop.PropertyType == typeof(short):
                        prop.SetValue(instance, Convert.ToInt16(values[v]));
                        break;
                    case bool _ when prop.PropertyType == typeof(byte):
                        prop.SetValue(instance, Convert.ToByte(values[v]));
                        break;
                    case bool _ when prop.PropertyType == typeof(sbyte):
                        prop.SetValue(instance, Convert.ToBoolean(values[v]));
                        break;
                    case bool _ when prop.PropertyType == typeof(double):
                        prop.SetValue(instance, Convert.ToDouble(values[v]));
                        break;
                    case bool _ when prop.PropertyType == typeof(float):
                        prop.SetValue(instance, Convert.ToSingle(values[v]));
                        break;
                    case bool _ when prop.PropertyType == typeof(bool):
                        prop.SetValue(instance, Convert.ToBoolean(values[v]));
                        break;
                    case bool _ when prop.PropertyType == typeof(string):
                        prop.SetValue(instance, values[v].ToString());
                        break;
                    default:
                        if (prop.PropertyType.IsArray)
                        {
                            var al = values[v] as ArrayList;
                            var elementType = prop.PropertyType.GetElementType();
                            var targetArray = Array.CreateInstance(elementType, al.Count);
                            DeserializeArray(al, elementType, ref targetArray);
                            prop.SetValue(instance, targetArray);
                        }
                        else
                        {
                            throw new NotSupportedException($"Type '{prop.PropertyType}' not supported");
                        }
                        break;
                }
            }
        }
    }
}