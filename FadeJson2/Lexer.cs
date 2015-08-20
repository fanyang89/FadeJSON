using System;
using System.Data.SqlTypes;
using System.IO;
using System.Reflection.Emit;
using System.Text;

namespace FadeJson2
{
    public class Token<T>
    {
        public readonly T Value;

        public Token(T val) {
            Value = val;
        }
    }
    
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
        
        public Token<string> GetStringToken() {
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
                    return new Token<string>(res.ToString());
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

        public Token<int> GetIntToken() {
            // Check integrity before loop to avoid accidently returning zero.
            var c = GetChar();
            if (c == Eof || !char.IsDigit(c)) {
                throw new InvalidOperationException("Internal: lexing number?");
            }
            var digit = c - '0';
            var res = digit;
            c = PeekChar();
            while (c != Eof && char.IsDigit(c)) {
                res = res * 10 + (c - '0');
                GetChar();
                c = PeekChar();
            }
            return new Token<int>(res);
        }

        public dynamic GetToken() {
            var c = PeekChar();
            if (c == ' ' || c == '\t') {
                GetChar();
                return GetToken();
            }
            if (char.IsDigit(c)) {
                return GetIntToken();
            }
            if (c == '"') {
                return GetStringToken();
            }
            return new Token<string>(GetChar().ToString());
        }

        public static Lexer FromString(string content) => new Lexer(content);

        public static Lexer FromStream(Stream stream) => new Lexer(stream);

        public static Lexer FromFile(string filename) => new Lexer(new FileStream(filename, FileMode.Open));
    }
}