namespace FadeJSON.Serialization.Serializers
{
    internal class StringSerializer : Serializer<string>
    {
        public override string Serialize(string obj) {
            return obj;
        }
    }
}