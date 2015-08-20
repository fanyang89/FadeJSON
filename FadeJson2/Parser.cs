using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FadeJson2
{
    public class Parser
    {
        private Lexer lexer;

        public Parser(Lexer lexer) {
            this.lexer = lexer;
        }

        public List<dynamic> GetAllTokens() {
            var tokens = new List<dynamic>();
            var token = lexer.GetToken();
            while (token != null) {
                tokens.Add(token);
                token = lexer.GetToken();
            }
            return tokens;
        }

        public JsonObject Parse() {
            throw new NotImplementedException();
        }
    }
}
