//#define FEATURE_COMMENT_SURPPORT

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace FadeJson
{
    public class Tokenizer
    {
        private const char EOF = unchecked((char) -1);
        private readonly char[] afterEsc = {'\n', '\b', '\f', '\r', '\t', '\'', '/', '\\'};
        private readonly ICommonCache<char, TextReader> cache;
        private readonly string[] escList = {"n", "b", "f", "r", "t", "\"", "/", "\\"};

        private readonly string[] keywordList = {"true", "false", "null"};

        private readonly char[] number = new char[32];

        private readonly StringBuilder sb = new StringBuilder(64);

        private readonly char[] unicodeCache = new char[4];
        private readonly JsonValue[] valueList = {JsonValue.True, JsonValue.False, JsonValue.Null};

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
                    case '\n':
                    case '\r':
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
            cache.Next();
            var c = cache.Next();
            while (true) {
                if (c == '\\') {
                    sb.Append(ParseEscapeChar());
                    c = cache.Next();
                }
                else if (c == '\n') {
                    throw new FormatException("Hit NEWLINE in a string literal");
                }
                else if (c == '\"') {
                    break;
                }
                else {
                    sb.Append(c);
                    c = cache.Next();
                }
            }
            var j = new JsonValue(JsonType.String) {Value = sb.ToString()};
            sb.Clear();
            return j;
        }

        private char ParseEscapeChar() {
            for (var i = 0; i < escList.Length; i++) {
                if (!cache.Check(escList[i])) continue;
                cache.Next();
                return afterEsc[i];
            }

            if (!cache.Check("u")) throw new FormatException();

            cache.Next();
            for (var i = 0; i < 4; i++) {
                unicodeCache[i] = cache.Next();
            }
            return (char) (int.Parse(string.Concat(unicodeCache), NumberStyles.HexNumber));
        }

        private JsonValue ParseKeywordToken() {
            for (var i = 0; i < keywordList.Length; i++) {
                var keyword = keywordList[i];
                if (!cache.Check(keyword)) continue;
                cache.Next(keyword.Length);
                return valueList[i];
            }
            throw new FormatException();
        }

        private JsonValue ParseNumberToken() {
            var pos = 1;
            number[0] = cache.Next();
            number[1] = '\0';

            var isDouble = false;

            var c = cache.Lookahead();
            while (!IsWhiteChar(c) && !IsKeyChar(c)) {
                if (cache.Lookahead() == '.') {
                    isDouble = true;
                }
                else if (!char.IsDigit(c)) {
                    break;
                }
                number[pos++] = c;
                cache.Next();
                c = cache.Lookahead();
            }

            return new JsonValue {
                Value = new string(number),
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