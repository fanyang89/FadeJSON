using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FadeJson
{
    public class CharCache : ICommonCache<char, TextReader>
    {
        private readonly char[] buffer;
        private int CharPosition = 1;
        private readonly int bufferMax;
        private readonly TextReader textReader;
        private int pos;
        private readonly int doubleBufferMax;

        public CharCache(TextReader textReader, int bufferSize = 8) {
            this.textReader = textReader;
            bufferMax = bufferSize;
            buffer = new char[2 * bufferSize];
            doubleBufferMax = 2 * bufferMax;
            FlushBuffer(0, doubleBufferMax);
        }

        public char Lookahead(int index = 0) {
            if (index < 0 || index > bufferMax) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (pos >= doubleBufferMax) {
                pos = 0;
                FlushBuffer(bufferMax, doubleBufferMax);
            }
            return buffer[(pos + index) % (doubleBufferMax)];
        }

        public bool Check(IEnumerable<char> list) {
            var p = 0;
            return list.All(i => i.Equals(Lookahead(p++)));
        }

        public char Next() {
            if (pos == bufferMax) {
                FlushBuffer(0, bufferMax);
                CharPosition++;
                return buffer[pos++];
            }
            if (pos >= doubleBufferMax) {
                pos = 0;
                FlushBuffer(bufferMax, doubleBufferMax);
                CharPosition++;
                return buffer[pos++];
            }
            CharPosition++;
            return buffer[pos++];
        }

        public void Next(int count) {
            for (var i = 0; i < count; i++) {
                Next();
            }
        }

        private void FlushBuffer(int from, int to) {
            for (var i = from; i < to; i++) {
                buffer[i] = unchecked((char)textReader.Read());
                if (buffer[i] == unchecked((char)-1)) {
                    break;
                }
            }
        }
    }
}