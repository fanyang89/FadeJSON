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

        public JsonObject Parse() {
            //TODO let array bacome an entry
            return ParseJsonObject();
        }

        private JsonObject ParseJsonObject() {
            var j = new JsonObject();
            var isExit = false;
            _.IsExit = new Ref<bool>(() => isExit, v => { isExit = v; });

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
            KeyValuePair<string, dynamic>? pair = null;
            var isExit = false;
            _.IsExit = new Ref<bool>(() => isExit, v => { isExit = v; });

            _.UsingToken(t => {
                key = t.Value;
                return true;
            }, TokenType.StringType, () => { isExit = true; });

            _.UsingToken(":");

            _.UsingToken(t => {
                switch (t.TokenType) {
                    case TokenType.StringType:
                    case TokenType.IntegerType:
                        pair = new KeyValuePair<string, dynamic>(key, t.RealValue);
                        return true;
                }
                var j = ParseJsonObject();
                pair = new KeyValuePair<string, dynamic>(key, j);
                return true;
            });

            return pair;
        }
    }
}