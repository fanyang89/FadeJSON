using System.IO;

namespace FadeJSON
{
    public class Json
    {
        public static JsonObject FromStream(Stream stream) {
            return new StreamParser(new StreamReader(stream)).Parse();
        }

        public static JsonObject FromFile(string path) {
            return new StreamParser(path).Parse();
        }

        public static JsonObject FromString(string content) {
            return new StringParser(content).Parse();
        }
    }
}