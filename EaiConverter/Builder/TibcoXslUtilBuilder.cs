using System.CodeDom;
using EaiConverter.Builder.Utils;
using System.Reflection;

namespace EaiConverter.Builder
{
    public class TibcoXslUtilBuilder
    {
        public const string TibcoXslHelperServiceName = "TibcoXslHelper";

        public const string FromXmlMethodName = "FromXml";

        public CodeNamespaceCollection Build(){
            var TibcoXslHelperNameSpace = new CodeNamespace(TargetAppNameSpaceService.xmlToolsNameSpace);

            // Generate the Service
            TibcoXslHelperNameSpace.Imports.AddRange(this.GenerateImports());
            var TibcoXslHelperClass = this.GenerateClass();
            TibcoXslHelperNameSpace.Types.Add(TibcoXslHelperClass);

            return new CodeNamespaceCollection{TibcoXslHelperNameSpace};
        }

        public CodeNamespaceImport[] GenerateImports()
        {
            return new CodeNamespaceImport[3] {
                new CodeNamespaceImport ("System"),
                new CodeNamespaceImport ("System.IO"),
                new CodeNamespaceImport ("System.Xml.Serialization")
            };
        }


        public CodeTypeDeclaration GenerateClass()
        {
            var xmlParserHelperService = new CodeTypeDeclaration(TibcoXslHelperServiceName);
            xmlParserHelperService.IsClass = true;
            xmlParserHelperService.TypeAttributes = TypeAttributes.Public;

            //xmlParserHelperService.Members.Add(this.GenerateNumberMethod());
            //xmlParserHelperService.Members.Add(this.GenerateParseDateMethod());
            //xmlParserHelperService.Members.Add(this.GenerateFromXmlMethod());

            return xmlParserHelperService;
        }
        
        public CodeMemberMethod GenerateFromXmlMethod()
        {
            CodeMemberMethod fromXmlMethod = new CodeMemberMethod();

            fromXmlMethod.Name = FromXmlMethodName;
            fromXmlMethod.Attributes = MemberAttributes.Final | MemberAttributes.Public|MemberAttributes.Static;

            CodeTypeParameter tTypeParameter = new CodeTypeParameter("T");
            //tType.HasConstructorConstraint = true;


            fromXmlMethod.TypeParameters.Add(tTypeParameter);
            fromXmlMethod.ReturnType = new CodeTypeReference("T");;
            //fromXmlMethod.Statements.Add();
            fromXmlMethod.Parameters.Add(new CodeParameterDeclarationExpression(
                new CodeTypeReference("String"), "xml"));
            fromXmlMethod.Comments.Add(new CodeCommentStatement("Call it using this code: YourStrongTypedEntity entity = FromXml<YourStrongTypedEntity>(YourMsgString);"));
            fromXmlMethod.Statements.Add(new CodeSnippetStatement(@"        T returnedXmlClass = default(T);

        using (TextReader reader = new StringReader(xml))
        {
            returnedXmlClass = (T)new XmlSerializer(typeof(T)).Deserialize(reader);
        }
        return returnedXmlClass ; "));

            return fromXmlMethod;
        }
    }
}

