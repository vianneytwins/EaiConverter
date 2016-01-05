using EaiConverter.Model;
using System.CodeDom;
using EaiConverter.Builder.Utils;
using EaiConverter.CodeGenerator.Utils;
using System.Reflection;
using System.Collections.Generic;
using EaiConverter.Processor;
using EaiConverter.Utils;

namespace EaiConverter.Builder
{
    public class JavaActivityBuilder : IActivityBuilder
    { 
        XslBuilder xslBuilder;

        const string InvokeMethodName = "Invoke";

        const string IJavaActivityService = "IJavaActivityService";

        public JavaActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }


        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity){
            JavaActivity javaActivity = (JavaActivity) activity;

            var javaNamespace = new CodeNamespace(javaActivity.PackageName);

            // Generate the Service
            javaNamespace.Imports.AddRange(this.GenerateImports());
            var javaClass = this.GenerateClass(javaActivity);
            javaClass.Members.Add(this.GenerateInvokeMethod());
            javaNamespace.Types.Add(javaClass);

            var codeNameSpaces =  new CodeNamespaceCollection {javaNamespace};

            // Generate the corresponding interface 
            if (ConfigurationApp.GetProperty ("IsJavaInterfaceAlreadyGenerated") != "true")
            {
                //TODO : Refactor because it's a bit dirty
                var javaServiceInterfaceNameSpace = InterfaceExtractorFromClass.Extract(javaClass, TargetAppNameSpaceService.javaToolsNameSpace());
                javaServiceInterfaceNameSpace.Types[0].Name = IJavaActivityService;
                codeNameSpaces.Add(javaServiceInterfaceNameSpace);
                ConfigurationApp.SaveProperty("IsJavaInterfaceAlreadyGenerated", "true");
            }

            return codeNameSpaces;
        }


        public List<CodeNamespaceImport> GenerateImports(Activity activity)
        {
            return new List<CodeNamespaceImport>
            {
                new CodeNamespaceImport(TargetAppNameSpaceService.javaToolsNameSpace()),
                new CodeNamespaceImport(TargetAppNameSpaceService.javaToolsNameSpace())
            };
        }

        public CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity activity)
        {
            return new CodeParameterDeclarationExpressionCollection();
        }

        public CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
        {
            return new CodeStatementCollection();
        }

        public List<CodeMemberField> GenerateFields(Activity activity)
        {
            return new List<CodeMemberField>();
        }

        public CodeNamespaceImport[] GenerateImports()
        {
            return new CodeNamespaceImport[2] { new CodeNamespaceImport("System"), new CodeNamespaceImport(TargetAppNameSpaceService.javaToolsNameSpace()) };
        }

        public CodeTypeDeclaration GenerateClass(JavaActivity activity)
        {
            var javaClass = new CodeTypeDeclaration(activity.FileName)
                                {
                                    IsClass = true,
                                    TypeAttributes = TypeAttributes.Public
                                };
            javaClass.BaseTypes.Add(new CodeTypeReference(IJavaActivityService));

            javaClass.Comments.Add(new CodeCommentStatement(activity.FullSource));

            return javaClass;
        }

        public CodeMemberMethod GenerateInvokeMethod()
        {
            {
                var method = new CodeMemberMethod
                                 {
                                     Attributes = MemberAttributes.Public | MemberAttributes.Final,
                                     Name = InvokeMethodName,
                                     ReturnType = new CodeTypeReference(CSharpTypeConstant.SystemVoid)
                                 };

                return method;
            }
        }

        public CodeStatementCollection GenerateInvocationCode(Activity activity)
        {
            var javaActivity = (JavaActivity)activity;

            var invocationCodeCollection = new CodeStatementCollection();
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(javaActivity));

            invocationCodeCollection.AddRange(this.xslBuilder.Build(javaActivity.InputBindings));


            var variableReturnType = new CodeTypeReference(javaActivity.PackageName + "." + javaActivity.FileName);
            var creation = new CodeObjectCreateExpression (variableReturnType, new CodeExpression[0]);

            string javaClassVariableName = VariableHelper.ToVariableName(javaActivity.FileName);
            var codeInvocation = new CodeVariableDeclarationStatement(variableReturnType, javaClassVariableName, creation);

            invocationCodeCollection.Add(codeInvocation);

            var javaClassReference = new CodeVariableReferenceExpression { VariableName = javaClassVariableName };

            //add input to java class
            invocationCodeCollection.AddRange(this.GenerateInputCallOnJavaClass(javaActivity, javaClassReference));

            // add call to invoke methode
            invocationCodeCollection.Add(this.GenerateInvokeCallOnJavaClass(javaClassReference));

            // instanciate the result class
            var activityReturnType = new CodeTypeReference(javaActivity.PackageName + "." + VariableHelper.ToClassName(javaActivity.Name));
            var creationActivityReturn = new CodeObjectCreateExpression (activityReturnType, new CodeExpression[0]);

            string activityClassVariableName = VariableHelper.ToVariableName(javaActivity.Name);
            var codeActivityInvocation = new CodeVariableDeclarationStatement(activityReturnType, activityClassVariableName, creationActivityReturn);

            invocationCodeCollection.Add(codeActivityInvocation);

            // retrieve the output
            CodeVariableReferenceExpression activityClassReference = new CodeVariableReferenceExpression();
            activityClassReference.VariableName = activityClassVariableName;

            invocationCodeCollection.AddRange(this.GenerateOutputCallOnJavaClass(javaActivity, javaClassReference, activityClassReference));

            return invocationCodeCollection;
        }

        private CodeStatementCollection GenerateInputCallOnJavaClass(JavaActivity javaActivity, CodeVariableReferenceExpression javaClassReference)
        {
            var invocationCodeCollection = new CodeStatementCollection();
            foreach (var parameter in javaActivity.Parameters)
            {
                CodeMethodInvokeExpression setterCall = new CodeMethodInvokeExpression();
                setterCall.Parameters.AddRange(new List<CodeExpression> {
                    new CodeSnippetExpression(parameter.Name)
                }.ToArray());
                CodeMethodReferenceExpression setterMethod = new CodeMethodReferenceExpression();
                setterMethod.MethodName = "set" + parameter.Name;
                setterMethod.TargetObject = javaClassReference;
                setterCall.Method = setterMethod;
                invocationCodeCollection.Add(setterCall);
            }

            return invocationCodeCollection;
        }

        private CodeStatementCollection GenerateOutputCallOnJavaClass(JavaActivity javaActivity, CodeVariableReferenceExpression javaClassReference, CodeVariableReferenceExpression activityClassReference)
        {
            var invocationCodeCollection = new CodeStatementCollection();
            foreach (var parameter in javaActivity.OutputData)
            {
                CodeAssignStatement _assign1 = new CodeAssignStatement();

                CodePropertyReferenceExpression _prop1 = new CodePropertyReferenceExpression();
                _prop1.PropertyName = parameter.Name;
                _prop1.TargetObject = activityClassReference;
                _assign1.Left = _prop1;
     //           CodePrimitiveExpression _value1 = new CodePrimitiveExpression();
      //          _value1.Value = "testvalue";
       //         _assign1.Right = _value1;
        //        __ctor_ctor1.Statements.Add(_assign1);


                CodeMethodInvokeExpression getterCall = new CodeMethodInvokeExpression();
                getterCall.Parameters.AddRange(new CodeExpression[0]);
                CodeMethodReferenceExpression getterMethod = new CodeMethodReferenceExpression();
                getterMethod.MethodName = "get" + parameter.Name;
                getterMethod.TargetObject = javaClassReference;
                getterCall.Method = getterMethod;
                _assign1.Right = getterCall;
                invocationCodeCollection.Add(_assign1);
            }

            return invocationCodeCollection;
        }

        private CodeMethodInvokeExpression GenerateInvokeCallOnJavaClass(CodeVariableReferenceExpression javaClassReference)
        {
            CodeMethodInvokeExpression invokeCall = new CodeMethodInvokeExpression();
            invokeCall.Parameters.AddRange(new CodeExpression[0]);
            CodeMethodReferenceExpression invokeMethod = new CodeMethodReferenceExpression();
            invokeMethod.MethodName = InvokeMethodName;
            invokeMethod.TargetObject = javaClassReference;
            invokeCall.Method = invokeMethod;
            return invokeCall;
        }

        public string GetReturnType (Activity activity)
        {
            return CSharpTypeConstant.SystemVoid;
        }
    }
}

