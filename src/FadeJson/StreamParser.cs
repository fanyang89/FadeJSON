using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace FadeJSON
{
    internal class StreamParser : IDisposable
    {
        private readonly StreamReader _reader;
        private readonly char[] _buffer;
        private const int MaxBufferSize = 256;

        private uint _pos;

        private uint Pos {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return _pos;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set {
                if (value >= MaxBufferSize) {
                    var rest = value % MaxBufferSize;
                    var length = _reader.Read(_buffer, 0, MaxBufferSize);
                    if (length < MaxBufferSize) {
                        _buffer[length] = '\0';
                    }
                    _pos = rest;
                } else {
                    _pos = value;
                }
            }
        }

        public StreamParser(StreamReader reader) {
            _reader = reader;
            _buffer = new char[MaxBufferSize];
            _reader.Read(_buffer, 0, MaxBufferSize);
        }

        //public StreamParser(string path) {
        //    _buffer = new char[MaxBufferSize];
        //    _reader = new StreamReader(new FileStream(path,
        //        FileMode.Open, FileAccess.Read, FileShare.Read, 8192, FileOptions.SequentialScan));
        //    _reader.Read(_buffer, 0, MaxBufferSize);
        //}
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SkipWhitespace() {
            while (Pos + 1 < MaxBufferSize
                   && _buffer[Pos].IsWhiteChar()
                   && _buffer[Pos + 1].IsWhiteChar()) {
                Pos += 2;
            }
            while (Pos < MaxBufferSize && _buffer[Pos].IsWhiteChar()) {
                Pos++;
            }
        }

        private JsonObject ParseObject() {
            Pos++; // skip '{'
            SkipWhitespace();
            var values = new Dictionary<string, JsonObject>();
            if (Peek == '}') {
                Pos++;
                return new JsonObject(values);
            }
            for (;;) {
                SkipWhitespace();
                var key = ParseString();
                SkipWhitespace();
                if (Peek != ':') {
                    throw new FormatException();
                }
                Pos++;
                SkipWhitespace();
                var value = ParseValue();
                SkipWhitespace();
                values.Add(key, value);
                if (Peek == '}') {
                    Pos++;
                    break;
                }
                if (Peek == ',') {
                    Pos++;
                }
            }
            return new JsonObject(values);
        }


        private uint ParseFastNumber(ref uint result) {
            var begin = Pos;
            while (Pos + 4 < MaxBufferSize
                && _buffer[Pos].IsDigit()
                && _buffer[Pos + 1].IsDigit()
                && _buffer[Pos + 2].IsDigit()
                && _buffer[Pos + 3].IsDigit()) {
                result = (uint)(_buffer[Pos] * 1000
                    + _buffer[Pos + 1] * 100
                    + _buffer[Pos + 2] * 10
                    + _buffer[Pos + 3]
                    - 53328
                    + result * 10000);
                Pos += 4;
            }
            while (Pos + 2 < MaxBufferSize
                 && _buffer[Pos].IsDigit()
                 && _buffer[Pos + 1].IsDigit()) {
                result = (uint)(_buffer[Pos] * 10
                                 + _buffer[Pos + 1]
                                 - 528
                                 + result * 100);
                Pos += 2;
            }
            while (Pos < MaxBufferSize && _buffer[Pos].IsDigit()) {
                result = (uint)(_buffer[Pos] - 48 + result * 10);
                Pos++;
            }
            return Pos - begin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint GetUintLength(uint i) {
            if (i == 0)
                return 0;
            if (i >= 1 && i <= 9)
                return 1;
            if (i >= 10 && i <= 99)
                return 2;
            if (i >= 100 && i <= 999)
                return 3;
            if (i >= 1000 && i <= 9999)
                return 4;
            if (i >= 10000 && i <= 99999)
                return 5;
            if (i >= 100000 && i <= 999999)
                return 6;
            if (i >= 1000000 && i <= 9999999)
                return 7;
            if (i >= 10000000 && i <= 99999999)
                return 8;
            if (i >= 100000000 && i <= 999999999)
                return 9;
            return 10;
        }

        private double ParseDouble() {
            uint part1 = 0;
            uint part2 = 0;
            var firstChar = Peek;
            var part1Length = ParseFastNumber(ref part1);
            //if (part1Length - GetUintLength(part1)  == 1 && part1 != 0 && firstChar == '0') {
            //    throw new FormatException("Number must start with a non-zero digit.");
            //}
            if (part1 != 0 && firstChar == '0') {
                throw new FormatException("Number must start with a non-zero digit.");
            }
            double result = part1;
            if (Peek == '.') {
                Pos++;
                var part2Length = ParseFastNumber(ref part2);
                result += part2 * Math.Pow(10, -part2Length);
            }
            if (Peek == 'e' || Peek == 'E') {
                Pos++;
                uint part3 = 0;
                switch (Peek) {
                    case '+':
                        Pos++;
                        if (ParseFastNumber(ref part3) == 0) {
                            throw new FormatException("Char after 'e' must be a number.");
                        }
                        result *= Math.Pow(10, part3);
                        break;
                    case '-':
                        Pos++;
                        if (ParseFastNumber(ref part3) == 0) {
                            throw new FormatException("Char after 'e' must be a number.");
                        }
                        result *= Math.Pow(10, -part3);
                        break;
                    default:
                        if (ParseFastNumber(ref part3) == 0) {
                            throw new FormatException("Char after 'e' must be a number.");
                        }
                        result *= Math.Pow(10, part3);
                        break;
                }
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonObject ParseValue() {
            SkipWhitespace();
            var la = Peek;
            switch (la) {
                case 'n':
                    Pos += 4;
                    return null;
                case 't':
                    Pos += 4;
                    return new JsonObject(true);
                case 'f':
                    Pos += 5;
                    return new JsonObject(false);
                case '{':
                    return ParseObject();
                case '[':
                    return ParseArray();
                case '"':
                    return new JsonObject(ParseString());
                case '-':
                    Pos++;
                    return new JsonObject(-ParseDouble());
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return new JsonObject(ParseDouble());
                default:
                    throw new FormatException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char EscapeChar(char c) {
            switch (c) {
                case 'n':
                    return '\n';
                case 'r':
                    return '\r';
                case 't':
                    return '\t';
                case '"':
                    return '\"';
                case '\\':
                    return '\\';
                case '/':
                    return '/';
                case 'b':
                    return '\b';
                case 'f':
                    return '\f';
                default:
                    throw new FormatException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte HexToByte(char c) {
            switch (c) {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'A':
                case 'a':
                    return 10;
                case 'B':
                case 'b':
                    return 11;
                case 'C':
                case 'c':
                    return 12;
                case 'D':
                case 'd':
                    return 13;
                case 'E':
                case 'e':
                    return 14;
                case 'F':
                case 'f':
                    return 15;
                default:
                    throw new FormatException();
            }
        }

        private unsafe string ParseString() {
            Pos++; //skip '"'
            if (Peek == '"') {
                Pos++;
                return string.Empty;
            }
            var sb = new StringBuilder();
            for (;;) {
                if (Peek == '\\') {
                    Pos++;
                    if (Peek == 'u') {
                        Pos++;
                        fixed (char* c = &_buffer[Pos]) {
                            var r = (char)(HexToByte(*c) * 4096 + HexToByte(c[1]) * 256 + HexToByte(c[2]) * 16 + HexToByte(c[3]));
                            sb.Append(r);
                        }
                        Pos += 4;
                    } else {
                        sb.Append(EscapeChar(Peek));
                        Pos++;
                    }
                } else if (Peek == '"') {
                    break;
                } else if (Peek == '\n') {
                    throw new FormatException("Hit \\n in a string literal.");
                } else if (Peek == '\t') {
                    throw new FormatException("Hit \\t in a string literal.");
                } else {
                    sb.Append(Peek);
                    Pos++;
                }
            }
            Pos++; //skip '"'
            return sb.ToString();
        }

      private JsonObject ParseArray() {
            Pos++; //skip '['
            SkipWhitespace();
            var list = new List<JsonObject>();
            if (Peek == ']') {
                Pos++;
                return new JsonObject(list);
            }
            for (;;) {
                SkipWhitespace();
                list.Add(ParseValue());
                SkipWhitespace();
                if (Peek == ']') {
                    Pos++;
                    break;
                }
                if (Peek == ',') {
                    Pos++;
                }
            }
            return new JsonObject(list);
        }

        public JsonObject Parse() {
            SkipWhitespace();
            JsonObject json;
            switch (Peek) {
                case '{':
                    json = ParseObject();
                    break;
                case '[':
                    json = ParseArray();
                    break;
                default:
                    throw new FormatException("JSON entry must be an object or array.");
            }
            SkipWhitespace();
            if (Peek != '\0') {
                throw new FormatException();
            }
            return json;
        }

        private char Peek {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return _buffer[Pos];
            }
        }

        public void Dispose() {
            _reader.Dispose();
        }
    }
}