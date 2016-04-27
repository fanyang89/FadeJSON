using System;

namespace FadeJson
{
    public class Parser
    {
        private readonly Cache<Tokenizer, JsonValue> _cache;

        public Parser(Cache<Tokenizer, JsonValue> cache) {
            this._cache = cache;
        }

        public JsonValue Parse() {
            var la = _cache.Peek();
            if (la.CheckValue(JsonType.Symbol, "{")) {
                return ParseJsonObject();
            }
            return la.CheckValue(JsonType.Symbol, "[") ? ParseJsonArray() : _cache.Consume();
        }

        private void Consume(string value, JsonType type = JsonType.Symbol) {
            var la = _cache.Peek();
            if (la.Type != type || la.Value != value) {
                throw new FormatException();
            }
            _cache.Consume();
        }

        private JsonValue ParseJsonArray() {
            Consume("[");
            var array = new JsonValue(JsonType.Array);
            var la = _cache.Peek();
            if (la.CheckValue(JsonType.Symbol, "]")) {
                Consume("]");
                return array;
            }
            while (la.Value != "]") {
                var value = Parse();
                array.Add(value);

                la = _cache.Peek();
                if (la.Value == "]" && la.Type == JsonType.Symbol) {
                    Consume("]");
                    break;
                }
                Consume(",");
            }
            return array;
        }

        private JsonValue ParseJsonObject() {
            Consume("{");
            var j = new JsonValue(JsonType.Object);
            var la = _cache.Peek();

            if (la.CheckValue(JsonType.Symbol, "}")) {
                Consume("}");
                return j;
            }

            if (la.Type != JsonType.String) {
                throw new FormatException();
            }

            while (la.Value != "}") {
                var key = _cache.Consume();
                Consume(":");
                var value = Parse();
                j.Add(key.Value, value);
                la = _cache.Peek();
                if (la.Value == "}" && la.Type == JsonType.Symbol) {
                    Consume("}");
                    break;
                }
                Consume(",");
            }
            return j;
        }
    }
}