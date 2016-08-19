using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FadeJSON.ConsoleTest
{
    internal static class Program
    {
        static void Main(string[] args) {
            var j = new FadeJSON.JsonObject(new Dictionary<string, FadeJSON.JsonObject> {
                ["123"] = new FadeJSON.JsonObject("123")
            });
            var s = Serializer.Serialize(j);

            if (!Directory.Exists("TestResults")) {
                Directory.CreateDirectory("TestResults");
            }
            var testFilePathList = Directory.GetFiles("TestDataFiles", "*.json", SearchOption.AllDirectories);
            JsonChecker("FadeJSON", p => {
                Json.FromFile(p);
            });
            JsonChecker("JSON.NET", p => {
                JObject.Load(new JsonTextReader(new StreamReader(p)));
            });
            JsonChecker("Jil", p => {
                Jil.JSON.DeserializeDynamic(new StreamReader(p));
            });
            JsonChecker("ServiceStack.Text", p => {
                ServiceStack.DynamicJson.Deserialize(File.ReadAllText(p));
            });
            JsonChecker("jsonfx", p => {
                new JsonFx.Json.JsonReader().Read(new StreamReader(new FileStream(p, FileMode.Open)));
            });
            Console.ReadKey();
            TestJsonLibraries(testFilePathList);
        }

        private static void JsonChecker(string title, Action<string> testAction) {
            Console.WriteLine($"============ {title} ============");
            var failedTestCount = 3;
            for (var i = 1; i <= 3; i++) {
                try {
                    testAction($"JSONChecker/pass{i}.json");
                } catch (Exception) {
                    failedTestCount--;
                    Console.WriteLine($"pass{i}.json parsing failed.");
                }
            }
            for (var i = 1; i <= 33; i++) {
                var isFailed = false;
                try {
                    testAction($"JSONChecker/fail{i}.json");
                } catch (Exception) {
                    isFailed = true;
                    failedTestCount++;
                }
                if (!isFailed) {
                    Console.WriteLine($"fail{i}.json parsing failed.");
                }
            }
            Console.WriteLine($"({failedTestCount}/36) {(failedTestCount / 36.0 * 100).ToString("F")}%");
            Console.WriteLine();
        }

        private static void TestJsonLibraries(string[] testFilePathList) {
            using (var fileStream = new FileStream(
                $"TestResults/DeserializationTestResult-{DateTime.Now.ToFileTime()}.csv",
                FileMode.OpenOrCreate))
            using (var writer = new StreamWriter(fileStream, Encoding.UTF8)) {
                const int iteration = 10;
                writer.WriteLine($"Deserialization Test (iteration: {iteration})");
                writer.WriteLine("Unit: ms,FadeJson,Jil,JSON.NET,ServiceStack.Text,SimpleJson,jsonfx");
                foreach (var filePath in testFilePathList) {
                    writer.Write(Path.GetFileNameWithoutExtension(filePath));
                    var results = new[] {
                        CodeTimer.Execute($"FadeJson {filePath}", iteration,
                            () => { Json.FromFile(filePath); }),
                        CodeTimer.Execute($"Jil {filePath}", iteration,
                            () => { Jil.JSON.DeserializeDynamic(new StreamReader(filePath)); }),
                        CodeTimer.Execute($"JSON.NET {filePath}", iteration,
                            () => { JObject.Load(new JsonTextReader(new StreamReader(filePath))); }),
                        CodeTimer.Execute($"ServiceStack.Text {filePath}", iteration,
                            () => { ServiceStack.DynamicJson.Deserialize(File.ReadAllText(filePath)); }),
                        CodeTimer.Execute($"SimpleJson {filePath}", iteration,
                            () => { SimpleJson.DeserializeObject(File.ReadAllText(filePath)); }),
                        CodeTimer.Execute($"jsonfx {filePath}", iteration,
                            () => {
                                new JsonFx.Json.JsonReader().Read(new StreamReader(filePath));
                            })
                    };
                    foreach (var result in results) {
                        writer.Write($",{result.TimeElapsed}");
                    }
                    writer.WriteLine();
                }
            }
        }
    }
}
