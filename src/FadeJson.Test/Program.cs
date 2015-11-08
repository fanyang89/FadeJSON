using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FadeJson.Test
{
    static class Program
    {
        private static void Main(string[] args) {
            var testSuitePathList = new string[] {
                "TestSuite/data.json",
                "TestSuite/TestSuite.json"
            };

            foreach (var path in testSuitePathList) {
                CodeTimer.Execute($"Json.NET Test {path}", 10, () => {
                    var fileStream = new FileStream(path, FileMode.Open);
                    var jObject = JObject.Load(new JsonTextReader(new StreamReader(fileStream)));
                    fileStream.Dispose();
                });

                CodeTimer.Execute($"FadeJson Test {path}", 10, () => {
                    var jsonValue = JsonValue.FromFile(path);
                });
            }

            Console.ReadKey();
        }
    }
}