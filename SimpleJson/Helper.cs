using System;
using System.Globalization;

namespace SimpleJsonSerializer;

internal static class Helper
{
    public const int MaxDoubleDigits = 16;

    public static bool IsWhiteSpace(char ch)
    {
        return ch == ' ';
    }

    // Parse integer values using localized number format information.
    public static bool TryParseUInt64Core(string str, bool parseHex, out ulong result, out bool sign)
    {
        if (str == null)
        {
            throw new ArgumentNullException("str");
        }

        // If number contains the Hex '0x' prefix, then make sure we're
        // managing a Hex number, and skip over the '0x'
        if (str.Length >= 2 && str.Substring(0, 2).ToLower() == "0x")
        {
            str = str.Substring(2);
            parseHex = true;
        }

        char ch;
        bool noOverflow = true;
        result = 0;

        // Skip leading white space.
        int len = str.Length;
        int posn = 0;
        while (posn < len && IsWhiteSpace(str[posn]))
        {
            posn++;
        }

        // Check for leading sign information.
        NumberFormatInfo nfi = CultureInfo.CurrentUICulture.NumberFormat;
        string posSign = nfi.PositiveSign;
        string negSign = nfi.NegativeSign;
        sign = false;
        while (posn < len)
        {
            ch = str[posn];
            if (!parseHex && ch == negSign[0])
            {
                sign = true;
                ++posn;
            }
            else if (!parseHex && ch == posSign[0])
            {
                sign = false;
                ++posn;
            }
            /*      else if (ch == thousandsSep[0])
                    {
                        ++posn;
                    }*/
            else if ((parseHex && ((ch >= 'A' && ch <= 'F') || (ch >= 'a' && ch <= 'f'))) ||
                     (ch >= '0' && ch <= '9'))
            {
                break;
            }
            else
            {
                return false;
            }
        }

        // Bail out if the string is empty.
        if (posn >= len)
        {
            return false;
        }

        // Parse the main part of the number.
        uint low = 0;
        uint high = 0;
        uint digit;
        ulong tempa, tempb;
        if (parseHex)
        {
            #region Parse a hexadecimal value.
            do
            {
                // Get the next digit from the string.
                ch = str[posn];
                if (ch >= '0' && ch <= '9')
                {
                    digit = (uint)(ch - '0');
                }
                else if (ch >= 'A' && ch <= 'F')
                {
                    digit = (uint)(ch - 'A' + 10);
                }
                else if (ch >= 'a' && ch <= 'f')
                {
                    digit = (uint)(ch - 'a' + 10);
                }
                else
                {
                    break;
                }

                // Combine the digit with the result, and check for overflow.
                if (noOverflow)
                {
                    tempa = ((ulong)low) * ((ulong)16);
                    tempb = ((ulong)high) * ((ulong)16);
                    tempb += (tempa >> 32);
                    if (tempb > ((ulong)0xFFFFFFFF))
                    {
                        // Overflow has occurred.
                        noOverflow = false;
                    }
                    else
                    {
                        tempa = (tempa & 0xFFFFFFFF) + ((ulong)digit);
                        tempb += (tempa >> 32);
                        if (tempb > ((ulong)0xFFFFFFFF))
                        {
                            // Overflow has occurred.
                            noOverflow = false;
                        }
                        else
                        {
                            low = unchecked((uint)tempa);
                            high = unchecked((uint)tempb);
                        }
                    }
                }
                ++posn; // Advance to the next character.
            } while (posn < len);
            #endregion
        }
        else
        {
            #region Parse a decimal value.
            do
            {
                // Get the next digit from the string.
                ch = str[posn];
                if (ch >= '0' && ch <= '9')
                {
                    digit = (uint)(ch - '0');
                }
                /*       else if (ch == thousandsSep[0])
                       {
                           // Ignore thousands separators in the string.
                           ++posn;
                           continue;
                       }*/
                else
                {
                    break;
                }

                // Combine the digit with the result, and check for overflow.
                if (noOverflow)
                {
                    tempa = ((ulong)low) * ((ulong)10);
                    tempb = ((ulong)high) * ((ulong)10);
                    tempb += (tempa >> 32);
                    if (tempb > ((ulong)0xFFFFFFFF))
                    {
                        // Overflow has occurred.
                        noOverflow = false;
                    }
                    else
                    {
                        tempa = (tempa & 0xFFFFFFFF) + ((ulong)digit);
                        tempb += (tempa >> 32);
                        if (tempb > ((ulong)0xFFFFFFFF))
                        {
                            // Overflow has occurred.
                            noOverflow = false;
                        }
                        else
                        {
                            low = unchecked((uint)tempa);
                            high = unchecked((uint)tempb);
                        }
                    }
                }
                ++posn;// Advance to the next character.
            } while (posn < len);
            #endregion
        }

        // Process trailing white space.
        if (posn < len)
        {
            do
            {
                ch = str[posn];
                if (IsWhiteSpace(ch))
                    ++posn;
                else
                    break;
            } while (posn < len);

            if (posn < len)
            {
                return false;
            }
        }

        // Return the results to the caller.
        result = (((ulong)high) << 32) | ((ulong)low);
        return noOverflow;
    }
}