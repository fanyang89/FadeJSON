using System;

namespace FadeJson
{
    public static class TokenUtils
    {
        public static dynamic ValueOf(this JsonValue token) {
            switch (token.Type) {
                case JsonType.Int32:
                    int i;
                    if (!int.TryParse(token.Value, out i)) {
                        throw new FormatException();
                    }
                    return i;
                case JsonType.String:
                    return token.Value;
                case JsonType.Double:
                    double d;
                    double.TryParse(token.Value, out d);
                    return d;
                case JsonType.Null:
                    return null;
                case JsonType.Boolean:
                    return token.Value == "true";
                case JsonType.Symbol:
                    return token.Value;
                case JsonType.Array:
                    throw new ArgumentOutOfRangeException();
                case JsonType.Object:
                    throw new ArgumentOutOfRangeException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}