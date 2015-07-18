using System.CodeDom;
using EaiConverter.Builder.Utils;
using EaiConverter.Utils;

namespace EaiConverter.Builder
{
	public class LoggerInterfaceBuilder
	{


		public CodeNamespace Build () {
			var namespaceResult = new CodeNamespace (TargetAppNameSpaceService.loggerNameSpace);
			namespaceResult.Imports.Add (new CodeNamespaceImport ("System"));
			var dataAccessInterface = new CodeTypeDeclaration ();
			dataAccessInterface.Name = "ILogger";
			dataAccessInterface.IsInterface = true;

			var message = new CodeParameterDeclarationExpression ("System.String", "message");

			var parameters = new CodeParameterDeclarationExpressionCollection ();
			parameters.Add (message);

            var voidReturnType = new CodeTypeReference (CSharpTypeConstant.SystemVoid);

			var debugMethod = new CodeMemberMethod { Name = "Debug", ReturnType = voidReturnType };
			debugMethod.Parameters.AddRange (parameters);
			dataAccessInterface.Members.Add (debugMethod);

			var infoMethod = new CodeMemberMethod { Name = "Info", ReturnType = voidReturnType };
			infoMethod.Parameters.AddRange (parameters);
			dataAccessInterface.Members.Add (infoMethod);

			var errorMethod = new CodeMemberMethod { Name = "Error", ReturnType = voidReturnType };
			errorMethod.Parameters.AddRange (parameters);
			dataAccessInterface.Members.Add (errorMethod);

			var warnMethod = new CodeMemberMethod { Name = "Warn", ReturnType = voidReturnType };
			warnMethod.Parameters.AddRange (parameters);
			dataAccessInterface.Members.Add (warnMethod);

			namespaceResult.Types.Add (dataAccessInterface);

			return namespaceResult;
		}
	}
}

