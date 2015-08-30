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

        private Token NextToken => tokenQueue.Count == 0 ? Lexer.NextToken() : tokenQueue.Dequeue();

        private void RollbackToken(Token token) {
            tokenQueue.Enqueue(token);
        }

        public delegate bool UsingTokenDelegate(Token token);

        public Ref<bool> IsExit { get; set; }

        public ParseSupporter UsingToken(UsingTokenDelegate method) {
            if (IsExit.Value) return this;
            var token = NextToken;
            if (token == null) {
                IsExit.Value = true;
                return this;
            }
            var result = method.Invoke(token);
            if (!result) {
                RollbackToken(token);
            }
            return this;
        }

        public delegate void MyAction();

        public ParseSupporter UsingToken(UsingTokenDelegate method, TokenType tokenType, MyAction action) {
            if (IsExit.Value) return this;
            var token = NextToken;
            if (token == null) {
                IsExit.Value = true;
                return this;
            }
            if (token.TokenType != tokenType) {
                action.Invoke();
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
            if (IsExit.Value) return this;
            var token = NextToken;
            if (token == null) {
                IsExit.Value = true;
                return this;
            }
            if (token.TokenType != tokenType || token.Value != tokenValue) {
                RollbackToken(token);
            }
            return this;
        }

        public ParseSupporter UsingToken(TokenType tokenType) {
            if (IsExit.Value) return this;
            var token = NextToken;
            if (token == null) {
                IsExit.Value = true;
                return this;
            }
            if (token.TokenType != tokenType) {
                RollbackToken(token);
            }
            return this;
        }

        public bool UsingToken(dynamic tokenValue) {
            if (IsExit.Value) return false;
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