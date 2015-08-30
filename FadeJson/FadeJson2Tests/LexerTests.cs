using Microsoft.VisualStudio.TestTools.UnitTesting;
using FadeJson2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FadeJson2.Tests
{
    [TestClass]
    public class LexerTests
    {
        private const string TEST_JSON = @"{""name"" : ""yangfan""}";

        [TestMethod]
        public void GetIntTest() {
            const string content = "123456";
            var lexer = Lexer.FromString(content);
            var token = lexer.GetIntToken();
            Assert.IsTrue(token.Value == "123456");
        }

        [TestMethod]
        public void GetTokenTest() {
            const string content = @"""foobar"" - 123456";
            var lexer = Lexer.FromString(content);
            dynamic token = lexer.NextToken();
            Assert.IsTrue(token.Value == "foobar");
            token = lexer.NextToken();
            Assert.IsTrue(token.Value == "-");
            token = lexer.NextToken();
            Assert.IsTrue(token.Value == 123456);
        }

        [TestMethod]
        public void GetStringTokenTest() {
            const string content = @"""foobar""";
            var lexer = Lexer.FromString(content);
            var token = lexer.GetStringToken();
            Assert.IsTrue(token.Value == "foobar");
        }
    }
}