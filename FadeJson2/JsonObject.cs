using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FadeJson2
{
    public class JsonObject
    {
        public static JsonObject FromFile(string filename) {
            var lexer = Lexer.FromFile(filename);
            var parser = new Parser(lexer);
            return parser.Parse();
        }
    }
}
