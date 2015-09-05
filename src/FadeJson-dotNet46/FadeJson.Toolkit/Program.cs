using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.CodeDom;

namespace FadeJson.Toolkit
{
    static class Program
    {
        //输入：json格式的文件内容
        //输出：该文件对应的类(*.cs)

        static void Main(string[] args) {
            if (args.Length != 1) {
                Console.WriteLine("Invalid args counts. Just one argument is accepted.");
                return;
            }
            fileName = args[0];
            var j = JsonValue.FromFile(fileName);
            ParserObj(j, "ROOT");
            Console.WriteLine("Done.");
        }

        static string Join(this List<string> list) {
            var sb = new StringBuilder();
            foreach (var str in list) {
                sb.Append(str);
                sb.Append("\n");
            }
            return sb.ToString();
        }

        static string fileName = string.Empty;
        static int classCount = 0;

        static void ParserObj(JsonValue j, string className) {
            var list = new List<string>();
            list.Add($"public class {className} {{");
            foreach (var key in j.Keys) {
                if (j[key].JsonValueType == JsonValueType.Object) {
                    ParserObj(j[key], $"{className}{classCount}");
                    list.Add($"public {className}{classCount} {key.ToString()} {{ get; set; }}");
                    continue;
                }
                var typeName = GetNameFromEnum(j[key].JsonValueType);
                list.Add($"public {typeName} {key.ToString()} {{ get; set; }}");
            }
            list.Add("}");

            File.WriteAllText($"{Path.GetFileNameWithoutExtension(fileName)}-{classCount++}.cs", list.Join());
        }

        static string GetNameFromEnum(JsonValueType valType) {
            var typeNames = new[] { "JsonValue", "JsonValue", "int", "string", "bool" };
            return typeNames[(int)valType];
        }
    }
}
