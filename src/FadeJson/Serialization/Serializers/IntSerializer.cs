namespace FadeJSON.Serialization.Serializers
{
    internal class IntSerializer : Serializer<int>
    {
        public override string Serialize(int obj) {
            return obj.ToString();
        }
    }
}