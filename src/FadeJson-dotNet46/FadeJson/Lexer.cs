using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FadeJson
{
    public class Lexer
    {
        public const char Eof = unchecked((char)-1);

        private const string EMPTY_CHAR_LIST = " \r\n\t";
        private const string KEY_CHAR_LIST = "{}:,[]";
        private readonly TextReader textReader;

        private Lexer(Stream stream) {
            textReader = new StreamReader(stream);
        }

        private Lexer(string content) {
            textReader = new StringReader(content);
        }

        public int CurrentLineNumber { get; set; } = 1;
        public int CurrentLinePosition { get; set; }

        public static Lexer FromFile(string filename) => new Lexer(new FileStream(filename, FileMode.Open));

        public static Lexer FromStream(Stream stream) => new Lexer(stream);

        public static Lexer FromString(string content) => new Lexer(content);

        public List<Token> GetAllTokens() {
            var tokens = new List<Token>();
            var token = NextToken();
            while (token != null) {
                tokens.Add(token.Value);
                token = NextToken();
            }
            return tokens;
        }

        public Token? NextToken() {
            var c = Peek();
            if (c == '\n') {
                CurrentLineNumber++;
                CurrentLinePosition = 0;
            }
            if (EMPTY_CHAR_LIST.Contains(new string(c, 1))) {
                Consume();
                return NextToken();
            }
            if (c == '/') {
                SkipComment();
                return NextToken();
            }
            if (KEY_CHAR_LIST.Contains(new string(c, 1))) {
                Consume();
                return new Token(c, TokenType.Symbol, CurrentLineNumber, CurrentLinePosition);
            }
            if (char.IsDigit(c)) {
                return GetNumberToken();
            }
            if (c == '@') {
                return GetOriginalStringToken();
            }
            if (c == '"') {
                return GetStringToken();
            }
            if (c == 't' || c == 'f') {
                return GetBoolToken();
            }
            return null;
        }

        private char Peek() => (char)textReader.Peek();

        public char Consume() {
            CurrentLinePosition++;
            return (char)textReader.Read();
        }

        public bool Consume(string value) {
            for (int i = 0; i < value.Length; i++) {
                var c = Peek();
                if (c != value[i]) {
                    return false;
                }
                Consume();
            }
            return true;
        }

        private Token? GetBoolToken() {
            var c = Peek();
            if (c == 't' && Consume("true")) {
                return new Token("true", TokenType.Bool, CurrentLineNumber, CurrentLinePosition);
            }
            if (c == 'f' && Consume("false")) {
                return new Token("false", TokenType.Bool, CurrentLineNumber, CurrentLinePosition);
            }
            throw new FormatException();
        }

        private Token GetNumberToken() {
            // Check integrity before loop to avoid accidentally returning zero.
            var c = Consume();
            if (c == Eof || !char.IsDigit(c)) {
                throw new InvalidOperationException("Internal: tokenize number?");
            }
            var res = new StringBuilder();
            var isDouble = false;
            res.Append(c);
            c = Peek();
            while (c != Eof) {
                if (c == '.') {
                    isDouble = true;
                }
                else if (!char.IsDigit(c)) {
                    break;
                }
                res.Append(c);
                Consume();
                c = Peek();
            }
            return new Token(res.ToString(),
                isDouble ? TokenType.Double : TokenType.Integer, CurrentLineNumber, CurrentLinePosition);
        }

        private Token GetOriginalStringToken() {
            var isDocString = false;
            var c = Consume();
            if (c == Eof || c != '@') {
                throw new InvalidOperationException("Internal: parsing original string?");
            }

            c = Peek();
            if (c == Eof || c != '\"') {
                throw new InvalidOperationException("Internal: parsing original string?");
            }
            Consume();

            c = Peek();
            if (c == '\"') {
                Consume();
                c = Peek();
                if (c == '\"') {
                    Consume();
                    isDocString = true;
                    c = Peek();
                }
            }

            var result = new StringBuilder();
            while (true) {
                if (c == Eof) {
                    throw new InvalidOperationException("Hit EOF in String literal.");
                }
                var quotesCount = 0;
                if (c == '\"') {
                    Consume();
                    if (!isDocString) {
                        return new Token(result.ToString(), TokenType.String,
                            CurrentLineNumber, CurrentLinePosition);
                    }
                    quotesCount++;

                    c = Peek();
                    if (c == '\"') {
                        quotesCount++;
                        Consume();
                        c = Peek();
                        if (c == '\"') {
                            quotesCount++;
                            Consume();
                        }
                    }
                    if (quotesCount == 3) {
                        return new Token(result.ToString(), TokenType.String,
                            CurrentLineNumber, CurrentLinePosition);
                    }
                    result.Append('\"', quotesCount);
                }
                result.Append(c);
                Consume();
                c = Peek();
            }
        }

        private Token GetStringToken() {
            var c = Consume();
            if (c == Eof || c != '\"') {
                throw new InvalidOperationException(
                    "Internal: parsing string?");
            }
            var res = new StringBuilder();
            var escape = false;
            c = Peek();
            while (true) {
                if (c == Eof) {
                    throw new InvalidOperationException(
                        "Hit EOF in string literal.");
                }
                if (c == '\n' || c == '\r') {
                    throw new InvalidOperationException(
                        "Hit newline in string literal");
                }
                if (c == '\\' && !escape) {
                    Consume();
                    escape = true;
                }
                else if (c == '"' && !escape) {
                    Consume();
                    return new Token(res.ToString(), TokenType.String, CurrentLineNumber, CurrentLinePosition);
                }
                else if (escape) {
                    escape = false;
                    Consume();
                    if (c == 'n') {
                        res.Append('\n');
                    }
                    else if (c == 't') {
                        res.Append('\t');
                    }
                    else if (c == 'r') {
                        res.Append('\r');
                    }
                    else if (c == '\"') {
                        res.Append('\"');
                    }
                    else if (c == '\\') {
                        res.Append('\\');
                    }
                }
                else {
                    Consume();
                    res.Append(c);
                }
                c = Peek();
            }
        }

        private void JumpToLineEnd() {
            var c = Peek();
            while (c != '\n') {
                Consume();
                c = Peek();
            }
        }

        private void SkipComment() {
            Consume();
            if (Peek() == '/') {
                Consume();
                JumpToLineEnd();
            }
        }
    }
}