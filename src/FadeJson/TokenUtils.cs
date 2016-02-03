using System;

namespace FadeJson
{
    public static class TokenUtils
    {
        public static
#if NET35
            object
#else
            dynamic
#endif
            ValueOf(this JsonValue jsonValue) {
            switch (jsonValue.Type) {
                case JsonType.Int32:
                    {
                        int i;
                        if (!int.TryParse(jsonValue.Value, out i)) {
                            throw new FormatException();
                        }
                        return i;
                    }
                case JsonType.String:
                    return jsonValue.Value;
                case JsonType.Double:
                    {
                        double d;
                        if (!double.TryParse(jsonValue.Value, out d)) {
                            throw new FormatException();
                        }
                        return d;
                    }
                case JsonType.Null:
                    return null;
                case JsonType.Boolean:
                    return jsonValue.Value == "true";
                case JsonType.Symbol:
                    return jsonValue.Value;
                case JsonType.Array:
                    return jsonValue;
                case JsonType.Object:
                    return jsonValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}