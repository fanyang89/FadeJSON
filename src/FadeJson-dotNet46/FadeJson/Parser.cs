using System;
using System.Collections.Generic;

namespace FadeJson
{
    public class Parser
    {
        private readonly ParseHelper parseSupporter;

        public Parser(Lexer lexer) {
            parseSupporter = new ParseHelper(lexer);
        }

        public JsonValue Parse() {
            if (parseSupporter.MatchToken(TokenType.Symbol, "{")) {
                return ParseJsonObject();
            }
            if (parseSupporter.MatchToken(TokenType.Symbol, "[")) {
                return ParseJsonArray();
            }
            throw new FormatException(
                $"LineNumber:{parseSupporter.LineNumber},LinePosition:{parseSupporter.LinePosition}");
        }

        private JsonValue ParseJsonArray() {
            var result = new JsonValue(JsonValueType.Array);
            parseSupporter.Consume(TokenType.Symbol, "[");

            var value = ParseValue();
            var index = 0;
            while (value != null) {
                result.AddKeyValue(index++, value);
                parseSupporter.Consume(TokenType.Symbol, ",");
                value = ParseValue();
            }

            parseSupporter.Consume(TokenType.Symbol, "]");

            return result;
        }

        private JsonValue ParseJsonObject() {
            var j = new JsonValue(JsonValueType.Object);

            parseSupporter.Consume(TokenType.Symbol, "{");

            var pair = ParsePair();
            while (pair != null) {
                j.AddKeyValue(pair);
                parseSupporter.Consume(TokenType.Symbol, ",");
                pair = ParsePair();
            }

            parseSupporter.Consume(TokenType.Symbol, "}");
            return j;
        }

        private KeyValuePair<object, JsonValue>? ParsePair() {
            string key;
            {
                var token = parseSupporter.Consume(TokenType.String);
                if (token == null) {
                    return null;
                }
                key = token.Value.Value;
            }
            parseSupporter.Consume(TokenType.Symbol, ":");
            var value = ParseValue();
            if (value == null) {
                return null;
            }
            return new KeyValuePair<object, JsonValue>(key, value);
        }

        private JsonValue ParseValue() {
            if (parseSupporter.MatchToken(TokenType.Symbol, "{")) {
                return ParseJsonObject();
            }
            if (parseSupporter.MatchToken(TokenType.Symbol, "[")) {
                return ParseJsonArray();
            }
            {
                var token = parseSupporter.ConsumeExpect(TokenType.Symbol);
                return token?.RealValue;
            }
        }
    }
}