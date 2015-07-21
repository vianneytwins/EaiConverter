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

        public TibcoProcessClassesBuilder()
        {
            this.coreProcessBuilder = new CoreProcessBuilder();
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
            tibcoBwProcessClassModel.Members.Add(this.GenerateConstructor(tibcoBwProcessToGenerate, tibcoBwProcessClassModel));


            processNamespace.Types.Add(tibcoBwProcessClassModel);

            targetUnit.Namespaces.Add(processNamespace);

            //7 Mappe les classes des activity
            this.GenerateActivityClasses(tibcoBwProcessToGenerate.Activities, activityNameToServiceNameDictionnary, targetUnit);

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
                new CodeNamespaceImport ("System"),
                new CodeNamespaceImport (TargetAppNameSpaceService.domainContractNamespaceName),
                new CodeNamespaceImport (tibcoBwProcessToGenerate.InputAndOutputNameSpace),
                new CodeNamespaceImport (TargetAppNameSpaceService.loggerNameSpace)
            };

            if (tibcoBwProcessToGenerate.XsdImports != null)
            {
                foreach (var xsdImport in tibcoBwProcessToGenerate.XsdImports)
                {
                    imports.Add(new CodeNamespaceImport(ConvertXsdImportToNameSpace(xsdImport.SchemaLocation)));
                }
            }

            var import4Activities = this.GenerateImport4Activities(tibcoBwProcessToGenerate.Activities);

            imports.AddRange(import4Activities);
            return imports.ToArray();
        }

        public List<CodeNamespaceImport> GenerateImport4Activities(List<Activity> activities)
        {
            var import4Activities = new List<CodeNamespaceImport>();
            foreach (var activity in activities)
            {
                if (activity.Type == ActivityType.callProcessActivityType)
                {
                    var callProcessActivity = (CallProcessActivity)activity;
                    import4Activities.Add(new CodeNamespaceImport(ConvertXsdImportToNameSpace(callProcessActivity.TibcoProcessToCall.ShortNameSpace)));
                    import4Activities.Add(new CodeNamespaceImport(ConvertXsdImportToNameSpace(callProcessActivity.TibcoProcessToCall.InputAndOutputNameSpace)));
                }
                else if (activity.Type == ActivityType.loopGroupActivityType || activity.Type == ActivityType.criticalSectionGroupActivityType)
                {
                    import4Activities.AddRange(this.GenerateImport4Activities(((GroupActivity)activity).Activities));
                }
            }
            return import4Activities;
        }

        public static string ConvertXsdImportToNameSpace(string schemaLocation)
        {
            if (schemaLocation.Contains("/"))
            {
                string filePath = schemaLocation.Substring(0, schemaLocation.LastIndexOf("/"));
                filePath = filePath.Remove(0, 1);
                filePath = filePath.Remove(0, filePath.IndexOf("/") + 1);
                return filePath.Replace("/", ".");
            }

            return schemaLocation;
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

            fields.AddRange(this.GenerateFieldsForActivityServices(tibcoBwProcessToGenerate.Activities));

            return fields.ToArray();
        }

        private List<CodeMemberField> GenerateFieldForProcessVariables(TibcoBWProcess tibcoBwProcessToGenerate)
        {
            var fields = new List<CodeMemberField>();
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

            return fields;
        }

        private List<CodeMemberField> GenerateFieldsForActivityServices(List<Activity> activities)
        {
            var fields = new List<CodeMemberField>();
            bool isXmlParserServiceAllReadyAdded = false;
            foreach (Activity activity in activities)
            {
                if (activity.Type == ActivityType.xmlParseActivityType && !isXmlParserServiceAllReadyAdded)
                {
                    fields.Add(new CodeMemberField
                    {
                        Type = new CodeTypeReference(XmlParserHelperBuilder.IXmlParserHelperServiceName),
                        Name = VariableHelper.ToVariableName(VariableHelper.ToClassName(XmlParserHelperBuilder.XmlParserHelperServiceName)),
                        Attributes = MemberAttributes.Private
                    });
                    isXmlParserServiceAllReadyAdded = true;
                }
                else if (activity.Type == ActivityType.assignActivityType || activity.Type == ActivityType.mapperActivityType || activity.Type == ActivityType.generateErrorActivity || activity.Type == ActivityType.writeToLogActivityType)
                {
                    // Do nothing for those type
                }
                else if (activity.Type == ActivityType.callProcessActivityType)
                {
                    var callProcessActivity = (CallProcessActivity)activity;
                    fields.Add(new CodeMemberField
                    {
                        Type = new CodeTypeReference(VariableHelper.ToClassName(callProcessActivity.ProcessName)),
                        Name = VariableHelper.ToVariableName(VariableHelper.ToClassName(callProcessActivity.ProcessName)),
                        Attributes = MemberAttributes.Private
                    });
                }
                else if (activity.Type == ActivityType.loopGroupActivityType)
                {
                    var groupActivity = (GroupActivity)activity;
                    fields.AddRange(this.GenerateFieldsForActivityServices(groupActivity.Activities));
                }
                else if (activity.Type == ActivityType.criticalSectionGroupActivityType)
                {
                    var groupActivity = (GroupActivity)activity;
                    fields.AddRange(this.GenerateFieldsForActivityServices(groupActivity.Activities));

                    // Lock for the synchronise section
                    fields.Add(new CodeMemberField
                        {
                            Type = new CodeTypeReference("System.Object"),
                            Name = VariableHelper.ToVariableName(VariableHelper.ToVariableName(groupActivity.Name + "Lock")),
                            Attributes = MemberAttributes.Private,
                            InitExpression = new CodeSnippetExpression("new System.Object()")
                        });
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

            return fields;
        }

        public CodeConstructor GenerateConstructor(TibcoBWProcess tibcoBwProcessToGenerate, CodeTypeDeclaration classModel)
        {

            var constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;
            foreach (CodeMemberField field in classModel.Members)
            {
                if (this.IsNotAProcessVariable(field.Name, tibcoBwProcessToGenerate.ProcessVariables))
                {
                    constructor.Parameters.Add(new CodeParameterDeclarationExpression(
                        field.Type, field.Name));

                    var parameterReference = new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), field.Name);

                    constructor.Statements.Add(new CodeAssignStatement(parameterReference,
                        new CodeArgumentReferenceExpression(field.Name)));
                }
            }

            return constructor;
        }

        private bool IsNotAProcessVariable(string fieldName, List<ProcessVariable> processVariables)
        {
            if (processVariables != null)
            {
                foreach (var processVariable in processVariables)
                {
                    if (processVariable.Parameter.Name == fieldName)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public CodeMemberMethod[] GenerateMethod(TibcoBWProcess tibcoBwProcessToGenerate, Dictionary<string, CodeStatementCollection> activityNameToServiceNameDictionnary)
        {
            return new List<CodeMemberMethod> { GenerateStartMethod(tibcoBwProcessToGenerate, activityNameToServiceNameDictionnary) }.ToArray();
        }

        public CodeMemberMethod GenerateStartMethod(TibcoBWProcess tibcoBwProcessToGenerate, Dictionary<string, CodeStatementCollection> activityNameToServiceNameDictionnary)
        {
            var startMethod = new CodeMemberMethod();
            if (tibcoBwProcessToGenerate.StartActivity == null)
            {
                return startMethod;
            }

            startMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            startMethod.Name = tibcoBwProcessToGenerate.StartActivity.Name;
            startMethod.ReturnType = this.GenerateStartMethodReturnType(tibcoBwProcessToGenerate);

            this.GenerateStartMethodInputParameters(tibcoBwProcessToGenerate, startMethod);

            this.GenerateStartMethodBody(tibcoBwProcessToGenerate, startMethod, activityNameToServiceNameDictionnary);

            return startMethod;
        }

        public void GenerateStartMethodInputParameters(TibcoBWProcess tibcoBwProcessToGenerate, CodeMemberMethod startMethod)
        {
            if (tibcoBwProcessToGenerate.StartActivity.Parameters != null)
            {
                foreach (var parameter in tibcoBwProcessToGenerate.StartActivity.Parameters)
                {
                    startMethod.Parameters.Add(new CodeParameterDeclarationExpression
                    {
                        Name = parameter.Name,
                        Type = new CodeTypeReference(parameter.Type)
                    });
                }
            }
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

        public void GenerateStartMethodBody(TibcoBWProcess tibcoBwProcessToGenerate, CodeMemberMethod startMethod, Dictionary<string, CodeStatementCollection> activityNameToServiceNameDictionnary)
        {
            if (tibcoBwProcessToGenerate.Transitions != null)
            {
                startMethod.Statements.AddRange(this.coreProcessBuilder.GenerateStartCodeStatement(tibcoBwProcessToGenerate.Transitions, tibcoBwProcessToGenerate.StartActivity.Name, null, activityNameToServiceNameDictionnary));
                // TODO VC : integrate the following section in in CoreProcessBuilder
                if (startMethod.ReturnType.BaseType != CSharpTypeConstant.SystemVoid)
                {
                    var returnName = VariableHelper.ToVariableName(tibcoBwProcessToGenerate.EndActivity.Parameters[0].Name);
                    var objectCreate = new CodeObjectCreateExpression(startMethod.ReturnType);
                    startMethod.Statements.Add(new CodeVariableDeclarationStatement(startMethod.ReturnType, returnName, objectCreate));
                    var returnStatement = new CodeMethodReturnStatement(new CodeVariableReferenceExpression(returnName));
                    startMethod.Statements.Add(returnStatement);
                }
            }
        }

        // Todo : To rename and refactor because not SRP
        public void GenerateActivityClasses(List<Activity> activities, Dictionary<string, CodeStatementCollection> activityNameToServiceNameDictionnary, CodeCompileUnit targetUnit)
        {
            var activityBuilderFactory = new ActivityBuilderFactory();
            foreach (var activity in activities)
            {
                var activityBuilder = activityBuilderFactory.Get(activity.Type);
                targetUnit.Namespaces.AddRange(activityBuilder.GenerateClassesToGenerate(activity));
                activityNameToServiceNameDictionnary.Add(activity.Name, activityBuilder.GenerateInvocationCode(activity));
            }
        }

        private CodeNamespaceCollection GenerateProcessVariablesNamespaces(TibcoBWProcess tibcoBwProcessToGenerate)
        {
            var processVariableNameNamespaces = new CodeNamespaceCollection();
            if (tibcoBwProcessToGenerate.ProcessVariables != null)
            {
                foreach (var item in tibcoBwProcessToGenerate.ProcessVariables)
                {
                    if (!IsBasicType(item.Parameter.Type))
                    {
                        processVariableNameNamespaces.Add(this.xsdClassGenerator.Build(item.ObjectXNodes, tibcoBwProcessToGenerate.NameSpace));
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

