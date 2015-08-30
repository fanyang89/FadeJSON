using System;
using System.Collections.Generic;

namespace FadeJson2
{
    public class Parser
    {
        private Lexer lexer;

        public Parser(Lexer lexer) {
            this.lexer = lexer;
        }

        #region Parse Method
        public JsonObject Parse() {
            //TODO let array bacome a entry
            return ParseJsonObject();
        }

        private JsonObject ParseJsonObject() {
            var j = new JsonObject();
            var token = NextToken;
            if (token.Value == "{") {
                var pair = ParsePair();
                while (pair != null) {
                    j.AddKeyValue(pair);
                    pair = ParsePair();
                }
                token = NextToken;
                if (token.Value == "}") {
                    return j;
                }
                RollbackToken(token);
            }
            RollbackToken(token);
            throw new FormatException();
        }

        private KeyValuePair<string, dynamic>? ParsePair() {
            var token = NextToken;
            if (token.TokenType == TokenType.StringType) {
                var key = token.Value;
                token = NextToken;
                if (token.TokenType == TokenType.SyntaxType && token.Value == ":") {
                    token = NextToken;
                    switch (token.TokenType) {
                        case TokenType.StringType:
                        case TokenType.IntegerType:
                            return new KeyValuePair<string, dynamic>(key, token.RealValue);
                    }
                    RollbackToken(token);
                }
                RollbackToken(token);
            }
            RollbackToken(token);
            return null;
        }
        #endregion

        #region Parsing Support
        public delegate bool UsingTokenDelegate(Token token);

        public void UsingToken(UsingTokenDelegate method) {
            var token = NextToken;
            var result = method.Invoke(token);
            if (!result) {
                RollbackToken(token);
            }
        }

        private void ParseTest() {
            UsingToken(token => {
                return true;
            });
        }
        #endregion

        #region Token Control
        private readonly Queue<Token> tokenQueue = new Queue<Token>();

        private Token NextToken {
            get {
                if (tokenQueue.Count == 0) {
                    return lexer.NextToken();
                }
                return tokenQueue.Dequeue();
            }
        }

        private void RollbackToken(Token token) {
            tokenQueue.Enqueue(token);
        }
        #endregion
    }
}