using System;
using System.Xml;
using System.IO;
using System.CodeDom.Compiler;
using System.CodeDom;

namespace EaiConverter.CodeGenerator
{
	public class CsharpSourceCodeGeneratorService : ISourceCodeGeneratorService
	{
        //TODO VC : Put the output directory as a parameter";
        public const string destinationPath = "./GeneratedSolution";

        public void Generate (CodeCompileUnit targetUnit)
		{
			CodeDomProvider provider = CodeDomProvider.CreateProvider ("CSharp");
			CodeGeneratorOptions options = new CodeGeneratorOptions ();
			options.BracingStyle = "C";
            options.IndentString = "    ";
            options.BlankLinesBetweenMembers = true;

            if (Directory.Exists(destinationPath)){
                Directory.Delete(destinationPath,true);
            }
            Directory.CreateDirectory(destinationPath);
			// Build the output file name.
			foreach (CodeNamespace namespaceUnit in targetUnit.Namespaces) {
                var namespaceName = namespaceUnit.Name;

                string sourceFile;
				if (provider.FileExtension [0] == '.') {
                    sourceFile =  this.PathFromNamespace(destinationPath, namespaceName) + "/" + namespaceUnit.Types[0].Name + provider.FileExtension;
				} else {
                    sourceFile = this.PathFromNamespace(destinationPath, namespaceName) + "/" + namespaceUnit.Types[0].Name + "." + provider.FileExtension;
				}

				using (StreamWriter sw = new StreamWriter (sourceFile, false)) {
					IndentedTextWriter tw = new IndentedTextWriter (sw, "    ");
					provider.GenerateCodeFromNamespace (namespaceUnit, tw, options);
					tw.Close ();
				}
				Console.WriteLine (sourceFile + " has been generated");
			}

		}

        // TODO refactor becuase not  really SRP
        private string PathFromNamespace(string outputPath, string ns)
        {
            string path =String.Format("{0}/{1}",
                outputPath,
                ns.Replace('.','/')
            );
            Directory.CreateDirectory(path);
            return path;
        }
	}

}
