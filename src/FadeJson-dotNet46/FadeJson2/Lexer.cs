using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FadeJson
{
    public class Lexer
    {
        public const char Eof = unchecked((char)-1);

        private const string EmptyCharList = " \r\n\t";
        private const string KeyCharList = "{}:,[]";
        readonly TextReader textReader;
        public int LineNumber { get; set; }
        public int LinePosition { get; set; }

        Lexer(Stream stream) {
            textReader = new StreamReader(stream);
        }

        Lexer(string content) {
            textReader = new StringReader(content);
        }

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
                LineNumber++;
                LinePosition = 0;
            }
            if (KeyCharList.Contains(new string(c, 1))) {
                GetChar();
                return new Token(c, TokenType.SyntaxType, LineNumber, LinePosition);
            }
            if (EmptyCharList.Contains(new string(c, 1))) {
                GetChar();
                return NextToken();
            }
            if (char.IsDigit(c)) {
                return GetIntToken();
            }
            if (c == '"') {
                return GetStringToken();
            }
            if (c == 't' || c == 'f') {
                return GetBoolToken();
            }

            return null;
        }

        Token? GetBoolToken() {
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
                            return new Token("true", TokenType.BoolType, LineNumber, LinePosition);
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
                                return new Token("false", TokenType.BoolType, LineNumber, LinePosition);
                            }
                        }
                    }
                }
            }
            throw new NotImplementedException();
        }

        char GetChar() => (char)textReader.Read();

        Token GetIntToken() {
            // Check integrity before loop to avoid accidently returning zero.
            var c = GetChar();
            if (c == Eof || !char.IsDigit(c)) {
                throw new InvalidOperationException("Internal: lexing number?");
            }
            var res = new StringBuilder();
            res.Append(c);
            c = PeekChar();
            while (c != Eof && char.IsDigit(c)) {
                res.Append(c);
                GetChar();
                c = PeekChar();
            }
            return new Token(res.ToString(), TokenType.IntegerType, LineNumber, LinePosition);
        }

        Token GetStringToken() {
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
                    return new Token(res.ToString(), TokenType.StringType, LineNumber, LinePosition);
                }
                else if (escape) {
                    escape = false;
                    GetChar();
                    switch (c) {
                        case 'n':
                            res.Append('\n');
                            break;

                        case 't':
                            res.Append('\t');
                            break;

                        case 'r':
                            res.Append('\r');
                            break;

                        case '\"':
                            res.Append('\"');
                            break;

                        case '\\':
                            res.Append('\\');
                            break;
                    }
                }
                else {
                    GetChar();
                    res.Append(c);
                }
                c = PeekChar();
            }
        }

        char PeekChar() => (char)textReader.Peek();
    }
}