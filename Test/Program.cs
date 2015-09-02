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
            Console.WriteLine("FadeJson2 Test: {0}ms", FadeJson2Test());
            Console.WriteLine("Json.NET Test: {0}ms", JsonDotNetTest());
            Console.ReadKey();
        }

        static int FadeJson2Test() {
            var sw = new Stopwatch();

            sw.Start();
            var j = FadeJson2.JsonObject.FromString(JsonFileContent);
            var description = j["description"];
            var linqVersion = j["frameworks"]["dotnet"]["dependencies"]["System.Linq"];
            sw.Stop();

            //Console.WriteLine($"description: {description}");
            //Console.WriteLine($"linqVersion: {linqVersion}");

            return sw.Elapsed.Milliseconds;
        }

        static int JsonDotNetTest() {
            var sw = new Stopwatch();

            sw.Start();
            var j = Newtonsoft.Json.Linq.JObject.Parse(JsonFileContent);
            var description = j["description"];
            var linqVersion = j["frameworks"]["dotnet"]["dependencies"]["System.Linq"];
            sw.Stop();

            //Console.WriteLine($"description: {description}");
            //Console.WriteLine($"linqVersion: {linqVersion}");

            return sw.Elapsed.Milliseconds;
        }
    }
}
