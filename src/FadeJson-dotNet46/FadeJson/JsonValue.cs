using System;
using System.Collections;
using System.Collections.Generic;

namespace FadeJson
{
    public enum JsonValueType
    {
        Object,
        Array,
        Int,
        String,
        Boolean,
        Double
    }

    public static class JsonValueExtension
    {
        public static T Value<T>(this JsonValue value) {
            return (T)value.Value;
        }
    }

    public class JsonValue
    {
        private readonly Dictionary<object, object> dict = new Dictionary<object, object>();

        public JsonValue(JsonValueType jsonValueType) {
            JsonValueType = jsonValueType;
            Value = null;
        }

        public JsonValue(bool val) {
            JsonValueType = JsonValueType.Boolean;
            Value = val;
        }

        public JsonValue(double val) {
            JsonValueType = JsonValueType.Double;
            Value = val;
        }

        public JsonValue(int val) {
            JsonValueType = JsonValueType.Int;
            Value = val;
        }

        public JsonValue(string val) {
            JsonValueType = JsonValueType.String;
            Value = val;
        }

        public JsonValueType JsonValueType { get; }
        public object Value { get; }
        public ICollection Keys => dict.Keys;
        public ICollection Values => dict.Values;

        public JsonValue this[object key] {
            get { return (JsonValue)dict[key]; }
            set { dict[key] = value; }
        }

        public static JsonValue FromObject(object o) {
            return Deserializer.Parse(o);
        }

        public static JsonValue FromFile(string filename) {
            var lexer = Lexer.FromFile(filename);
            var parser = new Parser(lexer);
            return parser.Parse();
        }

        public static JsonValue FromString(string content) {
            var lexer = Lexer.FromString(content);
            var parser = new Parser(lexer);
            return parser.Parse();
        }

        public static implicit operator JsonValue(int value) {
            return new JsonValue(value);
        }

        public static implicit operator JsonValue(string value) {
            return new JsonValue(value);
        }

        public static implicit operator JsonValue(bool value) {
            return new JsonValue(value);
        }

        public static implicit operator JsonValue(double value) {
            return new JsonValue(value);
        }

        public void AddKeyValue(KeyValuePair<object, JsonValue>? pair) {
            if (pair.HasValue) {
                dict.Add(pair.Value.Key, pair.Value.Value);
            }
        }

        public void AddKeyValue(int index, JsonValue value) {
            dict.Add(index, value);
        }

        public void AddKeyValue(string key, object value) {
            dict.Add(key, value);
        }

        public override string ToString() {
            return Value?.ToString() ?? string.Empty;
        }
    }
}