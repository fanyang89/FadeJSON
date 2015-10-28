using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace FadeJson
{
    public class Lexer
    {
        public const char Eof = unchecked((char)-1);

        private const string EmptyCharList = " \r\n\t";
        private const string KeyCharList = "{}:,[]";
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
            var c = PeekChar();
            if (c == '\n') {
                CurrentLineNumber++;
                CurrentLinePosition = 0;
            }
            if (EmptyCharList.Contains(new string(c, 1))) {
                GetChar();
                return NextToken();
            }
            if (c == '/') {
                GetChar();
                c = PeekChar();
                if (c == '/') {
                    GetChar();
                    JumpToLineEnd();
                    return NextToken();
                }
            }
            if (KeyCharList.Contains(new string(c, 1))) {
                GetChar();
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

        private Token? GetBoolToken() {
            var c = PeekChar();
            if (c == 't') {
                GetChar();
                c = PeekChar();
                if (c == 'r') {
                    GetChar();
                    c = PeekChar();
                    if (c == 'u') {
                        GetChar();
                        c = PeekChar();
                        if (c == 'e') {
                            GetChar();
                            return new Token("true", TokenType.Bool, CurrentLineNumber, CurrentLinePosition);
                        }
                    }
                }
            }
            if (c == 'f') {
                GetChar();
                c = PeekChar();
                if (c == 'a') {
                    GetChar();
                    c = PeekChar();
                    if (c == 'l') {
                        GetChar();
                        c = PeekChar();
                        if (c == 's') {
                            GetChar();
                            c = PeekChar();
                            if (c == 'e') {
                                GetChar();
                                return new Token("false", TokenType.Bool, CurrentLineNumber, CurrentLinePosition);
                            }
                        }
                    }
                }
            }
            throw new FormatException();
        }

        private char GetChar() {
            CurrentLinePosition++;
            return (char)textReader.Read();
        }

        private Token GetNumberToken() {
            // Check integrity before loop to avoid accidently returning zero.
            var c = GetChar();
            if (c == Eof || !char.IsDigit(c)) {
                throw new InvalidOperationException("Internal: lexing number?");
            }
            var res = new StringBuilder();
            var isDouble = false;
            res.Append(c);
            c = PeekChar();
            while (c != Eof) {
                if (c == '.') {
                    isDouble = true;
                }
                else if (!char.IsDigit(c)) {
                    break;
                }
                res.Append(c);
                GetChar();
                c = PeekChar();
            }
            return new Token(res.ToString(),
                isDouble ? TokenType.Double : TokenType.Integer, CurrentLineNumber, CurrentLinePosition);
        }

        private void JumpToLineEnd() {
            var c = PeekChar();
            while (c != '\n') {
                GetChar();
                c = PeekChar();
            }
        }

        private Token GetOriginalStringToken() {
            var isDocString = false;
            var c = GetChar();
            if (c == Eof || c != '@') {
                throw new InvalidOperationException("Internal: parsing original string?");
            }

            c = PeekChar();
            if (c == Eof || c != '\"') {
                throw new InvalidOperationException("Internal: parsing original string?");
            }
            GetChar();

            c = PeekChar();
            if (c == '\"') {
                GetChar();
                c = PeekChar();
                if (c == '\"') {
                    GetChar();
                    isDocString = true;
                    c = PeekChar();
                }
            }

            var result = new StringBuilder();
            while (true) {
                if (c == Eof) {
                    throw new InvalidOperationException("Hit EOF in String literal.");
                }
                var quotesCount = 0;
                if (c == '\"') {
                    GetChar();
                    if (!isDocString) {
                        return new Token(result.ToString(), TokenType.String,
                            CurrentLineNumber, CurrentLinePosition);
                    }
                    quotesCount++;

                    c = PeekChar();
                    if (c == '\"') {
                        quotesCount++;
                        GetChar();
                        c = PeekChar();
                        if (c == '\"') {
                            quotesCount++;
                            GetChar();
                        }
                    }
                    if (quotesCount == 3) {
                        return new Token(result.ToString(), TokenType.String,
                            CurrentLineNumber, CurrentLinePosition);
                    }
                    result.Append('\"', quotesCount);
                }
                result.Append(c);
                GetChar();
                c = PeekChar();
            }
        }

        private Token GetStringToken() {
            var c = GetChar();
            if (c == Eof || c != '\"') {
                throw new InvalidOperationException(
                    "Internal: parsing string?");
            }
            var res = new StringBuilder();
            var escape = false;
            c = PeekChar();
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
                    GetChar();
                    escape = true;
                }
                else if (c == '"' && !escape) {
                    GetChar();
                    return new Token(res.ToString(), TokenType.String, CurrentLineNumber, CurrentLinePosition);
                }
                else if (escape) {
                    escape = false;
                    GetChar();
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
                    GetChar();
                    res.Append(c);
                }
                c = PeekChar();
            }
        }

        private char PeekChar() => (char)textReader.Peek();
    }
}