using System.Collections.Generic;

namespace FadeJson2
{
    public class ParseSupporter
    {
        private Lexer Lexer { get; }

        public ParseSupporter(Lexer lexer) {
            Lexer = lexer;
        }

        private readonly Queue<Token> tokenQueue = new Queue<Token>();

        private Token NextToken {
            get {
                if (tokenQueue.Count == 0) {
                    return Lexer.NextToken();
                }
                return tokenQueue.Dequeue();
            }
        }

        private void RollbackToken(Token token) {
            tokenQueue.Enqueue(token);
        }

        public delegate bool UsingTokenDelegate(Token token);

        public ParseSupporter UsingToken(UsingTokenDelegate method) {
            var token = NextToken;
            if (token == null) {
                return this;
            }
            var result = method.Invoke(token);
            if (!result) {
                RollbackToken(token);
            }
            return this;
        }

        public delegate void MyAction();

        public ParseSupporter UsingToken(UsingTokenDelegate method, TokenType tokenType) {
            var token = NextToken;
            if (token == null) {
                return this;
            }
            if (token.TokenType != tokenType) {
                RollbackToken(token);
                return this;
            }
            var result = method.Invoke(token);
            if (!result) {
                RollbackToken(token);
            }
            return this;
        }

        public ParseSupporter UsingToken(TokenType tokenType, dynamic tokenValue) {
            var token = NextToken;
            if (token == null) {
                return this;
            }
            if (token.TokenType != tokenType || token.Value != tokenValue) {
                RollbackToken(token);
            }
            return this;
        }

        public ParseSupporter UsingToken(TokenType tokenType) {
            var token = NextToken;
            if (token == null) {
                return this;
            }
            if (token.TokenType != tokenType) {
                RollbackToken(token);
            }
            return this;
        }

        public bool UsingToken(dynamic tokenValue) {
            var token = NextToken;
            if (token == null) {
                return false;
            }
            if (token.Value != tokenValue) {
                RollbackToken(token);
                return false;
            }
            return true;
        }

        public bool CheckToken(dynamic tokenValue) {
            var token = NextToken;
            if (token == null) {
                return false;
            }
            if (token.Value != tokenValue) {
                RollbackToken(token);
                return false;
            }
            RollbackToken(token);
            return true;
        }

    }
}