//Need to add them both to import

namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Reflection;

    using EaiConverter.Builder.Utils;

    public class XmlParserHelperBuilder
    {
        public const string XmlParserHelperServiceName = "XmlParserHelperService";

        public const string IXmlParserHelperServiceName = "IXmlParserHelperService";

        public const string FromXmlMethodName = "FromXml";

        public CodeNamespaceCollection Build()
        {
            var xmlParserHelperNameSpace = new CodeNamespace(TargetAppNameSpaceService.xmlToolsNameSpace());

            // Generate the Service
            xmlParserHelperNameSpace.Imports.AddRange(this.GenerateImports());
            var xmlParserHelperService = this.GenerateClass();
            xmlParserHelperNameSpace.Types.Add(xmlParserHelperService);

            // Generate the corresponding interface
            var xmlParserHelperServiceInterfaceNameSpace = InterfaceExtractorFromClass.Extract(xmlParserHelperService, TargetAppNameSpaceService.xmlToolsNameSpace());

            ModuleBuilder.AddServiceToRegister(IXmlParserHelperServiceName, XmlParserHelperServiceName);
            return new CodeNamespaceCollection{xmlParserHelperNameSpace, xmlParserHelperServiceInterfaceNameSpace};
        }

        public CodeNamespaceImport[] GenerateImports()
        {
            return new CodeNamespaceImport[3] {
                new CodeNamespaceImport("System"),
                new CodeNamespaceImport("System.IO"),
                new CodeNamespaceImport("System.Xml.Serialization")
            };
        }


        public CodeTypeDeclaration GenerateClass()
        {
            var xmlParserHelperService = new CodeTypeDeclaration(XmlParserHelperServiceName);
            xmlParserHelperService.IsClass = true;
            xmlParserHelperService.TypeAttributes = TypeAttributes.Public;
            xmlParserHelperService.BaseTypes.Add(new CodeTypeReference(IXmlParserHelperServiceName));

            xmlParserHelperService.Members.Add(this.GenerateFromXmlMethod());

            return xmlParserHelperService;
        }


        public CodeMemberMethod GenerateFromXmlMethod()
        {
            var fromXmlMethod = new CodeMemberMethod();

            fromXmlMethod.Name = FromXmlMethodName;
            fromXmlMethod.Attributes = MemberAttributes.Final | MemberAttributes.Public;
           
            CodeTypeParameter tTypeParameter = new CodeTypeParameter("T");
            //tType.HasConstructorConstraint = true;


            fromXmlMethod.TypeParameters.Add(tTypeParameter);
            fromXmlMethod.ReturnType = new CodeTypeReference("T");
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

