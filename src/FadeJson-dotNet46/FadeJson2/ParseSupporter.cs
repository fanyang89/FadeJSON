using System.Collections.Generic;

namespace FadeJson
{
    public class ParseSupporter
    {
        private readonly Queue<Token> tokenQueue = new Queue<Token>();

        public ParseSupporter(Lexer lexer) {
            Lexer = lexer;
        }

        private Lexer Lexer { get; }

        private Token? NextToken {
            get {
                if (tokenQueue.Count == 0) {
                    return Lexer.NextToken();
                }
                return tokenQueue.Dequeue();
            }
        }

        public bool MatchToken(TokenType tokenType, string value) {
            var token = NextToken;
            if (token == null) {
                RollbackToken(token);
                return false;
            }
            if (token.Value.TokenType == tokenType && token.Value.Value == value) {
                RollbackToken(token);
                return true;
            }
            RollbackToken(token);
            return false;
        }

        public Token? UsingToken() {
            var token = NextToken;
            if (token == null) {
                return null;
            }
            return token;
        }

        public Token? UsingToken(TokenType tokenType) {
            var token = NextToken;
            if (token == null) {
                return null;
            }
            if (token.Value.TokenType == tokenType) {
                return token;
            }
            RollbackToken(token);
            return null;
        }

        public Token? UsingToken(TokenType tokenType, string value) {
            var token = NextToken;
            if (token == null) {
                return null;
            }
            if (token.Value.TokenType == tokenType && token.Value.Value == value) {
                return token;
            }
            RollbackToken(token);
            return null;
        }

        public Token? UsingTokenExpect(TokenType tokenType) {
            var token = NextToken;
            if (token == null) {
                return null;
            }
            if (token.Value.TokenType != tokenType) {
                return token;
            }
            RollbackToken(token);
            return null;
        }

        private void RollbackToken(Token? token) {
            if (token == null) {
                return;
            }
            tokenQueue.Enqueue(token.Value);
        }
    }
}