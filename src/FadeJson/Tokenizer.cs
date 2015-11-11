using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FadeJson
{
    public class Tokenizer
    {
        const string WHITE_CHAR_LIST = " \r\t\n";
        private const string KEYWORD_CHAR_LIST = ":,{}[]";
        private readonly JsonValue[] keywordValueList ={
            JsonValue.Colon,
            JsonValue.Comma,
            JsonValue.LeftBrace,
            JsonValue.RightBrace,
            JsonValue.LeftBracket,
            JsonValue.RightBracket
        };
        private readonly ICommonCache<char, TextReader> cache;
        private const char EOF = unchecked((char)-1);

        public Tokenizer(ICommonCache<char, TextReader> cache) {
            this.cache = cache;
        }

        public JsonValue GetNextToken() {
            var c = cache.Lookahead();

            if (WHITE_CHAR_LIST.Contains(c)) {
                cache.Next();
                return GetNextToken();
            }

            if (cache.Check("//")) {
                GotoLineEnd();
                return GetNextToken();
            }

            var keywordIndex = KEYWORD_CHAR_LIST.IndexOf(c);
            if (keywordIndex != -1) {
                cache.Next();
                return keywordValueList[keywordIndex];
            }

            if ("tfn".Contains(c)) {
                return ParseKeywordToken();
            }

            if (c == '\"') {
                return ParseStringToken();
            }

            if (c.IsNumber() || c == '-') {
                return ParseNumberToken();
            }

            return JsonValue.Null;
        }

        readonly StringBuilder sb = new StringBuilder(64);

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
            var j = new JsonValue(JsonType.String) { Value = sb.ToString() };
            sb.Clear();
            return j;
        }

        private readonly char[] unicodeCache = new char[4];
        private readonly string[] escList = { "n", "b", "f", "r", "t", "\"", "/", "\\" };
        private readonly char[] afterEsc = { '\n', '\b', '\f', '\r', '\t', '\'', '/', '\\' };

        private char ParseEscapeChar() {
            for (var i = 0; i < escList.Length; i++) {
                if (!cache.Check(escList[i])) continue;
                cache.Next();
                return afterEsc[i];
            }

            if (!cache.Check("u")) throw new FormatException();

            cache.Next();
            for (int i = 0; i < 4; i++) {
                unicodeCache[i] = cache.Next();
            }
            return (char)(int.Parse(string.Concat(unicodeCache), NumberStyles.HexNumber));
        }

        private readonly string[] keywordList = { "true", "false", "null" };
        private readonly JsonValue[] valueList = { JsonValue.True, JsonValue.False, JsonValue.Null };

        private JsonValue ParseKeywordToken() {
            for (var i = 0; i < keywordList.Length; i++) {
                var keyword = keywordList[i];
                if (!cache.Check(keyword)) continue;
                cache.Next(keyword.Length);
                return valueList[i];
            }
            throw new FormatException();
        }

        readonly char[] number = new char[32];
        private JsonValue ParseNumberToken() {
            int pos = 0;
            number[0] = cache.Next();
            var isDouble = false;

            var c = cache.Lookahead();
            while (!WHITE_CHAR_LIST.Contains(c)) {
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
                Value = string.Concat(number),
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
