using System;
using System.Xml;
using System.IO;
using System.CodeDom.Compiler;
using System.CodeDom;

namespace EaiConverter.CodeGenerator
{
	public interface ISourceCodeGeneratorService
	{
		void Generate (CodeCompileUnit targetUnit);
	}


}
