using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args) {
            var content = File.ReadAllText("test.json");
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var a = FadeJson.JsonObject.FromString(content);
            var valueA = a["frameworks"]["dotnet"]["dependencies"]["System.Linq"];
            stopwatch.Stop();
            Console.WriteLine("FadeJson: {0}ms", stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();

            stopwatch.Start();
            var b = Newtonsoft.Json.Linq.JObject.Parse(content);
            var valueB = b["frameworks"]["dotnet"]["dependencies"]["System.Linq"];
            stopwatch.Stop();
            Console.WriteLine("Newtonsoft.Json: {0}ms", stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();

            Console.ReadKey();
        }
    }
}
