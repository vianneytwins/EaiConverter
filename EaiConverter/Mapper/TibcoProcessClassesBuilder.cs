using System;
using System.Collections.Generic;
using System.CodeDom;
using System.Reflection;
using System.CodeDom.Compiler;
using System.IO;
using EaiConverter.CodeGenerator.Utils;
using EaiConverter.Model;
using EaiConverter.Mapper.Utils;
using EaiConverter.Processor;

namespace EaiConverter.Mapper
{
	public class TibcoProcessClassesBuilder
	{
        CoreProcessBuilder coreProcessBuilder;

        Dictionary<string,string> activityNameToServiceNameDictionnary = new Dictionary<string, string> ();

        public TibcoProcessClassesBuilder (){
            this.coreProcessBuilder = new CoreProcessBuilder();
        }

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

			

			processNamespace.Types.Add (tibcoBwProcessClassModel);

			targetUnit.Namespaces.Add (processNamespace);

			//6 Mappe les classes des activity
			targetUnit.Namespaces.AddRange (this.GenerateActivityClasses (tibcoBwProcessToGenerate));

			if (tibcoBwProcessToGenerate.EndActivity!= null && tibcoBwProcessToGenerate.EndActivity.ObjectXNodes != null) {
				targetUnit.Namespaces.Add (this.xsdClassGenerator.GenerateCodeFromXsdNodes (tibcoBwProcessToGenerate.EndActivity.ObjectXNodes, tibcoBwProcessToGenerate.inputAndOutputNameSpace));
			}
			if (tibcoBwProcessToGenerate.StartActivity!= null && tibcoBwProcessToGenerate.StartActivity.ObjectXNodes != null) {
				targetUnit.Namespaces.Add (this.xsdClassGenerator.GenerateCodeFromXsdNodes (tibcoBwProcessToGenerate.StartActivity.ObjectXNodes, tibcoBwProcessToGenerate.inputAndOutputNameSpace));
			}

            //7 la methode start avec input starttype et return du endtype
            tibcoBwProcessClassModel.Members.AddRange( this.GenerateMethod (tibcoBwProcessToGenerate));

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
            
		public CodeMemberMethod GenerateStartMethod (TibcoBWProcess tibcoBwProcessToGenerate)
		{
			var startMethod = new CodeMemberMethod ();
            if (tibcoBwProcessToGenerate.StartActivity == null){
                return startMethod;
            }

            startMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            startMethod.Name = tibcoBwProcessToGenerate.StartActivity.Name;
            startMethod.ReturnType = this.GenerateStartMethodReturnType (tibcoBwProcessToGenerate); 
           
            this.GenerateStartMethodInputParameters(tibcoBwProcessToGenerate, startMethod);
                
            this.GenerateStartMethodBody(tibcoBwProcessToGenerate, startMethod);

			return startMethod;
		}

        private void GenerateStartMethodInputParameters(TibcoBWProcess tibcoBwProcessToGenerate, CodeMemberMethod startMethod)
        {
            if (tibcoBwProcessToGenerate.StartActivity.Parameters != null)
            {
                foreach (var parameter in tibcoBwProcessToGenerate.StartActivity.Parameters)
                {
                    startMethod.Parameters.Add(new CodeParameterDeclarationExpression {
                        Name = parameter.Name,
                        Type = new CodeTypeReference(parameter.Type)
                    });
                }
            }
        }

        private CodeTypeReference GenerateStartMethodReturnType(TibcoBWProcess tibcoBwProcessToGenerate)
        {
            string returnType;
            if (tibcoBwProcessToGenerate.EndActivity == null || tibcoBwProcessToGenerate.EndActivity.Parameters == null)
            {
                returnType = "void";
            }
            else
            {
                returnType = tibcoBwProcessToGenerate.EndActivity.Parameters[0].Type;
            }
            return new CodeTypeReference(returnType);
        }

