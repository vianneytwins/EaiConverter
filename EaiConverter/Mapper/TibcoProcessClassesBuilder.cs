using System;
using System.Collections.Generic;
using System.CodeDom;
using System.Reflection;
using System.CodeDom.Compiler;
using System.IO;
using EaiConverter.CodeGenerator.Utils;
using EaiConverter.Model;
using EaiConverter.Mapper.Utils;

namespace EaiConverter.Mapper
{
	public class TibcoProcessClassesBuilder
	{
		XsdClassGenerator xsdClassGenerator = new XsdClassGenerator();

		public CodeCompileUnit Build (TibcoBWProcess tibcoBwProcessToGenerate){

			var targetUnit = new CodeCompileUnit();

			//create the namespace
			CodeNamespace processNamespace = new CodeNamespace(tibcoBwProcessToGenerate.NameSpace);

			processNamespace.Imports.AddRange (this.GenerateImport (tibcoBwProcessToGenerate));

			var tibcoBwProcessClassModel = new CodeTypeDeclaration (tibcoBwProcessToGenerate.ProcessName);
			tibcoBwProcessClassModel.IsClass = true;
			tibcoBwProcessClassModel.TypeAttributes = TypeAttributes.Public;

			// 3 les membres privee : les activité injecte
			tibcoBwProcessClassModel.Members.AddRange( this.GeneratePrivateFields (tibcoBwProcessToGenerate));

			// 4 le ctor avec injection des activités + logger
			tibcoBwProcessClassModel.Members.AddRange( this.GenerateConstructors (tibcoBwProcessToGenerate, tibcoBwProcessClassModel));

			// 5 les properties : ici y en a pas
			//this.GenerateProperties (tibcoBwProcessToGenerate);

			// 6 la methode start avec input starttype et return du endtype
			tibcoBwProcessClassModel.Members.AddRange( this.GenerateMethod (tibcoBwProcessToGenerate));

			processNamespace.Types.Add (tibcoBwProcessClassModel);

			targetUnit.Namespaces.Add (processNamespace);

			//7 Mappe les classes des activity
			targetUnit.Namespaces.AddRange (this.GenerateActivityClasses (tibcoBwProcessToGenerate));

			if (tibcoBwProcessToGenerate.EndActivity!= null && tibcoBwProcessToGenerate.EndActivity.ObjectXNodes != null) {
				targetUnit.Namespaces.Add (this.xsdClassGenerator.GenerateCodeFromXsdNodes (tibcoBwProcessToGenerate.EndActivity.ObjectXNodes, tibcoBwProcessToGenerate.inputAndOutputNameSpace));
			}
			if (tibcoBwProcessToGenerate.StartActivity!= null && tibcoBwProcessToGenerate.StartActivity.ObjectXNodes != null) {
				targetUnit.Namespaces.Add (this.xsdClassGenerator.GenerateCodeFromXsdNodes (tibcoBwProcessToGenerate.StartActivity.ObjectXNodes, tibcoBwProcessToGenerate.inputAndOutputNameSpace));
			}
				
			return targetUnit;
		}

		/// <summary>
		/// Generates the import.
		/// </summary>
		/// <returns>The import.</returns>
		/// <param name="tibcoBwProcessToGenerate">Tibco bw process to generate.</param>
		public CodeNamespaceImport[] GenerateImport (TibcoBWProcess tibcoBwProcessToGenerate)
		{
			return new CodeNamespaceImport[4] {
				new CodeNamespaceImport ("System"),
				new CodeNamespaceImport (TargetAppNameSpaceService.domainContractNamespaceName),
				new CodeNamespaceImport (tibcoBwProcessToGenerate.inputAndOutputNameSpace),
				new CodeNamespaceImport (TargetAppNameSpaceService.loggerNameSpace)
			};
		}

		public CodeMemberField[] GeneratePrivateFields (TibcoBWProcess tibcoBwProcessToGenerate)
		{
			var fields = new List<CodeMemberField> ();
			fields.Add (new CodeMemberField {
				Type = new CodeTypeReference("ILogger"),
				Name= "logger",
				Attributes = MemberAttributes.Private 
			});
			foreach (Activity activity in tibcoBwProcessToGenerate.Activities) {
				fields.Add (new CodeMemberField {
					Type = new CodeTypeReference("I" + VariableHelper.ToClassName (activity.Name + "Service")),
					Name = VariableHelper.ToVariableName (VariableHelper.ToClassName (activity.Name + "Service")),
					Attributes = MemberAttributes.Private
					});
			}

			return fields.ToArray();
		}

