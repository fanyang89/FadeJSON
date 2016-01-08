//#define FEATURE_COMMENT_SUPPORT

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace FadeJson
{
    public class Tokenizer
    {
        private int LinePosition = 1;

        private const char EOF = unchecked((char)-1);
        private readonly char[] afterEsc = { '\n', '\b', '\f', '\r', '\t', '\'', '/', '\\' };
        private readonly ICommonCache<char, TextReader> cache;

        private readonly char[] defaultNumberCache = new char[32];
        private readonly StringBuilder defaultStringBuilder = new StringBuilder(64);

        private readonly char[] defaultUnicodeCache = new char[4];
        private readonly string[] escapeCharFollowingList = { "n", "b", "f", "r", "t", "\"", "/", "\\" };

        private readonly string[] keywordList = { "true", "false", "null" };
        private readonly JsonValue[] keywordValueList = { JsonValue.True, JsonValue.False, JsonValue.Null };

        public Tokenizer(ICommonCache<char, TextReader> cache) {
            this.cache = cache;
        }

        private static bool IsWhiteChar(char c) {
            return c == ' ' || c == '\n' || c == '\r' || c == '\t';
        }

        private static bool IsKeyChar(char c) {
            return c == ':' || c == ',' || c == '{' || c == '[' || c == ']' || c == '}';
        }

        public JsonValue GetNextToken() {
            while (true) {
                var c = cache.Lookahead();

#if FEATURE_COMMENT_SURPPORT
                if (cache.Check("//")) {
                    GotoLineEnd();
                    continue;
                }
#endif
                switch (c) {
                    case ' ':
                    case '\t':
                    case '\r':
                        cache.Next();
                        continue;
                    case '\n':
                        LinePosition++;
                        cache.Next();
                        continue;
                    case ':':
                        cache.Next();
                        return JsonValue.Colon;
                    case ',':
                        cache.Next();
                        return JsonValue.Comma;
                    case '{':
                        cache.Next();
                        return JsonValue.LeftBrace;
                    case '}':
                        cache.Next();
                        return JsonValue.RightBrace;
                    case '[':
                        cache.Next();
                        return JsonValue.LeftBracket;
                    case ']':
                        cache.Next();
                        return JsonValue.RightBracket;
                    case 't':
                    case 'n':
                    case 'f':
                        return ParseKeywordToken();
                    case '\"':
                        return ParseStringToken();
                    case '-':
                        return ParseNumberToken();
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        return ParseNumberToken();
                    default:
                        return JsonValue.Null;
                }
            }
        }

        private JsonValue ParseStringToken() {
            cache.Next(); // skip the first "
            var c = cache.Next();

            while (true) {
                if (c == '\n') {
                    throw new FormatException("Hit NEWLINE in a string literal");
                }
                if (c == '\"') {
                    break;
                }
                defaultStringBuilder.Append(c == '\\' ? ParseEscapeChar() : c);
                c = cache.Next();
            }
            var j = new JsonValue(JsonType.String) {
                Value = defaultStringBuilder.ToString()
            };
#if NET35
            defaultStringBuilder.Length = 0;
#else
            defaultStringBuilder.Clear();
#endif
            return j;
        }

        private char ParseEscapeChar() {
            for (var i = 0; i < escapeCharFollowingList.Length; i++) {
                if (!cache.Check(escapeCharFollowingList[i])) continue;
                cache.Next();
                return afterEsc[i];
            }

            if (!cache.Check("u")) throw new FormatException();

            //unicode char processing
            cache.Next();
            for (var i = 0; i < 4; i++) {
                defaultUnicodeCache[i] = cache.Next();
            }
            return (char)int.Parse(string.Concat(defaultUnicodeCache), NumberStyles.HexNumber);
        }

        private JsonValue ParseKeywordToken() {
            for (var i = 0; i < keywordList.Length; i++) {
                var keyword = keywordList[i];
                if (!cache.Check(keyword)) continue;
                cache.Next(keyword.Length);
                return keywordValueList[i];
            }
            throw new FormatException();
        }

        private JsonValue ParseNumberToken() {
            var pos = 1;
            defaultNumberCache[0] = cache.Next();
            defaultNumberCache[1] = '\0';

            var isDouble = false;

            var c = cache.Lookahead();
            while (!IsWhiteChar(c) && !IsKeyChar(c)) {
                if (cache.Lookahead() == '.') {
                    isDouble = true;
                }
                else if (!char.IsDigit(c)) {
                    break;
                }
                defaultNumberCache[pos++] = c;
                cache.Next();
                c = cache.Lookahead();
            }

            return new JsonValue {
                Value = new string(defaultNumberCache, 0, pos),
                Type = isDouble ? JsonType.Double : JsonType.Int32
            };
        }

        private void GotoLineEnd() {
            while (cache.Lookahead() != '\n') {
                cache.Next();
            }
        }
    }
}