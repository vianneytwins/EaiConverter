namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Reflection;

    using EaiConverter.Builder.Utils;

    public class DatabaseAttributeBuilder
	{
		public CodeNamespace Build (string dataBaseAttributeName)
		{
			var dataBaseAttributeNameSpace = new CodeNamespace (TargetAppNameSpaceService.dataAccessCommonNamespace());
			dataBaseAttributeNameSpace.Imports.Add (new CodeNamespaceImport ("System"));

			var dataAccessToGenerate = new CodeTypeDeclaration ();
			dataAccessToGenerate.IsClass = true;
			dataAccessToGenerate.TypeAttributes = TypeAttributes.Public;

			dataAccessToGenerate.Name = dataBaseAttributeName + "Attribute";

			dataAccessToGenerate.BaseTypes.Add (new CodeTypeReference ("System.Attribute"));

			dataBaseAttributeNameSpace.Types.Add (dataAccessToGenerate);

			return dataBaseAttributeNameSpace;
		}
	}
}

