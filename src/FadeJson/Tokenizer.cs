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
        private static readonly string Terminal = WHITE_CHAR_LIST + KEYWORD_CHAR_LIST + EOF;
        private readonly ICommonCache<char, TextReader> cache;
        private const char EOF = unchecked((char)-1);

        public Tokenizer(ICommonCache<char, TextReader> cache) {
            this.cache = cache;
        }

        public JsonValue GetNextToken() {
            var c = cache.Lookahead().ToString();

            if (WHITE_CHAR_LIST.Contains(c)) {
                cache.Next();
                return GetNextToken();
            }
            if (cache.Check("//")) {
                GotoLineEnd();
                return GetNextToken();
            }

            if (KEYWORD_CHAR_LIST.Contains(c)) {
                cache.Next();
                return new JsonValue {
                    Value = c,
                    Type = JsonType.Symbol
                };
            }

            if ("tfn".Contains(c)) {
                return ParseKeywordToken();
            }

            if (c == "\"") {
                return ParseStringToken();
            }

            if (c.IsNumber()) {
                return ParseNumberToken();
            }

            return JsonValue.Null;
        }

        private JsonValue ParseStringToken() {
            cache.Next();
            var c = cache.Next();
            var list = new List<string>();
            while (true) {
                if (c == '\\') {
                    list.Add(ParseEscapeChar());
                    c = cache.Next();
                }
                else if (c == '\n') {
                    throw new FormatException("Hit NEWLINE in a string literal");
                }
                else if (c == '\"') {
                    break;
                }
                else {
                    list.Add(new string(c, 1));
                    c = cache.Next();
                }
            }

            return new JsonValue {
                Value = string.Concat(list),
                Type = JsonType.String
            };
        }

        private string ParseEscapeChar() {
            var escList = new[] { "n", "b", "f", "r", "t", "\"", "/", "\\" };
            var afterEsc = new[] { "\n", "\b", "\f", "\r", "\t", "\"", "/", "\\" };
            for (var i = 0; i < escList.Length; i++) {
                if (!cache.Check(escList[i])) continue;
                cache.Next();
                return afterEsc[i];
            }

            if (!cache.Check("u")) throw new FormatException();

            cache.Next();
            char[] t = new char[4];
            for (int i = 0; i < 4; i++) {
                t[i] = cache.Next();
            }
            return ((char)(int.Parse(string.Concat(t), NumberStyles.HexNumber))).ToString();
        }

        private JsonValue ParseKeywordToken() {
            string[] keywordList = { "true", "false", "null" };
            JsonValue[] valueList = { JsonValue.True, JsonValue.False, JsonValue.Null };
            for (var i = 0; i < keywordList.Length; i++) {
                var keyword = keywordList[i];
                if (!cache.Check(keyword)) continue;
                cache.Next(keyword.Length);
                return valueList[i];
            }
            throw new FormatException();
        }

        private JsonValue ParseNumberToken() {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(cache.Next());
            var isDouble = false;

            var c = cache.Lookahead();
            while (!WHITE_CHAR_LIST.Contains(c)) {
                if (cache.Lookahead() == '.') {
                    isDouble = true;
                }
                else if (!char.IsDigit(c)) {
                    break;
                }
                stringBuilder.Append(c);
                cache.Next();
                c = cache.Lookahead();
            }

            return new JsonValue {
                Value = stringBuilder.ToString(),
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
