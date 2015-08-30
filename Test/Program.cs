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
            const int TEST_COUNT = 1;
            JsonFileContent = File.ReadAllText("testSuite.json");

            Console.WriteLine("===========FadeJson2 Test===========");
            var fadeRankList = new List<int>(TEST_COUNT);
            for (int i = 0; i < TEST_COUNT; i++) {
                var rank = FadeJson2Test();
                fadeRankList.Add(rank);
                Console.WriteLine($"NO.{i} {rank} ms");
            }
            Console.WriteLine($"Avg: {fadeRankList.Average()}");

            Console.WriteLine("===========Json.NET Test===========");
            var jsonDotNetRankList = new List<int>(TEST_COUNT);
            for (int i = 0; i < TEST_COUNT; i++) {
                var rank = JsonDotNetTest();
                jsonDotNetRankList.Add(rank);
                Console.WriteLine($"NO.{i} {rank} ms");
            }
            Console.WriteLine($"Avg: {jsonDotNetRankList.Average()}");

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
