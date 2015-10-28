using System;
using System.IO;

namespace FadeJson
{
    public class CommonCache<T>
    {
        readonly T[] buffer;
        readonly int maxBufferSize;
        int bufferIndex;
        public bool IsCacheCompleted;

        public CommonCache(int maxBufferSize = 1024) {
            this.maxBufferSize = maxBufferSize;
            buffer = new T[this.maxBufferSize];
            FlushBuffer();
        }

        public bool IsReadToEnd => (IsCacheCompleted) && (bufferIndex + 1) >= maxBufferSize;

        public T Get() {
            if (bufferIndex >= maxBufferSize) {
                FlushBuffer();
                bufferIndex = 0;
            }
            return buffer[bufferIndex++];
        }

        public T Peek(int index = 0) {
            var newIndex = bufferIndex + index;
            if (newIndex >= maxBufferSize || newIndex < 0) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return buffer[newIndex];
        }

        private void FlushBuffer() {

        }
    }
}