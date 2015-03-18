using System;
using System.Xml;
using System.IO;
using System.CodeDom.Compiler;
using System.CodeDom;

namespace EaiConverter.Processor
{
	public interface ITibcoBWDirectoryProcessorService
	{
		void Process (string directory);
	}


}
