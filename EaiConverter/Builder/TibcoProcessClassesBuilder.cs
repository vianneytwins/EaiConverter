namespace EaiConverter.Builder
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Reflection;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Utils;

    public class TibcoProcessClassesBuilder
    {
        private readonly CoreProcessBuilder coreProcessBuilder;
		private readonly ActivityBuilderFactory activityBuilderFactory;

        public TibcoProcessClassesBuilder()
        {
            this.coreProcessBuilder = new CoreProcessBuilder();
			this.activityBuilderFactory = new ActivityBuilderFactory();
        }

        XsdBuilder xsdClassGenerator = new XsdBuilder();

        public CodeCompileUnit Build(TibcoBWProcess tibcoBwProcessToGenerate)
        {
            var activityNameToServiceNameDictionnary = new Dictionary<string, CodeStatementCollection>();

            var targetUnit = new CodeCompileUnit();

            // create the namespace
            var processNamespace = new CodeNamespace(tibcoBwProcessToGenerate.ShortNameSpace);

            processNamespace.Imports.AddRange(this.GenerateImport(tibcoBwProcessToGenerate));

            var tibcoBwProcessClassModel = new CodeTypeDeclaration(tibcoBwProcessToGenerate.ProcessName)
                                               {
                                                   IsClass = true,
                                                   TypeAttributes = TypeAttributes.Public
                                               };

            tibcoBwProcessClassModel.Comments.Add(new CodeCommentStatement(tibcoBwProcessToGenerate.Description));

            // 3 les membres privee : les activité injecte
            tibcoBwProcessClassModel.Members.AddRange(this.GeneratePrivateFields(tibcoBwProcessToGenerate));

            // 4 le ctor avec injection des activités + logger
            tibcoBwProcessClassModel.Members.Add(this.GenerateConstructor(tibcoBwProcessToGenerate));


            processNamespace.Types.Add(tibcoBwProcessClassModel);

            targetUnit.Namespaces.Add(processNamespace);

            //7 Mappe les classes des activity

			foreach (var activity in tibcoBwProcessToGenerate.Activities)
			{
				var activityBuilder = activityBuilderFactory.Get(activity.Type);
				targetUnit.Namespaces.AddRange(activityBuilder.GenerateClassesToGenerate(activity));
				activityNameToServiceNameDictionnary.Add(activity.Name, activityBuilder.GenerateInvocationCode(activity));
				processNamespace.Imports.AddRange(activityBuilder.GenerateImports(activity).ToArray());
				tibcoBwProcessClassModel.Members.AddRange (activityBuilder.GenerateFields (activity).ToArray());
			}

			if (tibcoBwProcessToGenerate.StarterActivity != null)
			{
				var activityBuilder = activityBuilderFactory.Get (tibcoBwProcessToGenerate.StarterActivity.Type);
				targetUnit.Namespaces.AddRange (activityBuilder.GenerateClassesToGenerate (tibcoBwProcessToGenerate.StarterActivity));
				processNamespace.Imports.AddRange (activityBuilder.GenerateImports (tibcoBwProcessToGenerate.StarterActivity).ToArray ());
				tibcoBwProcessClassModel.Members.AddRange (activityBuilder.GenerateFields (tibcoBwProcessToGenerate.StarterActivity).ToArray ());
			}
			//Same for the starter

            if (tibcoBwProcessToGenerate.EndActivity != null && tibcoBwProcessToGenerate.EndActivity.ObjectXNodes != null)
            {
                try
                {
                    targetUnit.Namespaces.Add(this.xsdClassGenerator.Build(tibcoBwProcessToGenerate.EndActivity.ObjectXNodes, tibcoBwProcessToGenerate.InputAndOutputNameSpace));
                }
                catch (Exception e)
                {
                    Console.WriteLine("############### ERROR####### unable to generate  END output object class for this process:" + tibcoBwProcessToGenerate.ProcessName);
                    Console.WriteLine(e);
                }
            }

            if (tibcoBwProcessToGenerate.StartActivity != null && tibcoBwProcessToGenerate.StartActivity.ObjectXNodes != null)
            {
                try
                {
                    targetUnit.Namespaces.Add(this.xsdClassGenerator.Build(tibcoBwProcessToGenerate.StartActivity.ObjectXNodes, tibcoBwProcessToGenerate.InputAndOutputNameSpace));
                }
                catch (Exception e)
                {
                    Console.WriteLine("############### ERROR####### unable to generate  Start input object class for this process:" + tibcoBwProcessToGenerate.ProcessName);
                    Console.WriteLine(e);
                }
            }

            targetUnit.Namespaces.AddRange(this.GenerateProcessVariablesNamespaces(tibcoBwProcessToGenerate));

            //7 la methode start avec input starttype et return du endtype
            tibcoBwProcessClassModel.Members.AddRange(this.GenerateMethod(tibcoBwProcessToGenerate, activityNameToServiceNameDictionnary));

            return targetUnit;
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
				new CodeNamespaceImport(tibcoBwProcessToGenerate.InputAndOutputNameSpace),
                new CodeNamespaceImport(TargetAppNameSpaceService.loggerNameSpace)
            };

            if (tibcoBwProcessToGenerate.XsdImports != null)
            {
                foreach (var xsdImport in tibcoBwProcessToGenerate.XsdImports)
                {
					imports.Add(new CodeNamespaceImport(TargetAppNameSpaceService.ConvertXsdImportToNameSpace(xsdImport.SchemaLocation)));
                }
            }

            return imports.ToArray();
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

        private List<CodeMemberField> GenerateFieldForProcessVariables(TibcoBWProcess tibcoBwProcessToGenerate)
        {
            var fields = new List<CodeMemberField>();
            if (tibcoBwProcessToGenerate.ProcessVariables != null)
            {
                foreach (var variable in tibcoBwProcessToGenerate.ProcessVariables)
                {
                    fields.Add(
                        new CodeMemberField
                        {
                            Type = new CodeTypeReference(tibcoBwProcessToGenerate.VariablesNameSpace + "." + variable.Parameter.Type),
                            Name = VariableHelper.ToVariableName(variable.Parameter.Name),
                            Attributes = MemberAttributes.Private
                        });
                }
            }

            return fields;
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
				var builder = this.activityBuilderFactory.Get (activity.Type);

				constructor.Parameters.AddRange(builder.GenerateConstructorParameter(activity));
				constructor.Statements.AddRange(builder.GenerateConstructorCodeStatement(activity));
            
            }

			if (tibcoBwProcessToGenerate.StarterActivity != null)
			{
				var builder = this.activityBuilderFactory.Get (tibcoBwProcessToGenerate.StarterActivity.Type);

				constructor.Parameters.AddRange(builder.GenerateConstructorParameter(tibcoBwProcessToGenerate.StarterActivity));
				constructor.Statements.AddRange(builder.GenerateConstructorCodeStatement(tibcoBwProcessToGenerate.StarterActivity));
			}

            return constructor;
        }


        public CodeMemberMethod[] GenerateMethod(TibcoBWProcess tibcoBwProcessToGenerate, Dictionary<string, CodeStatementCollection> activityNameToServiceNameDictionnary)
        {
			var methods = new List<CodeMemberMethod> ();
			methods.Add(this.GenerateStartMethod (tibcoBwProcessToGenerate, activityNameToServiceNameDictionnary));

			if (tibcoBwProcessToGenerate.StarterActivity != null) {
				this.GenerateOnEventMethod (tibcoBwProcessToGenerate, activityNameToServiceNameDictionnary);
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

			startMethod.Parameters.AddRange(this.GenerateStartMethodInputParameters(tibcoBwProcessToGenerate));

			if (tibcoBwProcessToGenerate.StartActivity != null)
			{
				startMethod.Statements.AddRange(this.GenerateMainMethodBody (tibcoBwProcessToGenerate,startMethod.ReturnType, activityNameToServiceNameDictionnary));
			}
			else if (tibcoBwProcessToGenerate.StarterActivity != null)
			{
				startMethod.Statements.AddRange(this.GenerateStarterMethodBody());
			}

            return startMethod;
        }

		public CodeParameterDeclarationExpressionCollection GenerateStartMethodInputParameters(TibcoBWProcess tibcoBwProcessToGenerate)
        {
			var parameters = new CodeParameterDeclarationExpressionCollection ();
            if (tibcoBwProcessToGenerate.StartActivity != null && tibcoBwProcessToGenerate.StartActivity.Parameters != null)
            {
                foreach (var parameter in tibcoBwProcessToGenerate.StartActivity.Parameters)
                {
                    parameters.Add(new CodeParameterDeclarationExpression
                    {
                        Name = parameter.Name,
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

			onEventMethod.Statements.AddRange(this.GenerateMainMethodBody (tibcoBwProcessToGenerate, onEventMethod.ReturnType, activityNameToServiceNameDictionnary));

			return onEventMethod;
		}

		public CodeStatementCollection GenerateMainMethodBody(TibcoBWProcess tibcoBwProcessToGenerate, CodeTypeReference returnType, Dictionary<string, CodeStatementCollection> activityNameToServiceNameDictionnary)
        {
			var statements = new CodeStatementCollection ();
			if (tibcoBwProcessToGenerate.Transitions != null)
            {
                statements.AddRange(this.coreProcessBuilder.GenerateMainCodeStatement(tibcoBwProcessToGenerate.Transitions, "Start", null, activityNameToServiceNameDictionnary));

				if (returnType.BaseType != CSharpTypeConstant.SystemVoid)
                {
                    var returnName = VariableHelper.ToVariableName(tibcoBwProcessToGenerate.EndActivity.Parameters[0].Name);
					var objectCreate = new CodeObjectCreateExpression(returnType);
					statements.Add(new CodeVariableDeclarationStatement(returnType, returnName, objectCreate));
                    var returnStatement = new CodeMethodReturnStatement(new CodeVariableReferenceExpression(returnName));
                    statements.Add(returnStatement);
                }
            }
			return statements;
        }

		private CodeStatementCollection GenerateStarterMethodBody ()
		{
			var statements = new CodeStatementCollection ();
			statements.Add (new CodeSnippetStatement("this.subscriber.Start()"));
			return statements;
		}

		public CodeParameterDeclarationExpressionCollection GenerateOnEventMethodInputParameters(TibcoBWProcess tibcoBwProcessToGenerate)
		{
			var parameters = new CodeParameterDeclarationExpressionCollection ();
            if (tibcoBwProcessToGenerate.StarterActivity.Parameters == null)
            {
                return parameters;
            }

			foreach (var parameter in tibcoBwProcessToGenerate.StarterActivity.Parameters) {
				parameters.Add (new CodeParameterDeclarationExpression (new CodeTypeReference (parameter.Type), parameter.Name));
			}
			return parameters;
		}
			
        private CodeNamespaceCollection GenerateProcessVariablesNamespaces(TibcoBWProcess tibcoBwProcessToGenerate)
        {
            var processVariableNameNamespaces = new CodeNamespaceCollection();
            if (tibcoBwProcessToGenerate.ProcessVariables != null)
            {
                foreach (var item in tibcoBwProcessToGenerate.ProcessVariables)
                {
                    try
                    {
                        if (!IsBasicType(item.Parameter.Type))
                        {
                            processVariableNameNamespaces.Add(
                                this.xsdClassGenerator.Build(item.ObjectXNodes, tibcoBwProcessToGenerate.VariablesNameSpace));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(
                            "############### ERROR####### unable to generate Process Variable object class for this process:"
                            + tibcoBwProcessToGenerate.ProcessName);
                        Console.WriteLine(e);
                    }
                }
            }

            return processVariableNameNamespaces;
        }

        private static bool IsBasicType(string type)
        {
            switch (type.ToLower())
            {
                case "string":
                    return true;
                case "int":
                    return true;
                case "datetime":
                    return true;
                case "bool":
                    return true;
                case "double":
                    return true;
                default:
                    return false;
            }
        }
    }
}

