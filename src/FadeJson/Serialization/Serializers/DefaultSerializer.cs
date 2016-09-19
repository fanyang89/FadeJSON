using System;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using System.Globalization;

namespace FadeJSON.Serialization.Serializers
{
    public class DefaultSerializer
    {
        private DefaultSerializer() { }
        private static readonly Lazy<DefaultSerializer> _default = new Lazy<DefaultSerializer>(() => new DefaultSerializer());
        public static DefaultSerializer Default => _default.Value;

        public string Serialize(object obj) {
            if (ReferenceEquals(null, obj)) {
                return "null";
            }
            var t = obj.GetType();

            if (t == typeof(string)) {
                return $"\"{(string)obj}\"";
            }

            if (t == typeof(bool)) {
                return (bool)obj ? "true" : "false";
            }

            if (t == typeof(double)) {
                return ((double)obj).ToString(CultureInfo.InvariantCulture);
            }
            if (t == typeof(float)) {
                return ((float)obj).ToString(CultureInfo.InvariantCulture);
            }
            if (t == typeof(int)) {
                return ((int)obj).ToString();
            }

            if (t == typeof(long)) {
                return ((long)obj).ToString();
            }
            if (t == typeof(char)) {
                return new string((char)obj, 1);
            }
            if (t == typeof(short)) {
                return ((short)obj).ToString();
            }
            if (t == typeof(sbyte)) {
                return ((sbyte)obj).ToString();
            }

            if (t == typeof(byte)) {
                return ((byte)obj).ToString();
            }
            if (t == typeof(ushort)) {
                return ((ushort)obj).ToString();
            }
            if (t == typeof(uint)) {
                return ((uint)obj).ToString();
            }
            if (t == typeof(ulong)) {
                return ((ulong)obj).ToString();
            }


            if (t.IsArray) {
                var array = (Array)obj;
                var length = array.Length;
                if (length == 0) {
                    return "[]";
                }
                if (length == 1) {
                    var value = array.GetValue(0);
                    var str = Serialize(value);

                    return $"[{str}]";
                }

                var sb = new StringBuilder('[');
                for (var i = 0; i < length - 1; i++) {
                    sb.Append(array.GetValue(i).Serialize()).Append(',');
                }
                sb.Append(array.GetValue(length - 1).Serialize()).Append(']');

                return sb.ToString();
            }

            if (t.IsClass) {
                var sb = new StringBuilder();
                var properties = t.GetProperties();
                if (properties.Length == 0) {
                    return "{}";
                }
                sb.Append('{');
                foreach (var property in properties) {
                    var name = property.Name;
                    var value = property.GetValue(obj);
                    var str = Serialize(value);
                    sb.Append('"').Append(name).Append('"').Append(':').Append(str).Append(',');
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append('}');
                return sb.ToString();
            }




            throw new NotSupportedException($"Cannot Serialize type {t.Name}");
        }
    }
}