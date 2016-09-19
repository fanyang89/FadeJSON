using System.IO;
using FadeJSON.Serialization;

namespace FadeJSON
{
    public class Json
    {
        public static JsonObject FromStream(Stream stream) {
            using (var reader = new StreamReader(stream))
            using (var parser = new StreamParser(reader)) {
                return parser.Parse();
            }
        }

        public static JsonObject FromFile(string path) {
            const int bufferSize = 8192;
            using (var fileStream = new FileStream(path,
                FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.SequentialScan))
            using (var reader = new StreamReader(fileStream))
            using (var parser = new StreamParser(reader)) {
                return parser.Parse();
            }
        }

        public static JsonObject FromString(string content) {
            return new StringParser(content).Parse();
        }

        public static string SerializeDynamic(object obj) {
            return Serializer.SerializeDynamic(obj);
        }

        public static string Serialize<T>(T obj) {
            return SerializerCache<T>.Default.Serialize(obj);
        }
    }
}