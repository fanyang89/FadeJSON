using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FadeJSON
{
    public enum JsonObjectType
    {
        Null = 1,
        Boolean,
        Number,
        String,
        Object,
        Array
    }

    public class JsonObject : IEquatable<JsonObject>
    {
        public readonly double Double;
        public readonly bool Boolean;
        public readonly string String;
        public readonly Dictionary<string, JsonObject> Object;
        public readonly List<JsonObject> Array;
        public readonly JsonObjectType Type;

        public dynamic Value {
            get {
                switch (Type) {
                    case JsonObjectType.Null:
                        return null;
                    case JsonObjectType.Boolean:
                        return Boolean;
                    case JsonObjectType.Number:
                        return Double;
                    case JsonObjectType.String:
                        return String;
                    case JsonObjectType.Object:
                        return Object;
                    case JsonObjectType.Array:
                        return Array;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public JsonObject this[int index] => Array[index];
        public JsonObject this[string key] => Object[key];

        #region ctor

        public JsonObject(List<JsonObject> value) {
            Array = value;
            Type = JsonObjectType.Array;
        }

        public JsonObject(Dictionary<string, JsonObject> objectValues) {
            Object = objectValues;
            Type = JsonObjectType.Object;
        }

        public JsonObject(double value) {
            Double = value;
            Type = JsonObjectType.Number;
        }

        public JsonObject(bool value) {
            Boolean = value;
            Type = JsonObjectType.Boolean;
        }

        public JsonObject(string value) {
            String = value;
            Type = JsonObjectType.String;
        }

        #endregion

        public bool Equals(JsonObject other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            switch (Type) {
                case JsonObjectType.Null:
                case JsonObjectType.Boolean:
                    return other.Type == Type;
                case JsonObjectType.Number:
                    return other.Type == Type && Math.Abs(other.Double - Double) < double.Epsilon;
                case JsonObjectType.String:
                    return other.Type == Type && String == other.String;
                case JsonObjectType.Object:
                    return other.Type == Type && Object.Count == other.Object.Count
                           && !Object.Except(other.Object).Any();
                case JsonObjectType.Array:
                    return other.Type == Type && Array.SequenceEqual(other.Array);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string ToString() {
            switch (Type) {
                case JsonObjectType.Null:
                    return "null";
                case JsonObjectType.Boolean:
                    return Boolean ? "true" : "false";
                case JsonObjectType.Number:
                    return Double.ToString();
                case JsonObjectType.String:
                    return String;
                case JsonObjectType.Object:
                    return $"{{{string.Join(",", Object.Select(x => $"\"{x.Key}\":{(ReferenceEquals(x.Value, null) ? "null" : x.Value.ToString())}"))}}}";
                case JsonObjectType.Array:
                    return $"[{string.Join(",", Array.Select(x => ReferenceEquals(null, x) ? "null" : x.ToString()))}]";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}