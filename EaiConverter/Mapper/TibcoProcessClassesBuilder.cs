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

        XsdBuilder xsdClassGenerator = new XsdBuilder();

		public CodeCompileUnit Build (TibcoBWProcess tibcoBwProcessToGenerate){

			var targetUnit = new CodeCompileUnit();

			//create the namespace
			CodeNamespace processNamespace = new CodeNamespace(tibcoBwProcessToGenerate.ShortNameSpace);

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
				targetUnit.Namespaces.Add (this.xsdClassGenerator.Build (tibcoBwProcessToGenerate.EndActivity.ObjectXNodes, tibcoBwProcessToGenerate.inputAndOutputNameSpace));
			}
			if (tibcoBwProcessToGenerate.StartActivity!= null && tibcoBwProcessToGenerate.StartActivity.ObjectXNodes != null) {
				targetUnit.Namespaces.Add (this.xsdClassGenerator.Build (tibcoBwProcessToGenerate.StartActivity.ObjectXNodes, tibcoBwProcessToGenerate.inputAndOutputNameSpace));
			}
            if (tibcoBwProcessToGenerate.ProcessVariables!= null) {
                foreach (var item in tibcoBwProcessToGenerate.ProcessVariables)
                {
                    if (!IsBasicType(item.Parameter.Type))
                    {
                        targetUnit.Namespaces.Add(this.xsdClassGenerator.Build(item.ObjectXNodes, tibcoBwProcessToGenerate.NameSpace));
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
            var imports = new List<CodeNamespaceImport> {
				new CodeNamespaceImport ("System"),
				new CodeNamespaceImport (TargetAppNameSpaceService.domainContractNamespaceName),
				new CodeNamespaceImport (tibcoBwProcessToGenerate.inputAndOutputNameSpace),
				new CodeNamespaceImport (TargetAppNameSpaceService.loggerNameSpace)
			};

            if (tibcoBwProcessToGenerate.XsdImports != null)
            {
                foreach (var xsdImport in tibcoBwProcessToGenerate.XsdImports)
                {
                    imports.Add(new CodeNamespaceImport (ConvertXsdImportToNameSpace(xsdImport.SchemaLocation)));
                }
            }

            return imports.ToArray();
		}

        public static string ConvertXsdImportToNameSpace(string schemaLocation)
        {
            string filePath = schemaLocation.Substring(0, schemaLocation.LastIndexOf("/"));
            filePath = filePath.Remove(0, 1);
            filePath = filePath.Remove(0, filePath.IndexOf("/")+1);
            return filePath.Replace("/",".");
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

            bool IsXmlParserServiceAllReadyAdded = false;
			foreach (Activity activity in tibcoBwProcessToGenerate.Activities) {
                if (activity.Type == ActivityType.xmlParseActivityType && !IsXmlParserServiceAllReadyAdded)
                {
                    fields.Add(new CodeMemberField
                        {
                            Type = new CodeTypeReference(XmlParserHelperBuilder.IXmlParserHelperServiceName),
                            Name = VariableHelper.ToVariableName(VariableHelper.ToClassName(XmlParserHelperBuilder.XmlParserHelperServiceName)),
                            Attributes = MemberAttributes.Private
                        });
                    IsXmlParserServiceAllReadyAdded = true;
                }
                if (activity.Type == ActivityType.assignActivityType || activity.Type == ActivityType.mapperActivityType)
                {
                    // DO nothing for those type
                }
                else
                {
                    fields.Add(new CodeMemberField
                        {
                            Type = new CodeTypeReference("I" + VariableHelper.ToClassName(activity.Name + "Service")),
                            Name = VariableHelper.ToVariableName(VariableHelper.ToClassName(activity.Name + "Service")),
                            Attributes = MemberAttributes.Private
                        });
                }
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
                    CodeTypeReference codeTypeRefernce;
                    if (this.IsBasicType(parameter.Type))
                    {
                        codeTypeRefernce = new CodeTypeReference(parameter.Type);
                    }
                    else {
                        codeTypeRefernce = new CodeTypeReference(tibcoBwProcessToGenerate.inputAndOutputNameSpace+"."+ parameter.Type);
                    }

                    startMethod.Parameters.Add(new CodeParameterDeclarationExpression {
                        Name = parameter.Name,
                        Type = codeTypeRefernce
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
                
                if (this.IsBasicType(tibcoBwProcessToGenerate.EndActivity.Parameters[0].Type))
                {
                    returnType = tibcoBwProcessToGenerate.EndActivity.Parameters[0].Type;
                }
                else {
                    returnType = tibcoBwProcessToGenerate.inputAndOutputNameSpace+"."+ tibcoBwProcessToGenerate.EndActivity.Parameters[0].Type;
                }

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

