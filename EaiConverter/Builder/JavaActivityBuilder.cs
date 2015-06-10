using System;
using EaiConverter.Model;
using System.CodeDom;
using EaiConverter.Builder.Utils;
using EaiConverter.CodeGenerator.Utils;
using System.Reflection;
using System.Collections.Generic;

namespace EaiConverter.Builder
{
    public class JavaActivityBuilder : IActivityBuilder
    { 
        XslBuilder xslBuilder;

        public JavaActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }

        public ActivityCodeDom Build(Activity activity)
        {
            JavaActivity javaActivity = (JavaActivity) activity;

            var result = new ActivityCodeDom();

            result.ClassesToGenerate = new CodeNamespaceCollection();
            result.InvocationCode = this.GenerateCodeInvocation (javaActivity);

            return result;
        }

        public CodeNamespaceCollection Build(JavaActivity activity){
            var javaNamespace = new CodeNamespace(activity.PackageName);

            // Generate the Service
            javaNamespace.Imports.AddRange(this.GenerateImports());
            var javaClass = this.GenerateClass(activity);
            javaNamespace.Types.Add(javaClass);

            // Generate the corresponding interface
            var xmlParserHelperServiceInterfaceNameSpace =  InterfaceExtractorFromClass.Extract(javaClass, TargetAppNameSpaceService.xmlToolsNameSpace);

            return new CodeNamespaceCollection{javaNamespace, xmlParserHelperServiceInterfaceNameSpace};
        }

        public CodeNamespaceImport[] GenerateImports()
        {
            return new CodeNamespaceImport[1] {
                new CodeNamespaceImport ("System")
            };
        }


        public CodeTypeDeclaration GenerateClass(JavaActivity activity)
        {
            var javaClass = new CodeTypeDeclaration(activity.FileName);
            javaClass.IsClass = true;
            javaClass.TypeAttributes = TypeAttributes.Public;

            javaClass.Comments.Add(new CodeCommentStatement(activity.FullSource));

            return javaClass;

        }

        public CodeStatementCollection GenerateCodeInvocation(JavaActivity javaActivity)
        {
            var invocationCodeCollection = new CodeStatementCollection();
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(javaActivity.Name));

            invocationCodeCollection.AddRange(this.xslBuilder.Build(javaActivity.InputBindings));
            var variableReturnType = new CodeTypeReference(javaActivity.PackageName + "." + javaActivity.FileName);
            var creation = new CodeObjectCreateExpression (variableReturnType, new CodeExpression[0]);

            string javaClassVariableName = VariableHelper.ToVariableName(javaActivity.FileName);

            var codeInvocation = new CodeVariableDeclarationStatement(variableReturnType, javaClassVariableName, creation);

            invocationCodeCollection.Add(codeInvocation);

            CodeVariableReferenceExpression javaClassReference = new CodeVariableReferenceExpression();
            javaClassReference.VariableName = javaClassVariableName;

            invocationCodeCollection.AddRange(this.GenerateInputCallOnJavaClass(javaActivity, javaClassReference));
            invocationCodeCollection.Add(this.GenerateInvokeCallOnJavaClass(javaClassReference));

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

        private CodeMethodInvokeExpression GenerateInvokeCallOnJavaClass(CodeVariableReferenceExpression javaClassReference)
        {
            CodeMethodInvokeExpression invokeCall = new CodeMethodInvokeExpression();
            invokeCall.Parameters.AddRange(new CodeExpression[0]);
            CodeMethodReferenceExpression invokeMethod = new CodeMethodReferenceExpression();
            invokeMethod.MethodName = "invoke";
            invokeMethod.TargetObject = javaClassReference;
            invokeCall.Method = invokeMethod;
            return invokeCall;
        }
    }
}

