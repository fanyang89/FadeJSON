using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace FadeJson.Toolkit.ExportClass
{
    internal static class Program
    {
        //输入：json格式的文件内容
        //输出：该文件对应的类(*.cs)

        private static int classCount = 0;

        private static string GetNameFromEnum(JsonValueType valType) {
            var typeNames = new[] { "JsonValue", "JsonValue", "int", "string", "bool" };
            return typeNames[(int)valType];
        }

        private static string Input(string msg) {
            Console.WriteLine(msg);
            return Console.ReadLine();
        }

        private static string Join(this List<string> list) {
            var sb = new StringBuilder();
            foreach (var str in list) {
                sb.Append(str);
                sb.Append("\n");
            }
            return sb.ToString();
        }

        private static void Main(string[] args) {
            string namespaceName = string.Empty;
            string className = string.Empty;
            string fileName = string.Empty;

            Console.WriteLine("FadeJson.ToolKit.ExportClass: 帮助你从JSON文件生成对应的类");

            if (args.Length != 1 && args.Length != 3) {
                Console.WriteLine("Invalid Arguments.");
                Console.WriteLine("Drag json file to this exe OR use it in cmd like this");
                Console.WriteLine("FadeJson.Toolkit.ExportClass.exe test.Json myNamespace myClass");
                Console.ReadKey();
                return;
            }

            fileName = args[0];

            if (args.Length == 1) {
                namespaceName = Input("NamespaceName is ?");
                className = Input("ClassName is ?");
            }

            if (args.Length == 3) {
                namespaceName = args[1];
                className = args[2];
            }

            GenerateTypeDecl(JsonValue.FromFile(fileName), namespaceName, className);
            Console.WriteLine("Done.");
        }

        private static void GenerateTypeDecl(JsonValue j, string namespaceName, string className) {
            var codeNamespace = new CodeNamespace(namespaceName);
            codeNamespace.Imports.Add(new CodeNamespaceImport("FadeJson"));
            var codeClass = new CodeTypeDeclaration(className);
            var codeCtor = new CodeConstructor();
            codeCtor.Attributes = MemberAttributes.Public;
            codeCtor.Name = className;
            codeCtor.Parameters.Add(new CodeParameterDeclarationExpression("JsonValue", "j"));

            foreach (var key in j.Keys) {
                var o = j[key];
                if (o.JsonValueType == JsonValueType.Object) {
                    var newClassName = $"{className}{classCount++}";
                    GenerateTypeDecl(o, namespaceName, newClassName);
                    var newProperty = new CodeSnippetTypeMember(
                        $"        public {newClassName} {key.ToString()} {{ get; set; }}");
                    codeClass.Members.Add(newProperty);
                    codeCtor.Statements.Add(new CodeSnippetStatement(
                        $"            {key.ToString()} = new {newClassName}(j[\"{key.ToString()}\"]);"
                        ));
                    continue;
                }
                var typeName = GetNameFromEnum(o.JsonValueType);
                var newField = new CodeSnippetTypeMember(
                    $"        public {typeName} {key.ToString()} {{ get; set; }}");
                codeClass.Members.Add(newField);
                codeCtor.Statements.Add(new CodeSnippetStatement(
                    $"            {key.ToString()} = ({typeName})j[\"{key.ToString()}\"].Value;"));
            }

            codeClass.Members.Add(codeCtor);
            codeNamespace.Types.Add(codeClass);
            var codeGenerater = new CSharpCodeProvider();
            var stringWriter = new StringWriter();
            codeGenerater.GenerateCodeFromNamespace(codeNamespace, stringWriter, new CodeGeneratorOptions() {
                VerbatimOrder = true,
                BlankLinesBetweenMembers = true,
                BracingStyle = "C"
            });
            File.WriteAllText(className + ".cs", stringWriter.ToString());
        }
    }
}