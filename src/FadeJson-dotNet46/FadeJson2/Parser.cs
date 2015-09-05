using System;
using System.Collections.Generic;

namespace FadeJson
{
    public class Parser
    {
        private readonly ParseSupporter _;

        public Parser(Lexer lexer) {
            _ = new ParseSupporter(lexer);
        }

        public dynamic Parse() {
            if (_.MatchToken(TokenType.SyntaxType, "{")) {
                return ParseJsonObject();
            }
            if (_.MatchToken(TokenType.SyntaxType, "[")) {
                return ParseJsonArray();
            }
            throw new FormatException();
        }

        private JsonValue ParseJsonArray() {
            var result = new JsonValue(JsonValueType.Array);
            _.UsingToken(TokenType.SyntaxType, "[");

            var value = ParseValue();
            int index = 0;
            while (value != null) {
                result.AddKeyValue(index++, value);
                _.UsingToken(TokenType.SyntaxType, ",");
                value = ParseValue();
            }

            _.UsingToken(TokenType.SyntaxType, "]");

            return result;
        }

        private JsonValue ParseJsonObject() {
            var j = new JsonValue(JsonValueType.Object);

            _.UsingToken(TokenType.SyntaxType, "{");

            var pair = ParsePair();
            while (pair != null) {
                j.AddKeyValue(pair);
                _.UsingToken(TokenType.SyntaxType, ",");
                pair = ParsePair();
            }

            _.UsingToken(TokenType.SyntaxType, "}");
            return j;
        }

        private KeyValuePair<object, JsonValue>? ParsePair() {
            var key = string.Empty;
            {
                var token = _.UsingToken(TokenType.StringType);
                if (token == null) {
                    return null;
                }
                key = token.Value.Value;
            }
            _.UsingToken(TokenType.SyntaxType, ":");
            var value = ParseValue();
            if (value == null) {
                return null;
            }
            return new KeyValuePair<object, JsonValue>(key, value);
        }

        private JsonValue ParseValue() {
            if (_.MatchToken(TokenType.SyntaxType, "{")) {
                return ParseJsonObject();
            }
            if (_.MatchToken(TokenType.SyntaxType, "[")) {
                return ParseJsonArray();
            }
            {
                var token = _.UsingTokenExpect(TokenType.SyntaxType);
                return token != null ? token.Value.RealValue : null;
            }
        }
    }
}