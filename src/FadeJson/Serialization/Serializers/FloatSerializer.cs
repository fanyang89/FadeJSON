using System.Globalization;

namespace FadeJSON.Serialization.Serializers
{
    internal class FloatSerializer : Serializer<float>
    {
        public override string Serialize(float obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }
    }
}