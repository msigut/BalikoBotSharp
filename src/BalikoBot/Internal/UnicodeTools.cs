using System;

namespace BalikoBot.Internal
{
    [Flags]
    public enum UnicodeEscapeConvention
    {
        /// <summary>
        /// C syntax (utf-16)
        /// </summary>
        C16 = 1,
        /// <summary>
        /// C syntax (utf-32)
        /// </summary>
        C32 = 2,
    }

    public class UnicodeTools
    {
        private const int HIGH_SURROGATE_START = 0xd800;
        private const int LOW_SURROGATE_START = 0xdc00;
        private const int UNICODE_PLANE01_START = 0x10000;

        private static int GetHexValue(char ch)
        {
            switch (ch)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;

                case 'a':
                case 'A':
                    return 0xa;

                case 'b':
                case 'B':
                    return 0xB;

                case 'c':
                case 'C':
                    return 0xC;

                case 'd':
                case 'D':
                    return 0xD;

                case 'e':
                case 'E':
                    return 0xE;

                case 'f':
                case 'F':
                    return 0xF;

                default:
                    return -1;
            }
        }

        private static char GetHexDigit(int value)
        {
            switch (value)
            {
                case 0: return '0';
                case 1: return '1';
                case 2: return '2';
                case 3: return '3';
                case 4: return '4';
                case 5: return '5';
                case 6: return '6';
                case 7: return '7';
                case 8: return '8';
                case 9: return '9';
                case 0xa: return 'a';
                case 0xb: return 'b';
                case 0xc: return 'c';
                case 0xd: return 'd';
                case 0xe: return 'e';
                case 0xf: return 'f';

                default:
                    throw new InvalidOperationException("Invalid value");
            }
        }

        private static bool TryParseHexInt32(string source, int index, int length, out int result)
        {
            result = 0;

            for (var i = 0; i < length; i++)
            {
                var ch = source[index + i];

                var value = GetHexValue(ch);
                if (value == -1)
                {
                    return false;
                }

                result += value << ((length - 1 - i) * 4);
            }

            return true;
        }

        /// <summary>
        /// Escape all unicode characters using C convention \uNNNN
        /// </summary>
        public static unsafe string Escape(string value)
        {
            var escapedLength = 0;

            for (var i = 0; i < value.Length; i++)
            {
                var ch = value[i];

                if (ch < 127)
                {
                    escapedLength += 1;
                }
                else
                {
                    escapedLength += 6;
                }
            }

            var result = new string('\0', escapedLength);

            fixed (char* pResult = result)
            {
                var resultIndex = 0;

                for (var i = 0; i < value.Length; i++)
                {
                    var ch = value[i];

                    if (ch < 127)
                    {
                        pResult[resultIndex++] = ch;
                    }
                    else
                    {
                        var code = (ushort)ch;

                        pResult[resultIndex++] = '\\';
                        pResult[resultIndex++] = 'u';
                        pResult[resultIndex++] = GetHexDigit((code >> 12) & 0xf);
                        pResult[resultIndex++] = GetHexDigit((code >> 8) & 0xf);
                        pResult[resultIndex++] = GetHexDigit((code >> 4) & 0xf);
                        pResult[resultIndex++] = GetHexDigit(code & 0xf);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Unescape all unicode characters using specified conventions.
        /// </summary>
        public static string Unescape(string value, UnicodeEscapeConvention options = UnicodeEscapeConvention.C16 | UnicodeEscapeConvention.C32)
        {
            var result = new char[value.Length];
            var resultLength = 0;

            for (var i = 0; i < value.Length; i++)
            {
                var ch = value[i];

                // remainder of input string not including current character
                var remainder = value.Length - (i + 1);

                switch (ch)
                {
                    // \uNNNN
                    case '\\' when (options & UnicodeEscapeConvention.C16) == UnicodeEscapeConvention.C16 &&
                            remainder >= 5 &&
                            value[i + 1] == 'u' &&
                            TryParseHexInt32(value, i + 2, 4, out var code):

                        result[resultLength++] = (char)code;
                        i += 5;
                        break;

                    // \UNNNNNNNN
                    case '\\' when (options & UnicodeEscapeConvention.C32) == UnicodeEscapeConvention.C32 &&
                            remainder >= 9 &&
                            value[i + 1] == 'U' &&
                            TryParseHexInt32(value, i + 2, 8, out var code):

                        if (code < UNICODE_PLANE01_START)
                        {
                            result[resultLength++] = (char)code;
                        }
                        else
                        {
                            code -= UNICODE_PLANE01_START;

                            result[resultLength++] = (char)((code / 0x400) + HIGH_SURROGATE_START);
                            result[resultLength++] = (char)((code % 0x400) + LOW_SURROGATE_START);
                        }

                        i += 9;
                        break;

                    // no escaping found
                    default:
                        result[resultLength++] = ch;
                        break;
                }
            }

            return new string(result, 0, resultLength);
        }
    }
}
