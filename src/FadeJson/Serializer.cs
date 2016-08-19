using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FadeJSON
{
    public class Serializer
    {
        public static string Serialize(JsonObject json) {
            switch (json.Type) {
                case JsonObjectType.Null:
                    return "null";
                case JsonObjectType.Boolean:
                    return json.Boolean ? "true" : "false";
                case JsonObjectType.Number:
                    return json.Double.ToString();
                case JsonObjectType.String:
                    return $"\"{json.String}\"";
                case JsonObjectType.Object:
                    return $"{{{string.Join(",", json.Object.Select(x => $"\"{x.Key}\":{Serialize(x.Value)}"))}}}";
                case JsonObjectType.Array:
                    return $"[{string.Join(",", json.Array.Select(Serialize))}]";
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return string.Empty;
        }
    }
}
