#define CONTENT_TEST
using System;
using System.Diagnostics;
using System.IO;
using FadeJson;
using Newtonsoft.Json.Linq;

namespace Test
{
    internal class Program
    {
        private static string jsonFileContent;

        private static void DrawLine() {
            for (var i = 0; i < Console.WindowWidth; i++) {
                Console.Write("=");
            }
        }

        private static int FadeJson2Test() {
            DrawLine();
            var sw = new Stopwatch();

            sw.Start();
            var j = JsonValue.FromString(jsonFileContent);
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
            var j = JObject.Parse(jsonFileContent);
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

        /// <summary>
        /// 简易正确性测试（不测试object和array）
        /// </summary>
        private static void CorrectnessTest() {
            DrawLine();
            Console.WriteLine("CorrectnessTest begin.");
            var jsonnet = JObject.Parse(jsonFileContent);
            var fadejson = FadeJson.JsonValue.FromString(jsonFileContent);
            foreach (var key in fadejson.Keys) {
                var item = fadejson[key];
                if (item.JsonValueType == JsonValueType.Object) continue;
                if (item.JsonValueType == JsonValueType.Array) {
                    continue;
                }
                var correct = jsonnet[key];
                if (item.ToString() != correct.ToString()) {
                    throw new InvalidOperationException();
                }
            }
            Console.WriteLine("CorrectnessTest completed.");
        }



        private static void Main(string[] args) {
            jsonFileContent = File.ReadAllText("testSuite.json");

            Console.WriteLine("Test Content: Read two items from the same file.");

            var fadeRank = FadeJson2Test();
            Console.WriteLine("FadeJson2 Test: {0}ms", fadeRank);

            var jsonDotNet = JsonDotNetTest();
            Console.WriteLine("Json.NET Test: {0}ms", jsonDotNet);

            CorrectnessTest();

            Console.ReadKey();
        }
    }
}