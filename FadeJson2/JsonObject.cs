using System.Collections.Generic;

namespace FadeJson2
{
    public class JsonObject
    {
        private readonly Dictionary<string, dynamic> dict = new Dictionary<string, dynamic>();

        public void AddKeyValue(string key, dynamic value) {
            dict.Add(key, value);
        }

        public void AddKeyValue(KeyValuePair<string, dynamic>? pair) {
            if (pair.HasValue)
                dict.Add(pair.Value.Key, pair.Value.Value);
        }

        public dynamic this[string key] {
            get {
                return dict[key];
            }
            set {
                dict[key] = value;
            }
        }
        
        public static JsonObject FromFile(string filename) {
            var lexer = Lexer.FromFile(filename);
            var parser = new Parser(lexer);
            return parser.Parse();
        }
    }
}