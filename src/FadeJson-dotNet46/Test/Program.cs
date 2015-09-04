using System;
using System.Diagnostics;
using System.IO;

namespace Test
{
    internal class Program
    {
        private static string JsonFileContent;

        private static void DrawLine() {
            for (int i = 0; i < Console.WindowWidth; i++) {
                Console.Write("=");
            }
        }

        private static int FadeJson2Test() {
            DrawLine();
            var sw = new Stopwatch();

            sw.Start();
            var j = FadeJson2.JsonValue.FromString(JsonFileContent);
            var description = j["description"];
            var linqVersion = j["frameworks"]["dotnet"]["dependencies"]["System.Linq"];
            sw.Stop();

#if CONTENT_TEST
            Console.WriteLine("Read From Test Suite:");
            Console.WriteLine($"description: {description}");
            Console.WriteLine($"linqVersion: {linqVersion}");
#endif

            return sw.Elapsed.Milliseconds;
        }

        private static int JsonDotNetTest() {
            DrawLine();
            var sw = new Stopwatch();

            sw.Start();
            var j = Newtonsoft.Json.Linq.JObject.Parse(JsonFileContent);
            var description = j["description"];
            var linqVersion = j["frameworks"]["dotnet"]["dependencies"]["System.Linq"];
            sw.Stop();

#if CONTENT_TEST
            Console.WriteLine("Read From Test Suite:");
            Console.WriteLine($"description: {description}");
            Console.WriteLine($"linqVersion: {linqVersion}");
#endif

            return sw.Elapsed.Milliseconds;
        }

        private static void Main(string[] args) {
            JsonFileContent = File.ReadAllText("testSuite.json");

            Console.WriteLine("Test Content: Read two items from the same file.");

            int fadeRank = FadeJson2Test();
            Console.WriteLine("FadeJson2 Test: {0}ms", fadeRank);

            int jsonDotNet = JsonDotNetTest();
            Console.WriteLine("Json.NET Test: {0}ms", jsonDotNet);

            Console.ReadKey();
        }
    }
}