		public CodeConstructor[] GenerateConstructors (TibcoBWProcess tibcoBwProcessToGenerate, CodeTypeDeclaration classModel)
		{

			var constructor = new CodeConstructor();
			constructor.Attributes = MemberAttributes.Public;
	
			foreach (CodeMemberField field in classModel.Members) {
				constructor.Parameters.Add(new CodeParameterDeclarationExpression(
					field.Type, field.Name));
					
				var parameterReference = new CodeFieldReferenceExpression(
					new CodeThisReferenceExpression(), field.Name);

				constructor.Statements.Add(new CodeAssignStatement(parameterReference,
					new CodeArgumentReferenceExpression(field.Name)));

			}

			return new List<CodeConstructor> { constructor }.ToArray();
		}

		public void GenerateProperties(TibcoBWProcess tibcoBwProcessToGenerate)
		{
		}

		public CodeMemberMethod[] GenerateMethod (TibcoBWProcess tibcoBwProcessToGenerate)
		{
			return new List<CodeMemberMethod> {GenerateStartMethod (tibcoBwProcessToGenerate)}.ToArray();
		}

		// TODO : refactoriser car ca commence a etre tres moche
		public CodeMemberMethod GenerateStartMethod (TibcoBWProcess tibcoBwProcessToGenerate)
		{
			var startMethod = new CodeMemberMethod ();
			startMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;

			string returnType;
			if (tibcoBwProcessToGenerate.EndActivity ==null || tibcoBwProcessToGenerate.EndActivity.Parameters == null) {
				returnType = "void";
			}
			else {
				returnType = tibcoBwProcessToGenerate.EndActivity.Parameters [0].Type;

			}

			startMethod.ReturnType = new CodeTypeReference(returnType);
			

			string inputType;
			string inputName;

			if (tibcoBwProcessToGenerate.StartActivity != null){
				startMethod.Name = tibcoBwProcessToGenerate.StartActivity.Name;
			}

			if (tibcoBwProcessToGenerate.StartActivity == null || tibcoBwProcessToGenerate.StartActivity.Parameters == null) {
				inputType = string.Empty;
				inputName = string.Empty;
			}
			else {

				inputType = tibcoBwProcessToGenerate.StartActivity.Parameters [0].Type;
				inputName = tibcoBwProcessToGenerate.StartActivity.Parameters [0].Name;
				startMethod.Parameters.Add( new CodeParameterDeclarationExpression {
					Name = inputName,
					Type = new CodeTypeReference(inputType)
				});

			}

			//Generate startMethod Body 
			if (returnType != "void") {

				var returnName = VariableHelper.ToVariableName (tibcoBwProcessToGenerate.EndActivity.Parameters [0].Name);
				var objectCreate = new CodeObjectCreateExpression (new CodeTypeReference (returnType));
				startMethod.Statements.Add(new CodeVariableDeclarationStatement(
					new CodeTypeReference(returnType), returnName,
					objectCreate));

				CodeMethodReturnStatement returnStatement = new CodeMethodReturnStatement(
					new CodeVariableReferenceExpression(returnName)
				);
			
				startMethod.Statements.Add (returnStatement);
			}

			return startMethod;

		}

		public CodeNamespaceCollection GenerateActivityClasses (TibcoBWProcess tibcoBwProcessToGenerate)
		{
			//TODO factory
			var jdbcQueryBuilderUtils = new JdbcQueryBuilderUtils ();
			var jdbcQueryActivityBuilder = new JdbcQueryActivityBuilder (new DataAccessBuilder(jdbcQueryBuilderUtils), new DataAccessServiceBuilder(jdbcQueryBuilderUtils), new DataAccessInterfacesCommonBuilder());
			var activityClasses = new CodeNamespaceCollection ();
			foreach (var activity in tibcoBwProcessToGenerate.Activities) {
				if (activity.Type == JdbcQueryActivity.jdbcQueryActivityType) {
					activityClasses.AddRange (jdbcQueryActivityBuilder.Build ((JdbcQueryActivity) activity));
				}
			}
			return activityClasses;
		}

	}
}

