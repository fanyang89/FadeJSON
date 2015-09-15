using System.Reflection;

namespace FadeJson
{
    public class Deserializer
    {
        public static JsonValue Parse(object obj) {
            var j = new JsonValue(JsonValueType.Object);
            if (obj == null) {
                return null;
            }
            if (obj is int) {
                return new JsonValue((int) obj);
            }
            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            if (obj is string) {
                return new JsonValue((string) obj);
            }
            if (obj is bool) {
                return new JsonValue((bool) obj);
            }
            var t = obj.GetType();
            var members = t.GetMembers();
            foreach (var member in members) {
                if (member.MemberType != MemberTypes.Field) continue;
                var name = member.Name;
                var value = Parse(t.GetField(name).GetValue(obj));
                j.AddKeyValue(name, value);
            }
            return j;
        }
    }
}