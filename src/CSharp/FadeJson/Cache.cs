using System;
using System.Collections.Generic;
using System.Linq;

namespace FadeJson
{
    public class Cache<TSource, TResult> where TResult : IEquatable<TResult>
    {
        private readonly TResult[] _buffer;
        private readonly int _bufferMax;
        private readonly int _doubleBufferMax;
        private readonly ICacheSource<TSource, TResult> _source;

        private int _index;

        public Cache(ICacheSource<TSource, TResult> source, int bufferSize = 8) {
            _source = source;
            _bufferMax = bufferSize;
            _doubleBufferMax = 2 * _bufferMax;

            _buffer = new TResult[2 * bufferSize];
            Flush(0, _doubleBufferMax);
        }

        public TResult Eof => _source.Eof;
        public bool IsCompleted { get; private set; }
        public int CharIndex { get; private set; }

        public bool CheckLookaheads(IEnumerable<char> list) {
            var pos = 0;
            return list.All(i => i.Equals(Peek(pos++)));
        }

        public TResult Consume() {
            TResult result;
            if (_index == _bufferMax) {
                Flush(0, _bufferMax);
                CharIndex++;
                result = _buffer[_index++];
            } else if (_index >= _doubleBufferMax) {
                _index = 0;
                Flush(_bufferMax, _doubleBufferMax);
                CharIndex++;
                result = _buffer[_index++];
            } else {
                CharIndex++;
                result = _buffer[_index++];
            }
            return result;
        }

        public void Consume(int count) {
            for (var i = 0; i < count; i++) {
                Consume();
            }
        }

        public TResult Peek(int index = 0) {
            if (index < 0 || index > _bufferMax) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (_index >= _doubleBufferMax) {
                _index = 0;
                Flush(_bufferMax, _doubleBufferMax);
            }
            return _buffer[(_index + index) % _doubleBufferMax];
        }
        
        private void Flush(int from, int to) {
            _source.ReadBlock(_buffer, from, to);
        }
    }
}