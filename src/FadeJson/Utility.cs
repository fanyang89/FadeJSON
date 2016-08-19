using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FadeJSON
{
    public static class Utility
    {
        public static int LineNumber = 1;

        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddLineNumber(char c) {
            if (c == '\n') {
                LineNumber++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhiteChar(this char c) {
            AddLineNumber(c);
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDigit(this char c) {
            return '0' <= c && c <= '9';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDigit1_9(this char c) {
            return '1' <= c && c <= '9';
        }
    }
}