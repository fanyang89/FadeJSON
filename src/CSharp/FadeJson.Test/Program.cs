using System;
using System.IO;
using Jil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FadeJson.Test
{
    internal static class Program
    {
        private static void Main(string[] args) {
            var testSuitePathList = new[] {
                "TestSuite/TestSuite.json",
                "TestSuite/data.json",
                "TestSuite/TestSuite2.json",
                "TestSuite/auctions.json"
            };

            //foreach (var path in testSuitePathList) {
            //    CodeTimer.Execute($"IO Test {path}", 10, () => {
            //        var content = File.ReadAllText(path);
            //    });
            //}

            foreach (var path in testSuitePathList) {
                CodeTimer.Execute($"Json.NET Test {path}", 10, () => {
                    using (var fileStream = new FileStream(path, FileMode.Open)) {
                        var jObject = JObject.Load(new JsonTextReader(new StreamReader(fileStream)));
                    }
                });
            }

            foreach (var path in testSuitePathList) {
                CodeTimer.Execute($"Jil Test {path}", 10, () => {
                    using (var stream = new FileStream(path, FileMode.Open)) {
                        using (var sr = new StreamReader(stream)) {
                            var j = JSON.DeserializeDynamic(sr);
                        }
                    }
                });
            }

            foreach (var path in testSuitePathList) {
                CodeTimer.Execute($"FadeJson Test {path}", 10, () => {
                    var jsonValue = JsonValue.FromFile(path);
                });
            }

            Console.ReadKey();
        }
    }
}