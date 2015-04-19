using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using EaiConverter.Test.Utils;
using System.IO;

namespace EaiConverter.Test.Utils
{
    public class TestCodeGeneratorUtils
    {
        public static string GenerateCode (CodeStatementCollection codeStatementCollection)
        {
            var classGenerator = CodeDomProvider.CreateProvider ("CSharp");
            var options = new CodeGeneratorOptions ();
            options.BracingStyle = "C";
            string classesInString;
            using (StringWriter writer = new StringWriter ()) {
                foreach (CodeStatement codeStatement in codeStatementCollection) {
                    classGenerator.GenerateCodeFromStatement (codeStatement, writer, options);
                }
                classesInString = writer.GetStringBuilder ().ToString ();
            }
            return classesInString.RemoveWindowsReturnLineChar();
        }

        public static string GenerateCode (CodeMemberMethod executeQueryMethod)
        {
            var classGenerator = CodeDomProvider.CreateProvider ("CSharp");
            var options = new CodeGeneratorOptions ();
            options.BracingStyle = "C";
            string classesInString;
            using (StringWriter writer = new StringWriter ()) {
                classGenerator.GenerateCodeFromStatement (executeQueryMethod.Statements [0], writer, options);
                classesInString = writer.GetStringBuilder ().ToString ();
            }
            return classesInString.RemoveWindowsReturnLineChar();
        }
    }
}

