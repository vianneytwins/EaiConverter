namespace EaiConverter.Builder
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Utils;

    using log4net;

    public class TibcoProcessClassesBuilder
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TibcoProcessClassesBuilder));

        private readonly CoreProcessBuilder coreProcessBuilder;
		private readonly ActivityBuilderFactory activityBuilderFactory;

        private readonly XsdBuilder xsdClassGenerator = new XsdBuilder();

        public TibcoProcessClassesBuilder()
        {
            this.coreProcessBuilder = new CoreProcessBuilder();
			this.activityBuilderFactory = new ActivityBuilderFactory();
        }

        public CodeCompileUnit Build(TibcoBWProcess tibcoBwProcessToGenerate)
        {
            Log.Info("Starting Generation of process:" + tibcoBwProcessToGenerate.FullProcessName);

            var activityNameToServiceNameDictionnary = new Dictionary<string, CodeStatementCollection>();

            var targetUnit = new CodeCompileUnit();

            // create the namespace
            var processNamespace = new CodeNamespace(TargetAppNameSpaceService.myAppName() + "." + tibcoBwProcessToGenerate.ShortNameSpace);
            processNamespace.Imports.AddRange(this.GenerateImport(tibcoBwProcessToGenerate));


            var tibcoBwProcessClassModel = this.GenerateSqueletonClassModel(tibcoBwProcessToGenerate);

            var processVariablesDictionary = this.GetProcessVariableDictionary(tibcoBwProcessToGenerate);

            // 4 le ctor avec injection des activités + logger
            tibcoBwProcessClassModel.Members.Add(this.GenerateConstructor(tibcoBwProcessToGenerate));

            processNamespace.Types.Add(tibcoBwProcessClassModel);

            targetUnit.Namespaces.Add(processNamespace);

            // 7 Mappe les classes des activity
           

            foreach (var activity in tibcoBwProcessToGenerate.Activities)
			{
				var activityBuilder = this.activityBuilderFactory.Get(activity.Type);
                targetUnit.Namespaces.AddRange(activityBuilder.GenerateClassesToGenerate(activity, processVariablesDictionary));
				activityNameToServiceNameDictionnary.Add(activity.Name, activityBuilder.GenerateInvocationCode(activity, processVariablesDictionary));
                var codeMemberMethod = activityBuilder.GenerateMethods(activity, processVariablesDictionary);
                if (codeMemberMethod != null)
                {
                    tibcoBwProcessClassModel.Members.AddRange(codeMemberMethod.ToArray());
                }
				processNamespace.Imports.AddRange(activityBuilder.GenerateImports(activity).ToArray());
				tibcoBwProcessClassModel.Members.AddRange(activityBuilder.GenerateFields (activity).ToArray());
			}

            // Same for the starter
			if (tibcoBwProcessToGenerate.StarterActivity != null)
			{
				var activityBuilder = this.activityBuilderFactory.Get(tibcoBwProcessToGenerate.StarterActivity.Type);
                targetUnit.Namespaces.AddRange(activityBuilder.GenerateClassesToGenerate(tibcoBwProcessToGenerate.StarterActivity, processVariablesDictionary));
				processNamespace.Imports.AddRange(activityBuilder.GenerateImports(tibcoBwProcessToGenerate.StarterActivity).ToArray());
				tibcoBwProcessClassModel.Members.AddRange(activityBuilder.GenerateFields(tibcoBwProcessToGenerate.StarterActivity).ToArray());
			}
			
            // Add the reduction on fields
            this.RemoveDuplicateFields(tibcoBwProcessClassModel);

            // Generate ouput and input classes from start and End Activity
            targetUnit.Namespaces.Add(this.GenerateInputOutputClasses(tibcoBwProcessToGenerate.EndActivity, TargetAppNameSpaceService.myAppName() + "." + tibcoBwProcessToGenerate.InputAndOutputNameSpace));
            targetUnit.Namespaces.Add(this.GenerateInputOutputClasses(tibcoBwProcessToGenerate.StartActivity, TargetAppNameSpaceService.myAppName() + "." + tibcoBwProcessToGenerate.InputAndOutputNameSpace));

            // Add the invocation code of the End if any
            if (tibcoBwProcessToGenerate.EndActivity != null)
            {
                activityNameToServiceNameDictionnary.Add(tibcoBwProcessToGenerate.EndActivity.Name, this.GenerateEndActivityInvocationCode(tibcoBwProcessToGenerate));
            }

            targetUnit.Namespaces.AddRange(this.GenerateProcessVariablesNamespaces(tibcoBwProcessToGenerate));
            // 8 la methode start avec input starttype et return du endtype
            tibcoBwProcessClassModel.Members.AddRange(this.GenerateMethod(tibcoBwProcessToGenerate, activityNameToServiceNameDictionnary));

            var interfaceNameSpace = this.GenerateProcessInterface(tibcoBwProcessToGenerate, tibcoBwProcessClassModel);
            targetUnit.Namespaces.Add(interfaceNameSpace);

            return targetUnit;
        }

        private Dictionary<string, string> GetProcessVariableDictionary(TibcoBWProcess tibcoBwProcessToGenerate)
        {
            var processVariablesDictionary = new Dictionary<string, string>();
            var fieldForProcessVariables = this.GenerateFieldForProcessVariables(tibcoBwProcessToGenerate);
            foreach (var codeMemberField in fieldForProcessVariables)
            {
                processVariablesDictionary[codeMemberField.Name] = codeMemberField.Type.BaseType;
            }

            if (tibcoBwProcessToGenerate.StartActivity != null && tibcoBwProcessToGenerate.StartActivity.Parameters != null)
            {
                foreach (var startMethodParameter in tibcoBwProcessToGenerate.StartActivity.Parameters)
                {
                    processVariablesDictionary["start_" + startMethodParameter.Name] = startMethodParameter.Type;
                }
            }

            if (tibcoBwProcessToGenerate.StartActivity != null)
            {
                processVariablesDictionary[tibcoBwProcessToGenerate.EndActivity.Name] =
                    this.GenerateStartMethodReturnType(tibcoBwProcessToGenerate).BaseType;
            }

            foreach (var activity in tibcoBwProcessToGenerate.Activities)
            {
                var activityBuilder = this.activityBuilderFactory.Get(activity.Type);
                processVariablesDictionary[activity.Name] = activityBuilder.GetReturnType(activity);
            }

            return processVariablesDictionary;
        }

        private CodeTypeDeclaration GenerateSqueletonClassModel(TibcoBWProcess tibcoBwProcessToGenerate)
        {
            var tibcoBwProcessClassModel =
                new CodeTypeDeclaration(VariableHelper.ToClassName(tibcoBwProcessToGenerate.ProcessName))
                    {
                        IsClass = true,
                        TypeAttributes =
                            TypeAttributes
                            .Public
                    };

            tibcoBwProcessClassModel.BaseTypes.Add(tibcoBwProcessToGenerate.InterfaceName);

            tibcoBwProcessClassModel.Comments.Add(new CodeCommentStatement(tibcoBwProcessToGenerate.Description));


            // 3 les membres privee : les activité injecte
            var privateFields = this.GeneratePrivateFields(tibcoBwProcessToGenerate);

            tibcoBwProcessClassModel.Members.AddRange(privateFields);


            return tibcoBwProcessClassModel;
        }

        public CodeNamespace GenerateProcessInterface(TibcoBWProcess tibcoBwProcessToGenerate, CodeTypeDeclaration tibcoBwProcessClassModel)
        {
            var namespaceName = TargetAppNameSpaceService.myAppName() + "." + tibcoBwProcessToGenerate.ShortNameSpace;
            var interfaceNameSpace = InterfaceExtractorFromClass.Extract(tibcoBwProcessClassModel, TargetAppNameSpaceService.myAppName() + "." + tibcoBwProcessToGenerate.ShortNameSpace);
            if ((tibcoBwProcessToGenerate.StartActivity != null && tibcoBwProcessToGenerate.StartActivity.Parameters != null) || (tibcoBwProcessToGenerate.EndActivity != null && tibcoBwProcessToGenerate.EndActivity.Parameters != null))
            {
                interfaceNameSpace.Imports.Add(new CodeNamespaceImport(TargetAppNameSpaceService.myAppName() + "." +tibcoBwProcessToGenerate.InputAndOutputNameSpace));
            }

            interfaceNameSpace.Imports.AddRange(this.GenerateXsdImports(tibcoBwProcessToGenerate).ToArray());

            ModuleBuilder.AddServiceToRegister(namespaceName + "." + interfaceNameSpace.Types[0].Name, namespaceName + "." + tibcoBwProcessClassModel.Name);
            return interfaceNameSpace;
        }

        /// <summary>
        /// Generates the import.
        /// </summary>
        /// <returns>The import.</returns>
        /// <param name="tibcoBwProcessToGenerate">Tibco bw process to generate.</param>
        public CodeNamespaceImport[] GenerateImport(TibcoBWProcess tibcoBwProcessToGenerate)
        {
            var imports = new List<CodeNamespaceImport>
            {
                new CodeNamespaceImport("System"),
                new CodeNamespaceImport(TargetAppNameSpaceService.xmlToolsNameSpace()),
                new CodeNamespaceImport(TargetAppNameSpaceService.loggerNameSpace())
            };

            if ((tibcoBwProcessToGenerate.StartActivity != null && tibcoBwProcessToGenerate.StartActivity.Parameters != null)
                || (tibcoBwProcessToGenerate.EndActivity != null && tibcoBwProcessToGenerate.EndActivity.Parameters != null))
            {
                imports.Add(new CodeNamespaceImport(TargetAppNameSpaceService.myAppName() + "." + tibcoBwProcessToGenerate.InputAndOutputNameSpace));
            }

            imports.AddRange(this.GenerateXsdImports(tibcoBwProcessToGenerate));

            return imports.ToArray();
        }

        private List<CodeNamespaceImport> GenerateXsdImports(TibcoBWProcess tibcoBwProcessToGenerate)
        {
            var imports = new List<CodeNamespaceImport>();
            if (tibcoBwProcessToGenerate.XsdImports != null)
            {
                foreach (var xsdImport in tibcoBwProcessToGenerate.XsdImports)
                {
                    imports.Add(
                        new CodeNamespaceImport(
                            TargetAppNameSpaceService.myAppName() + "."
                            + TargetAppNameSpaceService.ConvertXsdImportToNameSpace(xsdImport.SchemaLocation)));
                }
            }

            return imports;
        }

        public CodeMemberField[] GeneratePrivateFields(TibcoBWProcess tibcoBwProcessToGenerate)
        {
            var fields = new List<CodeMemberField>
                             {
                                 new CodeMemberField
                                     {
                                         Type = new CodeTypeReference("ILogger"),
                                         Name = "logger",
                                         Attributes = MemberAttributes.Private
                                     }
                             };

            fields.AddRange(this.GenerateFieldForProcessVariables(tibcoBwProcessToGenerate));

            return fields.ToArray();
        }

        public List<CodeMemberField> GenerateFieldForProcessVariables(TibcoBWProcess tibcoBwProcessToGenerate)
        {
            var fields = new List<CodeMemberField>();
            if (tibcoBwProcessToGenerate.ProcessVariables != null)
            {
                foreach (var variable in tibcoBwProcessToGenerate.ProcessVariables)
                {
                    CodeTypeReference typeReference;

                    if (!CodeDomUtils.IsBasicType(variable.Parameter.Type))
                    {
                        typeReference = new CodeTypeReference(tibcoBwProcessToGenerate.VariablesNameSpace + "." + variable.Parameter.Type);
                    }
                    else
                    {
                        typeReference = new CodeTypeReference(CodeDomUtils.GetCorrectBasicType(variable.Parameter.Type));
                    }


                    fields.Add(
                        new CodeMemberField
                        {
                            Type = typeReference,
                            Name = variable.Parameter.Name,
                            Attributes = MemberAttributes.Private
                        });
                }
            }

            return fields;
        }

        public void RemoveDuplicateFields(CodeTypeDeclaration tibcoBwProcessClassModel)
        {
            var keepers = tibcoBwProcessClassModel.Members
                .Cast<CodeTypeMember>()
                .GroupBy(x => new { x.Name })
                .SelectMany(x => x.Take(1))
                .ToArray();

            tibcoBwProcessClassModel.Members.Clear();

            foreach (var objTestReport in keepers)
            {
                tibcoBwProcessClassModel.Members.Add(objTestReport);
            }

        }

        public CodeConstructor GenerateConstructor(TibcoBWProcess tibcoBwProcessToGenerate)
        {
            var constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;

			constructor.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference("ILogger"), "logger"));

			constructor.Statements.Add(
				new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "logger"),
				new CodeArgumentReferenceExpression("logger"))
			);

			foreach (Activity activity in tibcoBwProcessToGenerate.Activities)
            {
				var builder = this.activityBuilderFactory.Get(activity.Type);

				constructor.Parameters.AddRange(builder.GenerateConstructorParameter(activity));
				constructor.Statements.AddRange(builder.GenerateConstructorCodeStatement(activity));
            }

            this.RemoveDuplicateParameters(constructor.Parameters);
           // this.RemoveDuplicateStatements(constructor.Statements);

			if (tibcoBwProcessToGenerate.StarterActivity != null)
			{
				var builder = this.activityBuilderFactory.Get(tibcoBwProcessToGenerate.StarterActivity.Type);

				constructor.Parameters.AddRange(builder.GenerateConstructorParameter(tibcoBwProcessToGenerate.StarterActivity));
				constructor.Statements.AddRange(builder.GenerateConstructorCodeStatement(tibcoBwProcessToGenerate.StarterActivity));
			}

            return constructor;
        }


        public CodeMemberMethod[] GenerateMethod(TibcoBWProcess tibcoBwProcessToGenerate, Dictionary<string, CodeStatementCollection> activityNameToServiceNameDictionnary)
        {
			var methods = new List<CodeMemberMethod>();
			methods.Add(this.GenerateStartMethod(tibcoBwProcessToGenerate, activityNameToServiceNameDictionnary));

			if (tibcoBwProcessToGenerate.StarterActivity != null)
            {
                methods.Add(this.GenerateOnEventMethod(tibcoBwProcessToGenerate, activityNameToServiceNameDictionnary));
			}

            return methods.ToArray();
        }

        public CodeMemberMethod GenerateStartMethod(TibcoBWProcess tibcoBwProcessToGenerate, Dictionary<string, CodeStatementCollection> activityNameToServiceNameDictionnary)
        {
            var startMethod = new CodeMemberMethod();
			if (tibcoBwProcessToGenerate.StartActivity == null && tibcoBwProcessToGenerate.StarterActivity == null)
            {
                return startMethod;
            }

            startMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            if (tibcoBwProcessToGenerate.StartActivity != null)
            {
                startMethod.Name = tibcoBwProcessToGenerate.StartActivity.Name;
            }
            else
            {
                startMethod.Name = "Start";
            }

            startMethod.ReturnType = this.GenerateStartMethodReturnType(tibcoBwProcessToGenerate);

			if (tibcoBwProcessToGenerate.StartActivity != null)
			{
                startMethod.Parameters.AddRange(this.GenerateStartMethodInputParameters(tibcoBwProcessToGenerate));
				startMethod.Statements.AddRange(this.GenerateMainMethodBody(tibcoBwProcessToGenerate, activityNameToServiceNameDictionnary));
			}
			else if (tibcoBwProcessToGenerate.StarterActivity != null)
			{
				startMethod.Statements.AddRange(this.GenerateStarterMethodBody());
			}

            return startMethod;
        }

		public CodeParameterDeclarationExpressionCollection GenerateStartMethodInputParameters(TibcoBWProcess tibcoBwProcessToGenerate)
        {
			var parameters = new CodeParameterDeclarationExpressionCollection();
            if (tibcoBwProcessToGenerate.StartActivity != null && tibcoBwProcessToGenerate.StartActivity.Parameters != null)
            {
                foreach (var parameter in tibcoBwProcessToGenerate.StartActivity.Parameters)
                {
                    parameters.Add(new CodeParameterDeclarationExpression
                    {
                        Name = "start_" + parameter.Name,
                        Type = new CodeTypeReference(parameter.Type)
                    });
                }
            }

			return parameters;
        }

        public CodeTypeReference GenerateStartMethodReturnType(TibcoBWProcess tibcoBwProcessToGenerate)
        {
            string returnType;
            if (tibcoBwProcessToGenerate.EndActivity == null || tibcoBwProcessToGenerate.EndActivity.Parameters == null)
            {
                returnType = CSharpTypeConstant.SystemVoid;
            }
            else
            {
                returnType = tibcoBwProcessToGenerate.EndActivity.Parameters[0].Type;
            }
            return new CodeTypeReference(returnType);
        }

		public CodeMemberMethod GenerateOnEventMethod (TibcoBWProcess tibcoBwProcessToGenerate, Dictionary<string, CodeStatementCollection> activityNameToServiceNameDictionnary)
		{
			var onEventMethod = new CodeMemberMethod();
			if (tibcoBwProcessToGenerate.StarterActivity == null)
			{
				return onEventMethod;
			}

			onEventMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			onEventMethod.Name = "OnEvent";
			onEventMethod.ReturnType = new CodeTypeReference (CSharpTypeConstant.SystemVoid);

			onEventMethod.Parameters.AddRange(this.GenerateOnEventMethodInputParameters(tibcoBwProcessToGenerate));

			onEventMethod.Statements.AddRange(this.GenerateMainMethodBody (tibcoBwProcessToGenerate, activityNameToServiceNameDictionnary));

			return onEventMethod;
		}

		public CodeStatementCollection GenerateMainMethodBody(TibcoBWProcess tibcoBwProcessToGenerate, Dictionary<string, CodeStatementCollection> activityNameToServiceNameDictionnary)
        {
			var statements = new CodeStatementCollection();
			if (tibcoBwProcessToGenerate.Transitions != null)
            {
                if (tibcoBwProcessToGenerate.StarterActivity != null)
                {
                    //foreach (var parameter in tibcoBwProcessToGenerate.StarterActivity.Parameters)
                    //{
                        //statements.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(parameter.Type), parameter.Name));
                    //}
                }

                statements.AddRange(this.coreProcessBuilder.GenerateMainCodeStatement(tibcoBwProcessToGenerate.Transitions, tibcoBwProcessToGenerate.StartingPoint, null, activityNameToServiceNameDictionnary));

            }
			return statements;
        }


		public CodeParameterDeclarationExpressionCollection GenerateOnEventMethodInputParameters(TibcoBWProcess tibcoBwProcessToGenerate)
		{
			var parameters = new CodeParameterDeclarationExpressionCollection ();
            parameters.Add (new CodeParameterDeclarationExpression (new CodeTypeReference (CSharpTypeConstant.SystemObject), "sender"));
            parameters.Add (new CodeParameterDeclarationExpression (new CodeTypeReference ("System.EventArgs"), "args"));

			return parameters;
		}

        public CodeNamespace GenerateInputOutputClasses(Activity myActivity, string inputOutputNamespace)
        {
            var xsdCodeNamespace = new CodeNamespace();
            if (myActivity != null && myActivity.ObjectXNodes != null)
            {
                try
                {
                    xsdCodeNamespace = this.xsdClassGenerator.Build(myActivity.ObjectXNodes, inputOutputNamespace);
                }
                catch (Exception e)
                {
                    try
                    {
                        Log.Warn("Error generating " + myActivity.Name + " object class for this namespace :" + inputOutputNamespace + ", we will try with a homemade xsdbuilder", e);
                        xsdCodeNamespace = this.xsdClassGenerator.Build(myActivity.Parameters, inputOutputNamespace);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Unable to generate " + myActivity.Name + " object class for this namespace :" + inputOutputNamespace, ex);
                    }
                }
            }

            return xsdCodeNamespace;
        }

        public CodeStatementCollection GenerateEndActivityInvocationCode(TibcoBWProcess tibcoBwProcessToGenerate)
        {
            var statements = new CodeStatementCollection();

            var returnType = this.GenerateStartMethodReturnType(tibcoBwProcessToGenerate);

            if (returnType.BaseType != CSharpTypeConstant.SystemVoid)
            {
                //statements.AddRange(DefaultActivityBuilder.LogActivity(tibcoBwProcessToGenerate.EndActivity));
                statements.AddRange(new XslBuilder(new XpathBuilder()).Build(TargetAppNameSpaceService.myAppName() + "." + tibcoBwProcessToGenerate.InputAndOutputNameSpace, tibcoBwProcessToGenerate.EndActivity.InputBindings));
                var returnName = tibcoBwProcessToGenerate.EndActivity.Parameters[0].Name;
           
                var returnStatement = new CodeMethodReturnStatement(new CodeVariableReferenceExpression(returnName));
                statements.Add(returnStatement);
            }
            else
            {
                statements.Add(new CodeMethodReturnStatement());
            }

            return statements;
        }

        private CodeStatementCollection GenerateStarterMethodBody()
        {
            var statements = new CodeStatementCollection();
            statements.Add(new CodeSnippetStatement("        this.subscriber.Start();"));
            return statements;
        }
	
        private CodeNamespaceCollection GenerateProcessVariablesNamespaces(TibcoBWProcess tibcoBwProcessToGenerate)
        {
            var processVariableNameNamespaces = new CodeNamespaceCollection();
            if (tibcoBwProcessToGenerate.ProcessVariables != null)
            {
                foreach (var item in tibcoBwProcessToGenerate.ProcessVariables)
                {
                    if (!CodeDomUtils.IsBasicType(item.Parameter.Type))
                    {
                        try
                        {
                                processVariableNameNamespaces.Add(
                                    this.xsdClassGenerator.Build(item.ObjectXNodes, TargetAppNameSpaceService.myAppName() + "." + tibcoBwProcessToGenerate.VariablesNameSpace));
                        }
                        catch (Exception e)
                        {
                            Log.Error(
                                "unable to generate Process Variable object class for this process: "
                                + tibcoBwProcessToGenerate.ProcessName,
                                e);
                        }
                    }
                }
            }

            return processVariableNameNamespaces;
        }

        private void RemoveDuplicateParameters(CodeParameterDeclarationExpressionCollection parameters)
        {
            var toKeep = parameters
    .Cast<CodeParameterDeclarationExpression>()
    .GroupBy(x => new { x.Name })
    .SelectMany(x => x.Take(1))
    .ToArray();

            parameters.Clear();

            foreach (var objToKeep in toKeep)
            {
                parameters.Add(objToKeep);
            }
        }
    }
}

