using System;
using System.Collections.Generic;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime;
// ReSharper disable UnusedMember.Global

namespace FadeJson
{
    public class JsonObject
    {
        private readonly Dictionary<string, dynamic> _dict = new Dictionary<string, dynamic>();

        public void AddKeyValue(string key, dynamic value) {
            _dict.Add(key, value);
        }

        public dynamic this[string key] {
            get {
                return _dict[key];
            }
            set {
                _dict[key] = value;
            }
        }

        public static dynamic FromString(string jsonContent) {
            var inputStream = new AntlrInputStream(jsonContent);
            var lexer = new JsonLexer(inputStream);
            var tokenList = new CommonTokenStream(lexer);
            var parser = new JsonParser(tokenList) { BuildParseTree = true };
            var tree = parser.json();
            var visitor = new JsonVisitor();
            return visitor.VisitJson(tree);
        }
    }
}