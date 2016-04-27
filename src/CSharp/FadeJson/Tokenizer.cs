//#define FEATURE_COMMENT_SUPPORT

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace FadeJson
{
    public class Tokenizer
    {
        private const char Eof = '\uffff';

        private readonly char[] _afterEsc = {
            '\n', '\b', '\f', '\r', '\t', '\'', '/', '\\'
        };

        private readonly Cache<TextReader, char> _cache;
        private readonly char[] _defaultNumberCache = new char[32];
        private readonly StringBuilder _defaultStringBuilder = new StringBuilder(64);
        private readonly char[] _defaultUnicodeCache = new char[4];
        private readonly string[] _escapeCharFollowingList = {
            "n", "b", "f", "r", "t", "\"", "/", "\\"
        };
        private readonly string[] _keywordList = {
            "true", "false", "null"
        };
        private readonly JsonValue[] _keywordValueList = {
            JsonValue.True, JsonValue.False, JsonValue.Null
        };
        private int _linePosition = 1;

        public Tokenizer(Cache<TextReader, char> cache) {
            this._cache = cache;
        }

        public JsonValue GetNextToken() {
            while (true) {
                var c = _cache.Peek();

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
                        _cache.Consume();
                        continue;
                    case '\n':
                        _linePosition++;
                        _cache.Consume();
                        continue;
                    case ':':
                        _cache.Consume();
                        return JsonValue.Colon;

                    case ',':
                        _cache.Consume();
                        return JsonValue.Comma;

                    case '{':
                        _cache.Consume();
                        return JsonValue.LeftBrace;

                    case '}':
                        _cache.Consume();
                        return JsonValue.RightBrace;

                    case '[':
                        _cache.Consume();
                        return JsonValue.LeftBracket;

                    case ']':
                        _cache.Consume();
                        return JsonValue.RightBracket;

                    case 't':
                    case 'n':
                    case 'f':
                        return ParseKeywordToken();

                    case '\"':
                        return ParseStringToken();

                    case '-':
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
                        return JsonValue.Eof;
                }
            }
        }

        private static bool IsKeyChar(char c) {
            return c == ':' || c == ',' || c == '{' || c == '[' || c == ']' || c == '}';
        }

        private static bool IsWhiteChar(char c) {
            return c == ' ' || c == '\n' || c == '\r' || c == '\t';
        }

        private void GotoLineEnd() {
            while (_cache.Peek() != '\n') {
                _cache.Consume();
            }
        }

        private char ParseEscapeChar() {
            for (var i = 0; i < _escapeCharFollowingList.Length; i++) {
                if (!_cache.CheckLookaheads(_escapeCharFollowingList[i])) continue;
                _cache.Consume();
                return _afterEsc[i];
            }

            if (!_cache.CheckLookaheads("u")) throw new FormatException();

            //unicode char processing
            _cache.Consume();
            for (var i = 0; i < 4; i++) {
                _defaultUnicodeCache[i] = _cache.Consume();
            }
            return (char)int.Parse(string.Concat(_defaultUnicodeCache), NumberStyles.HexNumber);
        }

        private JsonValue ParseKeywordToken() {
            for (var i = 0; i < _keywordList.Length; i++) {
                var keyword = _keywordList[i];
                if (!_cache.CheckLookaheads(keyword)) continue;
                _cache.Consume(keyword.Length);
                return _keywordValueList[i];
            }
            throw new FormatException();
        }

        private JsonValue ParseNumberToken() {
            var pos = 1;
            _defaultNumberCache[0] = _cache.Consume();
            _defaultNumberCache[1] = '\0';

            var isDouble = false;

            var c = _cache.Peek();
            while (!IsWhiteChar(c) && !IsKeyChar(c)) {
                if (_cache.Peek() == '.') {
                    isDouble = true;
                } else if (!char.IsDigit(c)) {
                    break;
                }
                _defaultNumberCache[pos++] = c;
                _cache.Consume();
                c = _cache.Peek();
            }

            return new JsonValue(isDouble ? JsonType.Double : JsonType.Int32) {
                Value = new string(_defaultNumberCache, 0, pos)
            };
        }

        private JsonValue ParseStringToken() {
            _cache.Consume(); // skip the first "
            var c = _cache.Consume();

            while (true) {
                if (c == '\n') {
                    throw new FormatException("Hit NEWLINE in a string literal");
                }
                if (c == '\"') {
                    break;
                }
                _defaultStringBuilder.Append(c == '\\' ? ParseEscapeChar() : c);
                c = _cache.Consume();
            }
            var j = new JsonValue(JsonType.String) {
                Value = _defaultStringBuilder.ToString()
            };
#if NET35
            _defaultStringBuilder.Length = 0;
#else
            _defaultStringBuilder.Clear();
#endif
            return j;
        }
    }
}