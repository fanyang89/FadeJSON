using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime;


namespace FadeJson
{
    public class Json
    {
        public static dynamic Parse(string jsonContent) {
            var inputStream = new AntlrInputStream(jsonContent);
            var lexer = new JsonLexer(inputStream);
            var tokenList = new CommonTokenStream(lexer);
            var parser = new JsonParser(tokenList) { BuildParseTree = true };
            var tree = parser.json();
            var visitor = new Visitor();
            return visitor.VisitJson(tree);
        }
    }
}
