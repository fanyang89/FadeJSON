using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FadeJson;

namespace Test
{
    class Program
    {
        static void Main(string[] args) {
            var content = File.ReadAllText("test.json");

            var j = Json.Parse(content);
            Console.ReadKey();
        }
    }
}
