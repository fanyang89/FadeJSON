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
            if (parseSupporter.MatchToken(TokenType.SyntaxType, "{")) {
                return ParseJsonObject();
            }
            if (parseSupporter.MatchToken(TokenType.SyntaxType, "[")) {
                return ParseJsonArray();
            }
            throw new FormatException(
                $"LineNumber:{parseSupporter.LineNumber},LinePosition:{parseSupporter.LinePosition}");
        }

        private JsonValue ParseJsonArray() {
            var result = new JsonValue(JsonValueType.Array);
            parseSupporter.UsingToken(TokenType.SyntaxType, "[");

            var value = ParseValue();
            var index = 0;
            while (value != null) {
                result.AddKeyValue(index++, value);
                parseSupporter.UsingToken(TokenType.SyntaxType, ",");
                value = ParseValue();
            }

            parseSupporter.UsingToken(TokenType.SyntaxType, "]");

            return result;
        }

        private JsonValue ParseJsonObject() {
            var j = new JsonValue(JsonValueType.Object);

            parseSupporter.UsingToken(TokenType.SyntaxType, "{");

            var pair = ParsePair();
            while (pair != null) {
                j.AddKeyValue(pair);
                parseSupporter.UsingToken(TokenType.SyntaxType, ",");
                pair = ParsePair();
            }

            parseSupporter.UsingToken(TokenType.SyntaxType, "}");
            return j;
        }

        private KeyValuePair<object, JsonValue>? ParsePair() {
            string key;
            {
                var token = parseSupporter.UsingToken(TokenType.StringType);
                if (token == null) {
                    return null;
                }
                key = token.Value.Value;
            }
            parseSupporter.UsingToken(TokenType.SyntaxType, ":");
            var value = ParseValue();
            if (value == null) {
                return null;
            }
            return new KeyValuePair<object, JsonValue>(key, value);
        }

        private JsonValue ParseValue() {
            if (parseSupporter.MatchToken(TokenType.SyntaxType, "{")) {
                return ParseJsonObject();
            }
            if (parseSupporter.MatchToken(TokenType.SyntaxType, "[")) {
                return ParseJsonArray();
            }
            {
                var token = parseSupporter.UsingTokenExpect(TokenType.SyntaxType);
                return token?.RealValue;
            }
        }
    }
}