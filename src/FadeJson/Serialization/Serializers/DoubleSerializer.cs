using System.Globalization;

namespace FadeJSON.Serialization.Serializers
{
    internal class DoubleSerializer : Serializer<double>
    {
        public override string Serialize(double obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }
    }
}