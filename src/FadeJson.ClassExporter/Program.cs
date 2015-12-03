using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CSharp;

namespace FadeJson.ClassExporter
{
    internal static class Program
    {
        //输入：json格式的文件内容
        //输出：该文件对应的类(*.cs)

        private static int classCount;

        private static string GetNameFromEnum(JsonType valType) {
            var typeNames = new[] {"JsonValue", "JsonValue", "int", "string", "bool"};
            return typeNames[(int) valType];
        }

        private static string Input(string msg) {
            Console.WriteLine(msg);
            return Console.ReadLine();
        }

        private static void Main(string[] args) {
            var namespaceName = string.Empty;
            var className = string.Empty;

            Console.WriteLine("FadeJson.ToolKit.ExportClass: 帮助你从JSON文件生成对应的类");

            if (args.Length != 1 && args.Length != 3) {
                Console.WriteLine("Invalid Arguments.");
                Console.WriteLine("Drag json file to this exe OR use it in cmd like this");
                Console.WriteLine("FadeJson.Toolkit.ExportClass.exe test.Json myNamespace myClass");
                Console.ReadKey();
                return;
            }

            var fileName = args[0];

            if (args.Length == 1) {
                namespaceName = Input("NamespaceName is ?");
                className = Input("ClassName is ?");
            }

            if (args.Length == 3) {
                namespaceName = args[1];
                className = args[2];
            }

            GenerateTypeDecl(JsonValueUtils.FromFile(fileName), namespaceName, className);
            Console.WriteLine("Done.");
        }

        private static void GenerateTypeDecl(JsonValue j, string namespaceName, string className) {
            var codeNamespace = new CodeNamespace(namespaceName);
            codeNamespace.Imports.Add(new CodeNamespaceImport("FadeJson"));
            var codeClass = new CodeTypeDeclaration(className);
            var codeCtor = new CodeConstructor {
                Attributes = MemberAttributes.Public,
                Name = className
            };
            codeCtor.Parameters.Add(new CodeParameterDeclarationExpression("JsonValue", "j"));

            foreach (var key in j.Keys) {
                var o = j[key];
                if (o.Type == JsonType.Object) {
                    var newClassName = $"{className}{classCount++}";
                    GenerateTypeDecl(o, namespaceName, newClassName);
                    var newProperty = new CodeSnippetTypeMember(
                        $"        public {newClassName} {key} {{ get; set; }}");
                    codeClass.Members.Add(newProperty);
                    codeCtor.Statements.Add(new CodeSnippetStatement(
                        $"            {key} = new {newClassName}(j[\"{key}\"]);"
                        ));
                    continue;
                }
                var typeName = GetNameFromEnum(o.Type);
                var newField = new CodeSnippetTypeMember(
                    $"        public {typeName} {key} {{ get; set; }}");
                codeClass.Members.Add(newField);
                codeCtor.Statements.Add(new CodeSnippetStatement(
                    $"            {key} = ({typeName})j[\"{key}\"].Value;"));
            }

            codeClass.Members.Add(codeCtor);
            codeNamespace.Types.Add(codeClass);
            var codeGenerater = new CSharpCodeProvider();
            var stringWriter = new StringWriter();
            codeGenerater.GenerateCodeFromNamespace(codeNamespace, stringWriter, new CodeGeneratorOptions {
                VerbatimOrder = true,
                BlankLinesBetweenMembers = true,
                BracingStyle = "C"
            });
            File.WriteAllText(className + ".cs", stringWriter.ToString());
        }
    }
}