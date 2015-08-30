using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FadeJson2
{
    public class Lexer
    {
        public const char Eof = unchecked((char)-1);

        private readonly TextReader textReader;

        private Lexer(Stream stream) {
            textReader = new StreamReader(stream);
        }

        private Lexer(string content) {
            textReader = new StringReader(content);
        }

        private char GetChar() => (char)textReader.Read();

        private char PeekChar() => (char)textReader.Peek();

        public Token GetStringToken() {
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
                    return new Token(res.ToString(), TokenType.StringType);
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

        public Token GetIntToken() {
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
            return new Token(res.ToString(), TokenType.IntegerType);
        }


        public List<Token> GetAllTokens() {
            var tokens = new List<Token>();
            var token = NextToken();
            while (token != null) {
                tokens.Add(token);
                token = NextToken();
            }
            return tokens;
        }

        private readonly List<char> emptyCharList = new List<char> { ' ', '\r', '\n', '\t' };

        private readonly List<char> keyCharList = new List<char> { '{', '}', ':', ',', '[', ']' };

        public Token NextToken() {
            var c = PeekChar();
            if (keyCharList.Contains(c)) {
                GetChar();
                return new Token(c, TokenType.SyntaxType);
            }
            if (emptyCharList.Contains(c)) {
                GetChar();
                return NextToken();
            }
            if (char.IsDigit(c)) {
                return GetIntToken();
            }
            if (c == '"') {
                return GetStringToken();
            }
            return null;
        }

        public static Lexer FromString(string content) => new Lexer(content);

        public static Lexer FromStream(Stream stream) => new Lexer(stream);

        public static Lexer FromFile(string filename) => new Lexer(new FileStream(filename, FileMode.Open));
    }
}