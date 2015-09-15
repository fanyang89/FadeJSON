using System.Collections.Generic;

namespace FadeJson
{
    public class ParseHelper
    {
        private readonly List<Token> tokenQueue = new List<Token>();

        public ParseHelper(Lexer lexer) {
            Lexer = lexer;
        }

        public int LineNumber => Lexer.CurrentLineNumber;
        public int LinePosition => Lexer.CurrentLinePosition;

        private Lexer Lexer { get; }

        private Token? NextToken {
            get {
                if (tokenQueue.Count == 0) {
                    return Lexer.NextToken();
                }
                var token = tokenQueue[0];
                tokenQueue.RemoveAt(0);
                return token;
            }
        }

        public bool MatchToken(TokenType tokenType, string value) {
            var token = NextToken;
            if (token == null) {
                RollbackToken(null);
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

        /// <summary>
        ///     返回除了给定token类型的token
        /// </summary>
        /// <param name="tokenType">排除列表</param>
        /// <returns></returns>
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
            tokenQueue.Add(token.Value);
        }
    }
}