using System;

namespace FadeJson
{
    public class Parser
    {
        private readonly ICommonCache<JsonValue, Tokenizer> cache;

        public Parser(ICommonCache<JsonValue, Tokenizer> cache) {
            this.cache = cache;
        }

        public JsonValue Parse() {
            var la = cache.Lookahead();
            if (la.CheckValue(JsonType.Symbol, "{")) {
                return ParseJsonObject();
            }
            return la.CheckValue(JsonType.Symbol, "[") ? ParseJsonArray() : cache.Next();
        }

        private void Consume(string value, JsonType type = JsonType.Symbol) {
            var la = cache.Lookahead();
            if (la.Type != type || la.Value != value) {
                throw new FormatException();
            }
            cache.Next();
        }

        private JsonValue ParseJsonArray() {
            Consume("[");
            var array = new JsonValue(JsonType.Array) {Type = JsonType.Array};
            var la = cache.Lookahead();
            while (la.Value != "]") {
                var value = Parse();
                array.Add(value);

                la = cache.Lookahead();
                if (la.Value == "]" && la.Type == JsonType.Symbol) {
                    break;
                }
                Consume(",");
            }
            Consume("]");
            return array;
        }

        private JsonValue ParseJsonObject() {
            Consume("{");
            if (cache.Lookahead().Type != JsonType.String) {
                throw new FormatException();
            }

            var j = new JsonValue(JsonType.Object);
            var la = cache.Lookahead();
            while (la.Value != "}") {
                var key = cache.Next();
                Consume(":");
                var value = Parse();
                j.Add(key.Value, value);
                la = cache.Lookahead();
                if (la.Value == "}" && la.Type == JsonType.Symbol) {
                    break;
                }
                Consume(",");
            }

            Consume("}");
            return j;
        }
    }
}