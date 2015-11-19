using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FadeJson
{
    public struct JsonValue
    {
        public static readonly JsonValue Null = new JsonValue { Value = null, Type = JsonType.Null };
        public static readonly JsonValue True = new JsonValue { Value = "true", Type = JsonType.Boolean };
        public static readonly JsonValue False = new JsonValue { Value = "false", Type = JsonType.Boolean };
        public static readonly JsonValue Colon = new JsonValue { Value = ":", Type = JsonType.Symbol };
        public static readonly JsonValue LeftBrace = new JsonValue { Value = "{", Type = JsonType.Symbol };
        public static readonly JsonValue RightBrace = new JsonValue { Value = "}", Type = JsonType.Symbol };
        public static readonly JsonValue LeftBracket = new JsonValue { Value = "[", Type = JsonType.Symbol };
        public static readonly JsonValue RightBracket = new JsonValue { Value = "]", Type = JsonType.Symbol };
        public static readonly JsonValue Comma = new JsonValue { Value = ",", Type = JsonType.Symbol };

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

        public JsonType Type { get; set; }
        public string Value { get; set; }

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

        public IEnumerable<JsonValue> Values => dictionary.Values.Concat(list);

        public IEnumerable<string> Keys {
            get {
                return dictionary.Keys.Concat(Enumerable.Range(0, list.Count).Select(i => i.ToString()));
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

        public void Add(string key, JsonValue value) {
            dictionary.Add(key, value);
        }
        
        public void Add(JsonValue value) {
            list.Add(value);
        }

        public bool CheckValue(JsonType type, string value) {
            return type == Type && value == Value;
        }

        public override string ToString() {
            return Value;
        }

        public static JsonValue FromString(string src) {
            var stringReader = new StringReader(src);
            var charCache = new CharCache(stringReader);
            var tokenizer = new Tokenizer(charCache);
            var tokenCache = new TokenCache(tokenizer);
            var parser = new Parser(tokenCache);
            return parser.Parse();
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
    }
}