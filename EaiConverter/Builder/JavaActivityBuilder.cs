using System;
using EaiConverter.Model;
using System.CodeDom;
using EaiConverter.Builder.Utils;
using EaiConverter.CodeGenerator.Utils;
using System.Reflection;

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

            var variableToAssignReference = new CodeFieldReferenceExpression ( new CodeThisReferenceExpression (), VariableHelper.ToVariableName(javaActivity.FileName));
            var codeInvocation = new CodeAssignStatement (variableToAssignReference, new CodeVariableReferenceExpression(VariableHelper.ToVariableName(javaActivity.FileName)));
            invocationCodeCollection.Add(codeInvocation);
            return invocationCodeCollection;
        }

    }
}

