namespace FadeJSON.Serialization.Serializers
{
    internal class LongSerializer : Serializer<long>
    {
        public override string Serialize(long obj) {
            return obj.ToString();
        }
    }
}