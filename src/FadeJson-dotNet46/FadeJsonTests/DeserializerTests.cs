using FadeJson;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FadeJsonTests
{
    [TestClass]
    public class DeserializerTests
    {
        [TestMethod]
        public void ParseTest() {
            var j = Deserializer.Parse(new TestClass());
            var intTest = j["intTest"];
            Assert.IsTrue(intTest.JsonValueType == JsonValueType.Int && (int) intTest.Value == 123);
        }

        private class TestClass
        {
            public int intTest = 123;
        }
    }
}