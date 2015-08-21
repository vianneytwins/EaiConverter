namespace EaiConverter.Builder
{
    using System.CodeDom;

    public class InterfaceExtractorFromClass
	{
		public static CodeNamespace Extract(CodeTypeDeclaration classToTranformInInterface, string namespaceName)
        {
			var namespaceResult = new CodeNamespace(namespaceName);

			namespaceResult.Imports.Add(new CodeNamespaceImport("System"));
			namespaceResult.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));

			var interfaceToGenerate = new CodeTypeDeclaration
			                              {
			                                  Name = "I" + classToTranformInInterface.Name,
			                                  IsInterface = true
			                              };
		    foreach (CodeTypeMember member in classToTranformInInterface.Members)
            {
				if (member is CodeMemberMethod)
                {
					interfaceToGenerate.Members.Add(member);
				}
			}

			namespaceResult.Types.Add(interfaceToGenerate);
			return namespaceResult;
		}
	}
}

