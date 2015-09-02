using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Test
{
    class Program
    {
        static string JsonFileContent;

        static void Main(string[] args) {
            JsonFileContent = File.ReadAllText("testSuite.json");
            int fadeRank = FadeJson2Test();
            int jsonDotNet = JsonDotNetTest();
            DrawLine();
            Console.WriteLine("FadeJson2 Test: {0}ms", fadeRank);
            Console.WriteLine("Json.NET Test: {0}ms", jsonDotNet);
            DrawLine();
            Console.WriteLine("Json.Net比FadeJson2快{0}ms", fadeRank - jsonDotNet);
            Console.ReadKey();
        }

        static void DrawLine() {
            for (int i = 0; i < Console.WindowWidth; i++) {
                Console.Write("=");
            }
        }

        static int FadeJson2Test() {
            DrawLine();
            var sw = new Stopwatch();

            sw.Start();
            var j = FadeJson2.JsonObject.FromString(JsonFileContent);
            var description = j["description"];
            var linqVersion = j["frameworks"]["dotnet"]["dependencies"]["System.Linq"];
            sw.Stop();

            Console.WriteLine("Read From Test Suite:");
            Console.WriteLine($"description: {description}");
            Console.WriteLine($"linqVersion: {linqVersion}");

            return sw.Elapsed.Milliseconds;
        }

        static int JsonDotNetTest() {
            DrawLine();
            var sw = new Stopwatch();

            sw.Start();
            var j = Newtonsoft.Json.Linq.JObject.Parse(JsonFileContent);
            var description = j["description"];
            var linqVersion = j["frameworks"]["dotnet"]["dependencies"]["System.Linq"];
            sw.Stop();

            Console.WriteLine("Read From Test Suite:");
            Console.WriteLine($"description: {description}");
            Console.WriteLine($"linqVersion: {linqVersion}");

            return sw.Elapsed.Milliseconds;
        }
    }
}
