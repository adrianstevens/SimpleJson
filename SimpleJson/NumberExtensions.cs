using System;

namespace SimpleJsonSerializer;

internal enum NumberStyle
{
    Decimal = 1,
    Hexadecimal
}

internal static class UInt32Extensions
{
    public static bool TryParse(string str, NumberStyle style, out UInt32 result)
    {
        bool bresult = Helper.TryParseUInt64Core(str, style == NumberStyle.Hexadecimal ? true : false, out ulong tmp, out bool sign);
        result = (UInt32)tmp;

        return bresult && !sign;
    }
}

internal static class Int64Extensions
{
    public static long Parse(string str)
    {
        if (TryParse(str, out long result))
        {
            return result;
        }
        throw new Exception();
    }

    public static long Parse(string str, NumberStyle style)
    {
        if (style == NumberStyle.Hexadecimal)
        {
            return ParseHex(str);
        }

        return Parse(str);
    }

    public static bool TryParse(string str, out long result)
    {
        result = 0;

        if (Helper.TryParseUInt64Core(str, false, out ulong r, out bool sign))
        {
            if (!sign)
            {
                if (r <= 9223372036854775807)
                {
                    result = unchecked((long)r);
                    return true;
                }
            }
            else
            {
                if (r <= 9223372036854775808)
                {
                    result = unchecked(-((long)r));
                    return true;
                }
            }
        }
        return false;
    }

    private static long ParseHex(string str)
    {
        if (TryParseHex(str, out ulong result))
        {
            return (long)result;
        }
        throw new Exception();
    }

    private static bool TryParseHex(string str, out ulong result)
    {
        return Helper.TryParseUInt64Core(str, true, out result, out bool sign);
    }
}

internal static class CharExtensions
{
    /// <summary>
    /// Converts a Unicode character to a string of its ASCII equivalent.
    /// Very simple, it works only on ordinary characters.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static string ConvertFromUtf32(int p)
    {
        char c = (char)p;
        return c.ToString();
    }
}