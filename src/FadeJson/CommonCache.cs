using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FadeJson
{
    public class CommonCache : ICommonCache<Char, TextReader>
    {
        private readonly TextReader textReader;

        private readonly char[] buffer;
        private int pos;

        private readonly int bufferMax;

        public CommonCache(TextReader textReader, int bufferSize = 8) {
            this.textReader = textReader;
            bufferMax = bufferSize;
            buffer = new char[2 * bufferSize];
            FlushBuffer(0, bufferMax);
            FlushBuffer(bufferMax, 2 * bufferMax);
        }

        public char Lookahead(int index = 0) {
            if (index < 0 || index > bufferMax) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (pos >= 2 * bufferMax) {
                pos = 0;
                FlushBuffer(bufferMax, 2 * bufferMax);
            }
            return buffer[pos + index];
        }

        public bool Check(IEnumerable<char> list) {
            var p = 0;
            return list.All(i => i.Equals(Lookahead(p++)));
        }

        public char Next() {
            if (pos == bufferMax) {
                FlushBuffer(0, bufferMax);
                return buffer[pos++];
            }
            if (pos >= 2 * bufferMax) {
                pos = 0;
                FlushBuffer(bufferMax, 2 * bufferMax);
                return buffer[pos++];
            }
            return buffer[pos++];
        }

        public void Next(int count) {
            for (int i = 0; i < count; i++) {
                Next();
            }
        }

        private void FlushBuffer(int from, int to) {
            for (int i = from; i < to; i++) {
                buffer[i] = unchecked((char)textReader.Read());
                if (buffer[i] == unchecked((char)-1)) {
                    break;
                }
            }
        }
    }
}
