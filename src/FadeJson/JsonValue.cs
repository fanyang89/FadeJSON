using System;
using System.Collections.Generic;
using System.IO;

namespace FadeJson
{
    public class JsonValue
    {
        public static readonly JsonValue Null = new JsonValue { Value = null, Type = JsonType.Null };
        public static readonly JsonValue True = new JsonValue() { Value = "true", Type = JsonType.Boolean };
        public static readonly JsonValue False = new JsonValue() { Value = "false", Type = JsonType.Boolean };

        private readonly Dictionary<string, JsonValue> dictionary = new Dictionary<string, JsonValue>();

        public JsonType Type { get; set; }

        public string Value { get; set; }

        public int Count => dictionary.Count;
        public IEnumerable<JsonValue> Values => dictionary.Values;
        public IEnumerable<string> Keys => dictionary.Keys;

        public JsonValue this[string key] {
            get { return dictionary[key]; }
            set { dictionary[key] = value; }
        }

        public JsonValue this[object key] {
            get { return this[key.ToString()]; }
            set { dictionary[key.ToString()] = value; }
        }

        public void Add(object key, JsonValue value) {
            dictionary.Add(key.ToString(), value);
        }

        public void Add(string key, JsonValue value) {
            dictionary.Add(key, value);
        }

        public bool CheckValue(JsonType type, string value) {
            return type == Type && value == Value;
        }

        public override string ToString() {
            return Value;
        }

        public static JsonValue FromString(string src) {
            var stringReader = new StringReader(src);
            var charCache = new CommonCache(stringReader);
            var tokenizer = new Tokenizer(charCache);
            var tokenCache = new TokenCache(tokenizer);
            var parser = new Parser(tokenCache);
            return parser.Parse();
        }

        public static JsonValue FromFile(string path) {
            var fileStream = new FileStream(path, FileMode.Open);
            var streamReader = new StreamReader(fileStream);
            var charCache = new CommonCache(streamReader);
            var tokenizer = new Tokenizer(charCache);
            var tokenCache = new TokenCache(tokenizer);
            var parser = new Parser(tokenCache);
            var result = parser.Parse();
            fileStream.Dispose();
            return result;
        }
    }
}