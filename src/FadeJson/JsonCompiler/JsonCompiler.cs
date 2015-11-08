using System;
using System.CodeDom;

namespace FadeJson.JsonCompiler
{
    public class JsonCompiler
    {
        public void CompileToSourceFile(JsonValue jsonValue, string filename, JsonCompilerOptions options) {
            var codeNamespace = new CodeNamespace(options.Namespace);
            var codeType = new CodeTypeDeclaration(options.ClassName);

            throw new NotImplementedException();
        }

        private void CodeGen() {
        }
    }
}