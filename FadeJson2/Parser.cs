using System;
using System.Collections.Generic;

namespace FadeJson2
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

        private List<dynamic> ParseJsonArray() {
            var result = new List<dynamic>();
            Token token;
            token = _.UsingToken(TokenType.SyntaxType, "[");

            var value = ParseValue();
            while (value != null) {
                result.Add(value);
                token = _.UsingToken(TokenType.SyntaxType, ",");
                value = ParseValue();
            }

            token = _.UsingToken(TokenType.SyntaxType, "]");

            return result;
        }

        private JsonObject ParseJsonObject() {
            var j = new JsonObject();

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

        private KeyValuePair<string, dynamic>? ParsePair() {
            var key = string.Empty;
            {
                var token = _.UsingToken(TokenType.StringType);
                if (token == null) {
                    return null;
                }
                key = token.Value;
            }
            _.UsingToken(TokenType.SyntaxType, ":");
            var value = ParseValue();
            if (value == null) {
                return null;
            }
            return new KeyValuePair<string, dynamic>(key, value);
        }

        private dynamic ParseValue() {
            if (_.MatchToken(TokenType.SyntaxType, "{")) {
                return ParseJsonObject();
            }
            if (_.MatchToken(TokenType.SyntaxType, "[")) {
                return ParseJsonArray();
            }
            {
                var token = _.UsingTokenExpect(TokenType.SyntaxType);
                return token != null ? token.RealValue : null;
            }
        }
    }
}