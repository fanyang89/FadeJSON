using System;
using System.IO;
using System.Text;
using Jil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceStack;
using JsonReader = JsonFx.Json.JsonReader;

namespace FadeJSON.ConsoleTest
{
    internal static class Program
    {
        private static void Main(string[] args) {
            JsonChecker.Check("FadeJSON", p => { Json.FromFile(p); });
            JsonChecker.Check("JSON.NET", p => { JObject.Load(new JsonTextReader(new StreamReader(p))); });
            JsonChecker.Check("Jil", p => { JSON.DeserializeDynamic(new StreamReader(p)); });
            JsonChecker.Check("ServiceStack.Text", p => { DynamicJson.Deserialize(File.ReadAllText(p)); });
            JsonChecker.Check("jsonfx", p => { new JsonReader().Read(new StreamReader(new FileStream(p, FileMode.Open))); });
            if (!Directory.Exists("TestResults")) {
                Directory.CreateDirectory("TestResults");
            }
            var testFilePathList = Directory.GetFiles("TestDataFiles", "*.json", SearchOption.AllDirectories);
            TestJsonLibraries(testFilePathList);
            Console.ReadKey();
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
                            () => { JSON.DeserializeDynamic(new StreamReader(filePath)); }),
                        CodeTimer.Execute($"JSON.NET {filePath}", iteration,
                            () => { JObject.Load(new JsonTextReader(new StreamReader(filePath))); }),
                        CodeTimer.Execute($"ServiceStack.Text {filePath}", iteration,
                            () => { DynamicJson.Deserialize(File.ReadAllText(filePath)); }),
                        CodeTimer.Execute($"jsonfx {filePath}", iteration,
                            () => { new JsonReader().Read(new StreamReader(filePath)); })
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