        public void GenerateStartMethodBody(TibcoBWProcess tibcoBwProcessToGenerate, CodeMemberMethod startMethod)
        {
            if (tibcoBwProcessToGenerate.Transitions != null)
            {
                startMethod.Statements.AddRange(this.coreProcessBuilder.GenerateStartCodeStatement(tibcoBwProcessToGenerate, startMethod, tibcoBwProcessToGenerate.StartActivity.Name, null, this.activityNameToServiceNameDictionnary));
                // TODO VC : integrate the following section in in CoreProcessBuilder
                if (startMethod.ReturnType.BaseType != "void")
                {
                    var returnName = VariableHelper.ToVariableName(tibcoBwProcessToGenerate.EndActivity.Parameters[0].Name);
                    var objectCreate = new CodeObjectCreateExpression(startMethod.ReturnType);
                    startMethod.Statements.Add(new CodeVariableDeclarationStatement(startMethod.ReturnType, returnName, objectCreate));
                    CodeMethodReturnStatement returnStatement = new CodeMethodReturnStatement(new CodeVariableReferenceExpression(returnName));
                    startMethod.Statements.Add(returnStatement);
                }
            }
        }

		public CodeNamespaceCollection GenerateActivityClasses (TibcoBWProcess tibcoBwProcessToGenerate)
		{
			//TODO build a factory instead
			var jdbcQueryBuilderUtils = new JdbcQueryBuilderUtils ();
			var jdbcQueryActivityBuilder = new JdbcQueryActivityBuilder (new DataAccessBuilder(jdbcQueryBuilderUtils), new DataAccessServiceBuilder(jdbcQueryBuilderUtils), new DataAccessInterfacesCommonBuilder());

			var activityClasses = new CodeNamespaceCollection ();
			foreach (var activity in tibcoBwProcessToGenerate.Activities) {
                if (activity.Type == ActivityType.jdbcQueryActivityType || activity.Type == ActivityType.jdbcCallActivityType || activity.Type == ActivityType.jdbcUpdateActivityType)
                {
                    var jdbcActivity = (JdbcQueryActivity)activity;
                    if (this.HasThisSqlRequestAlreadyGenerateAService(jdbcActivity.QueryStatement))
                    {
                        this.RegisterThatThisJdbcActivityMapsAnExistingService(tibcoBwProcessToGenerate, jdbcActivity);
                    }
                    else
                    {
                        var jdbcActivityCodeNameSpaces = jdbcQueryActivityBuilder.Build(jdbcActivity);

                        activityClasses.AddRange(jdbcActivityCodeNameSpaces);
                        this.RegisterThatThisSqlRequestCorrespondToAService(tibcoBwProcessToGenerate, jdbcActivity, jdbcActivityCodeNameSpaces);
                    }
                }
                else
                {
                    this.activityNameToServiceNameDictionnary.Add( activity.Name, VariableHelper.ToVariableName(activity.Name));
                }
			}
			return activityClasses;
		}

        private bool HasThisSqlRequestAlreadyGenerateAService(string queryStatement)
        {
            return SqlRequestToActivityMapper.ContainsKey(queryStatement);
        }

        private void RegisterThatThisSqlRequestCorrespondToAService(TibcoBWProcess tibcoBwProcessToGenerate, JdbcQueryActivity jdbcActivity, CodeNamespaceCollection jdbcActivityCodeNameSpaces)
        {
            // TODO : not safe to rely on indexes to find the service name
            var jdbcServiceName = jdbcActivityCodeNameSpaces[2].Types[0].Name;

            SqlRequestToActivityMapper.SaveSqlRequest(jdbcActivity.QueryStatement, jdbcServiceName);
            activityNameToServiceNameDictionnary.Add( jdbcActivity.Name, jdbcServiceName);
        }

        private void RegisterThatThisJdbcActivityMapsAnExistingService(TibcoBWProcess tibcoBwProcessToGenerate, JdbcQueryActivity jdbcActivity)
        {
            var jdbcServiceName = SqlRequestToActivityMapper.GetJdbcServiceName(jdbcActivity.QueryStatement);
            this.activityNameToServiceNameDictionnary.Add( jdbcActivity.Name, jdbcServiceName);
        }
	}
}

