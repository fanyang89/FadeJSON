using System;
using System.Collections.Generic;

namespace FadeJson2
{
    public class Parser
    {
        private readonly ParseSupporter _;

        public Parser(ParseSupporter parseSupporter) {
            _ = parseSupporter;
        }

        public Parser(Lexer lexer) {
            _ = new ParseSupporter(lexer);
        }

        public dynamic Parse() {
            if (_.CheckToken("{")) {
                return ParseJsonObject();
            }
            if (_.CheckToken("[")) {
                return ParseJsonArray();
            }
            throw new FormatException();
        }

        private List<dynamic> ParseJsonArray() {
            var result = new List<dynamic>();
            _.UsingToken("[");

            var value = ParseValue();
            while (value != null) {
                result.Add(value);
                _.UsingToken(",");
                value = ParseValue();
            }

            _.UsingToken("]");
            return result;
        }

        private JsonObject ParseJsonObject() {
            var j = new JsonObject();

            _.UsingToken("{");

            var pair = ParsePair();
            while (pair != null) {
                j.AddKeyValue(pair);
                _.UsingToken(",");
                pair = ParsePair();
            }

            _.UsingToken("}");
            return j;
        }

        private KeyValuePair<string, dynamic>? ParsePair() {
            var key = string.Empty;

            _.UsingToken(t => {
                key = t.Value;
                return true;
            }, TokenType.StringType);

            _.UsingToken(":");

            var value = ParseValue();
            if (value == null) {
                return null;
            }

            return new KeyValuePair<string, dynamic>(key, value);
        }

        private dynamic ParseValue() {
            dynamic result = null;
            
            if (_.CheckToken("{")) {
                result = ParseJsonObject();
                return result;
            }
            if (_.CheckToken("[")) {
                result = ParseJsonArray();
                return result;
            }

            _.UsingToken(t => {
                switch (t.TokenType) {
                    case TokenType.StringType:
                    case TokenType.IntegerType:
                    case TokenType.BoolType:
                        result = t.RealValue;
                        return true;
                }
                return true;
            });
            return result;
        }
    }
}