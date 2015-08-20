using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Test
{
    static class Program
    {
        private const int TEST_COUNT = 1;

        [STAThread]
        static void Main(string[] args) {
            var content = File.ReadAllText("test.json");
            FadeJsonTest(content);
            JsonDotNetTest(content);
            Console.ReadKey();
        }

        static void JsonDotNetTest(string content) {
            var stopwatch = new Stopwatch();
            var elapsedTime = new long[TEST_COUNT];
            for (var i = 0; i < TEST_COUNT; i++) {
                stopwatch.Start();
                var b = Newtonsoft.Json.Linq.JObject.Parse(content);
                var valueB = b["frameworks"]["dotnet"]["dependencies"]["System.Linq"];
                stopwatch.Stop();
                elapsedTime[i] = stopwatch.ElapsedMilliseconds;
                stopwatch.Reset();
            }
            Console.WriteLine($"Json.NET :{elapsedTime.Average()} ms");
        }

        static void FadeJsonTest(string content) {
            var stopwatch = new Stopwatch();
            var elapsedTime = new long[TEST_COUNT];
            for (var i = 0; i < TEST_COUNT; i++) {
                stopwatch.Start();
                var a = FadeJson.JsonObject.FromString(content);
                var valueA = a["frameworks"]["dotnet"]["dependencies"]["System.Linq"];
                stopwatch.Stop();
                elapsedTime[i] = stopwatch.ElapsedMilliseconds;
                stopwatch.Reset();
            }
            Console.WriteLine($"FadeJson :{elapsedTime.Average()} ms");
        }
    }
}
