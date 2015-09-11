using Microsoft.VisualStudio.TestTools.UnitTesting;
using FadeJson;

namespace FadeJsonTests
{
    [TestClass()]
    public class DeserializerTests
    {
        private class TestClass
        {
            public int intTest = 123;
        }

        [TestMethod()]
        public void ParseTest() {
            var j = Deserializer.Parse(new TestClass());
            var intTest = j["intTest"];
            Assert.IsTrue(intTest.JsonValueType == JsonValueType.Int && (int)intTest.Value == 123);
        }
    }
}