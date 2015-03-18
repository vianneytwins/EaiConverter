using System;
using System.Xml;
using System.IO;
using System.CodeDom.Compiler;
using System.CodeDom;

namespace EaiConverter.CodeGenerator
{
	public class CsharpSourceCodeGeneratorService : ISourceCodeGeneratorService
	{
		public void Generate (CodeCompileUnit targetUnit)
		{
			CodeDomProvider provider = CodeDomProvider.CreateProvider ("CSharp");
			CodeGeneratorOptions options = new CodeGeneratorOptions ();
			options.BracingStyle = "C";

			//var destinationPath = "../../../../TestBW/TestBW/src/";
            var destinationPath = "./";
			// Build the output file name.
			foreach (CodeNamespace namespaceUnit in targetUnit.Namespaces) {
				string sourceFile;
				if (provider.FileExtension [0] == '.') {
					sourceFile = destinationPath + namespaceUnit.Types[0].Name + provider.FileExtension;
				} else {
					sourceFile = destinationPath + namespaceUnit.Types[0].Name + "." + provider.FileExtension;
				}

				using (StreamWriter sw = new StreamWriter (sourceFile, false)) {
					IndentedTextWriter tw = new IndentedTextWriter (sw, "    ");
					provider.GenerateCodeFromNamespace (namespaceUnit, tw, options);
					tw.Close ();
				}
				Console.WriteLine (sourceFile + " has been generated");
			}

		}
	}

}
