using System;
using FadeJSON.Serialization.Serializers;

namespace FadeJSON.Serialization
{
    internal class SerializerCache<T>
    {
        private static readonly Lazy<Serializer<T>> _default = new Lazy<Serializer<T>>(() => {
            object serializer = null;
            var t = typeof(T);
            if (t == typeof(int)) {
                serializer = new IntSerializer();
            }
            else if (t == typeof(string)) {
                serializer = new StringSerializer();
            }
            else if (t == typeof(double)) {
                serializer = new DoubleSerializer();
            }
            else if (t == typeof(float)) {
                serializer = new FloatSerializer();
            }
            else if (t == typeof(long)) {
                serializer = new LongSerializer();
            }
            return (Serializer<T>) serializer;
        });

        public static Serializer<T> Default => _default.Value;
    }
}