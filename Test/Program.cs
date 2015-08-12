using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FadeJson;
using Newtonsoft.Json;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args) {
            var content = File.ReadAllText("test.json");
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var j = Json.Parse(content);
            stopwatch.Stop();
            Console.WriteLine("FadeJson: {0}ms", stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();

            stopwatch.Start();
            var jobj = JObject.Parse(content);
            stopwatch.Stop();
            Console.WriteLine("Newtonsoft.Json: {0}", stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();

            Console.ReadKey();
        }
    }
}
