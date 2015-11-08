using System;
using System.Diagnostics;

namespace FadeJson
{
    public class Program
    {
        public static void Main(string[] args) {
            var s = new Stopwatch();
            s.Start();
            var j = JsonValue.FromFile("Test/data.json");
            s.Stop();
            AllCount(j);
            Console.WriteLine($"{s.ElapsedMilliseconds}ms ItemCount:{acc}");

            Console.ReadKey();
        }

        static int acc = 0;
        private static void AllCount(JsonValue j) {
            acc += j.Count;
            foreach (var value in j.Values) {
                AllCount(value);
            }
        }
    }
}