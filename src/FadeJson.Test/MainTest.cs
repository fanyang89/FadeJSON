using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FadeJson2;

namespace Test
{
    static class MainTest
    {
        public static void Main(string[] args) {
            var lexer = Lexer.FromFile("TestSuite.json");
            var tokens = lexer.GetAllTokens();
            ;
            Console.ReadKey();
        }
    }
}
