using FadeJSON.Serialization.Serializers;

namespace FadeJSON.Serialization
{
    internal class Serializer<T> : ISerialize<T>
    {
        public virtual string Serialize(T obj) {
            return ReferenceEquals(obj, null) ? "null" : SerializerCache<T>.Default.Serialize(obj);
        }
    }

    public static class Serializer
    {
        public static string SerializeDynamic(object obj) {
            return DefaultSerializer.Default.Serialize(obj);
        }
    }
}