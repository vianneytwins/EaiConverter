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

        Dictionary<string,CodeStatementCollection> activityNameToServiceNameDictionnary = new Dictionary<string, CodeStatementCollection> ();

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
            if (tibcoBwProcessToGenerate.ProcessVariables!= null) {
                foreach (var item in tibcoBwProcessToGenerate.ProcessVariables)
                {
                    if (!IsBasicType(item.Parameter.Type))
                    {
                        targetUnit.Namespaces.Add(this.xsdClassGenerator.GenerateCodeFromXsdNodes(item.ObjectXNodes, tibcoBwProcessToGenerate.NameSpace));
                    }
                }
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

            if (tibcoBwProcessToGenerate.ProcessVariables != null)
            {
                foreach (var variable in tibcoBwProcessToGenerate.ProcessVariables)
                {
                    fields.Add(new CodeMemberField
                        {
                            Type = new CodeTypeReference(tibcoBwProcessToGenerate.NameSpace + "." + variable.Parameter.Type),
                            Name = VariableHelper.ToVariableName(variable.Parameter.Name),
                            Attributes = MemberAttributes.Private
                        });
                }
            }

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

        // Todo : To rename and refavtor because not SRP
		public CodeNamespaceCollection GenerateActivityClasses (TibcoBWProcess tibcoBwProcessToGenerate)
		{
            var activityBuilderFactory = new ActivityBuilderFactory();
			var activityClasses = new CodeNamespaceCollection ();
			foreach (var activity in tibcoBwProcessToGenerate.Activities) {
                //TODO : faut il mieux 2 method ou 1 objet avec les 2
                var activityBuilder = activityBuilderFactory.Get(activity.Type);

                var activityCodeDom = activityBuilder.Build(activity);

                activityClasses.AddRange(activityCodeDom.ClassesToGenerate);
                this.activityNameToServiceNameDictionnary.Add( activity.Name, activityCodeDom.InvocationCode);
			}
			return activityClasses;
		}
  
       
        bool IsBasicType(string type)
        {
            switch (type) {
                case "string" :
                        return true;
                case "int" :
                    return true;
                case "DateTime" :
                    return true;
                case "bool" :
                    return true;
                case "double" :
                    return true;
                default:
                    return false; 
            }

        }
	}
}

