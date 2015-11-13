using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FadeJson.Test
{
    internal static class Program
    {
        private static void Main(string[] args) {
            var testSuitePathList = new[] {
                "TestSuite/TestSuite.json",
                "TestSuite/data.json"
            };

            Console.WriteLine("Parsing with IO.");

            foreach (var path in testSuitePathList) {
                CodeTimer.Execute($"Json.NET Test {path}", 10, () => {
                    var fileStream = new FileStream(path, FileMode.Open);
                    var jObject = JObject.Load(new JsonTextReader(new StreamReader(fileStream)));
                    fileStream.Dispose();
                });
            }

            foreach (var path in testSuitePathList) {
                CodeTimer.Execute($"FadeJson Test {path}", 10, () => { var jsonValue = JsonValue.FromFile(path); });
            }

            Console.WriteLine("Parsing without IO.");

            {
                foreach (var path in testSuitePathList) {
                    var content = File.ReadAllText(path);
                    CodeTimer.Execute($"Json.NET Test {path}", 10, () => { var jObject = JObject.Parse(content); });
                }

                foreach (var path in testSuitePathList) {
                    var content = File.ReadAllText(path);
                    CodeTimer.Execute($"FadeJson Test {path}", 10,
                        () => { var jsonValue = JsonValue.FromString(content); });
                }
            }


            Console.ReadKey();
        }
    }
}