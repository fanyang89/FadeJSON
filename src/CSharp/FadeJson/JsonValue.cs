using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FadeJson
{
    public class JsonValue : IEquatable<JsonValue>
    {
        public static readonly JsonValue Colon = new JsonValue(JsonType.Symbol) { Value = ":" };

        public static readonly JsonValue Comma = new JsonValue(JsonType.Symbol) { Value = "," };

        public static readonly JsonValue Eof = new JsonValue(JsonType.Symbol) { Value = null };

        public static readonly JsonValue False = new JsonValue(JsonType.Boolean) { Value = "false" };

        public static readonly JsonValue LeftBrace = new JsonValue(JsonType.Symbol) { Value = "{" };

        public static readonly JsonValue LeftBracket = new JsonValue(JsonType.Symbol) { Value = "[" };

        public static readonly JsonValue Null = new JsonValue(JsonType.Null) { Value = null };

        public static readonly JsonValue RightBrace = new JsonValue(JsonType.Symbol) { Value = "}" };

        public static readonly JsonValue RightBracket = new JsonValue(JsonType.Symbol) { Value = "]" };

        public static readonly JsonValue True = new JsonValue(JsonType.Boolean) { Value = "true" };

        private readonly Dictionary<string, JsonValue> _dictionary;

        private readonly List<JsonValue> _list;

        public JsonValue(JsonType type) {
            _dictionary = null;
            _list = null;
            Value = string.Empty;
            switch (type) {
                case JsonType.Array:
                    _list = new List<JsonValue>();
                    Type = JsonType.Array;
                    break;

                case JsonType.Object:
                    _dictionary = new Dictionary<string, JsonValue>();
                    Type = JsonType.Object;
                    break;
            }
            Type = type;
        }

        public int Count {
            get {
                var count = 0;
                if (_dictionary != null) {
                    count += _dictionary.Count;
                }
                if (_list != null) {
                    count += _list.Count;
                }
                return count;
            }
        }

        public IEnumerable<string> Keys {
            get {
                if (_dictionary == null && _list == null) {
                    return null;
                }
                if (_dictionary == null && _list != null) {
                    return Enumerable.Range(0, _list.Count).Select(i => i.ToString());
                }
                if (_dictionary != null && _list == null) {
                    return _dictionary.Keys;
                }
                return _dictionary.Keys.Concat(Enumerable.Range(0, _list.Count).Select(i => i.ToString()));
            }
        }

        public JsonType Type { get; }

        public string Value { get; set; }

        public IEnumerable<JsonValue> Values {
            get {
                if (_dictionary == null && _list == null) {
                    return null;
                } else
                if (_dictionary == null && _list != null) {
                    return _list;
                } else if (_dictionary != null && _list == null) {
                    return _dictionary.Values;
                } else {
                    return _dictionary.Values.Concat(_list);
                }
            }
        }

        public JsonValue this[string key] {
            get { return _dictionary[key]; }
            set { _dictionary[key] = value; }
        }

        public JsonValue this[int key] {
            get { return _list[key]; }
            set { _list[key] = value; }
        }

        public static JsonValue FromFile(string path) {
            var fileStream = new FileStream(path, FileMode.Open);
            var streamReader = new StreamReader(fileStream);
            var charCache = new CharCacheSource(streamReader);
            var tokenizer = new Tokenizer(new Cache<TextReader, char>(charCache));
            var tokenCache = new TokenCacheSource(tokenizer);
            var parser = new Parser(new Cache<Tokenizer, JsonValue>(tokenCache));
            var result = parser.Parse();
            fileStream.Dispose();
            return result;
        }

        public static JsonValue FromStream(Stream stream) {
            var streamReader = new StreamReader(stream);
            var charCache = new CharCacheSource(streamReader);
            var tokenizer = new Tokenizer(new Cache<TextReader, char>(charCache));
            var tokenCache = new TokenCacheSource(tokenizer);
            var parser = new Parser(new Cache<Tokenizer, JsonValue>(tokenCache));
            var result = parser.Parse();
            return result;
        }

        public static JsonValue FromString(string src) {
            var stringReader = new StringReader(src);
            var charCache = new CharCacheSource(stringReader);
            var tokenizer = new Tokenizer(new Cache<TextReader, char>(charCache));
            var tokenCache = new TokenCacheSource(tokenizer);
            var parser = new Parser(new Cache<Tokenizer, JsonValue>(tokenCache));
            return parser.Parse();
        }

        public static bool operator !=(JsonValue a, JsonValue b) {
            return !(a == b);
        }

        public static bool operator ==(JsonValue a, JsonValue b) {
            return !ReferenceEquals(a, null) && a.Equals(b);
        }

        public void Add(string key, JsonValue value) {
            _dictionary.Add(key, value);
        }

        public void Add(JsonValue value) {
            _list.Add(value);
        }

        public bool CheckValue(JsonType type, string value) {
            return type == Type && value == Value;
        }

        public bool Equals(JsonValue other) {
            return Equals(_dictionary, other._dictionary) && Equals(_list, other._list)
                && Type == other.Type && string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            return obj is JsonValue && Equals((JsonValue)obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = _dictionary?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (_list?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (int)Type;
                hashCode = (hashCode * 397) ^ (Value?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public override string ToString() {
            switch (Type) {
                case JsonType.Array: {
                        var sb = new StringBuilder();
                        sb.Append("[ ");
                        foreach (var value in _list) {
                            sb.Append(value);
                            sb.Append(",");
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append(" ]");
                        return sb.ToString();
                    }
                case JsonType.Object: {
                        var sb = new StringBuilder();
                        sb.Append("{ ");
                        foreach (var value in _dictionary) {
                            sb.Append("\"").Append(value.Key).Append("\"");
                            sb.Append(":");
                            sb.Append(value.Value);
                            sb.Append(",");
                        }
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append("}");
                        return sb.ToString();
                    }
                case JsonType.String:
                    return $"\"{Value}\"";
            }
            return Value;
        }
    }
}