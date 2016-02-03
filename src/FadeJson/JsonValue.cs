using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FadeJson
{
    public struct JsonValue
    {
        public static readonly JsonValue Colon = new JsonValue {Value = ":", Type = JsonType.Symbol};

        public static readonly JsonValue Comma = new JsonValue {Value = ",", Type = JsonType.Symbol};

        public static readonly JsonValue Eof = new JsonValue {Value = null, Type = JsonType.Symbol};

        public static readonly JsonValue False = new JsonValue {Value = "false", Type = JsonType.Boolean};

        public static readonly JsonValue LeftBrace = new JsonValue {Value = "{", Type = JsonType.Symbol};

        public static readonly JsonValue LeftBracket = new JsonValue {Value = "[", Type = JsonType.Symbol};

        public static readonly JsonValue Null = new JsonValue {Value = null, Type = JsonType.Null};

        public static readonly JsonValue RightBrace = new JsonValue {Value = "}", Type = JsonType.Symbol};

        public static readonly JsonValue RightBracket = new JsonValue {Value = "]", Type = JsonType.Symbol};

        public static readonly JsonValue True = new JsonValue {Value = "true", Type = JsonType.Boolean};

        private readonly Dictionary<string, JsonValue> dictionary;

        private readonly List<JsonValue> list;

        public JsonValue(JsonType type) {
            dictionary = null;
            list = null;
            Value = string.Empty;
            switch (type) {
                case JsonType.Array:
                    list = new List<JsonValue>();
                    Type = JsonType.Array;
                    break;

                case JsonType.Object:
                    dictionary = new Dictionary<string, JsonValue>();
                    Type = JsonType.Object;
                    break;
            }
            Type = type;
        }

        public int Count {
            get {
                var count = 0;
                if (dictionary != null) {
                    count += dictionary.Count;
                }
                if (list != null) {
                    count += list.Count;
                }
                return count;
            }
        }

        public IEnumerable<string> Keys {
            get {
                if (dictionary == null && list == null) {
                    return null;
                }
                if (dictionary == null && list != null) {
                    return Enumerable.Range(0, list.Count).Select(i => i.ToString());
                }
                if (dictionary != null && list == null) {
                    return dictionary.Keys;
                }
                return dictionary.Keys.Concat(Enumerable.Range(0, list.Count).Select(i => i.ToString()));
            }
        }

        public JsonType Type { get; set; }

        public string Value { get; set; }

        public IEnumerable<JsonValue> Values {
            get {
                if (dictionary == null && list == null) {
                    return null;
                }
                if (dictionary == null && list != null) {
                    return list;
                }
                if (dictionary != null && list == null) {
                    return dictionary.Values;
                }
                return dictionary.Values.Concat(list);
            }
        }

        public JsonValue this[string key] {
            get { return dictionary[key]; }
            set { dictionary[key] = value; }
        }

        public JsonValue this[int key] {
            get { return list[key]; }
            set { list[key] = value; }
        }

        public static JsonValue FromFile(string path) {
            var fileStream = new FileStream(path, FileMode.Open);
            var streamReader = new StreamReader(fileStream);
            var charCache = new CharCache(streamReader);
            var tokenizer = new Tokenizer(charCache);
            var tokenCache = new TokenCache(tokenizer);
            var parser = new Parser(tokenCache);
            var result = parser.Parse();
            fileStream.Dispose();
            return result;
        }

        public static JsonValue FromStream(Stream stream) {
            var streamReader = new StreamReader(stream);
            var charCache = new CharCache(streamReader);
            var tokenizer = new Tokenizer(charCache);
            var tokenCache = new TokenCache(tokenizer);
            var parser = new Parser(tokenCache);
            var result = parser.Parse();
            return result;
        }

        public static JsonValue FromString(string src) {
            var stringReader = new StringReader(src);
            var charCache = new CharCache(stringReader);
            var tokenizer = new Tokenizer(charCache);
            var tokenCache = new TokenCache(tokenizer);
            var parser = new Parser(tokenCache);
            return parser.Parse();
        }

        public static bool operator !=(JsonValue a, JsonValue b) {
            return !(a == b);
        }

        public static bool operator ==(JsonValue a, JsonValue b) {
            if (a.Type == b.Type && a.Value == b.Value) {
                return true;
            }
            return false;
        }

        public void Add(string key, JsonValue value) {
            dictionary.Add(key, value);
        }

        public void Add(JsonValue value) {
            list.Add(value);
        }

        public bool CheckValue(JsonType type, string value) {
            return type == Type && value == Value;
        }

        public bool Equals(JsonValue other) {
            return Equals(dictionary, other.dictionary) && Equals(list, other.list) && Type == other.Type &&
                   string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            return obj is JsonValue && Equals((JsonValue) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = dictionary?.GetHashCode() ?? 0;
                hashCode = (hashCode*397) ^ (list?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ (int) Type;
                hashCode = (hashCode*397) ^ (Value?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public override string ToString() {
            switch (Type) {
                case JsonType.Array: {
                    var sb = new StringBuilder();
                    sb.Append("[ ");
                    foreach (var value in list) {
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
                    foreach (var value in dictionary) {
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