using System;
using System.CodeDom;

namespace EaiConverter.Builder
{
	public class InterfaceExtractorFromClass
	{
		public static CodeNamespace Extract (CodeTypeDeclaration classToTranformInInterface, string namespaceName){
			var namespaceResult = new CodeNamespace (namespaceName);
			namespaceResult.Imports.Add (new CodeNamespaceImport ("System"));

			var interfaceToGenerate = new CodeTypeDeclaration ();
			interfaceToGenerate.Name = "I" + classToTranformInInterface.Name;
			interfaceToGenerate.IsInterface = true;
			foreach (CodeTypeMember member in classToTranformInInterface.Members){
				if (member is CodeMemberMethod) {
					interfaceToGenerate.Members.Add (member);
				}
			}
			namespaceResult.Types.Add (interfaceToGenerate);
			return namespaceResult;
		}
	}
}

