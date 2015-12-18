namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Reflection;

    using EaiConverter.Builder.Utils;

    public class TibcoXslUtilBuilder
    {
        public const string TibcoXslHelperServiceName = "TibcoXslHelper";

        public CodeNamespaceCollection Build()
        {
            var TibcoXslHelperNameSpace = new CodeNamespace(TargetAppNameSpaceService.xmlToolsNameSpace());

            // Generate the Service
            TibcoXslHelperNameSpace.Imports.AddRange(this.GenerateImports());
            var TibcoXslHelperClass = this.GenerateClass();
            TibcoXslHelperNameSpace.Types.Add(TibcoXslHelperClass);

            return new CodeNamespaceCollection{TibcoXslHelperNameSpace};
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
            var tibcoXslHelper = new CodeTypeDeclaration(TibcoXslHelperServiceName);
            tibcoXslHelper.IsClass = true;
            tibcoXslHelper.TypeAttributes = TypeAttributes.Public;

            //xmlParserHelperService.Members.Add(this.GenerateNumberMethod());
            //xmlParserHelperService.Members.Add(this.GenerateParseDateMethod());
            //xmlParserHelperService.Members.Add(this.GenerateFromXmlMethod());

            return tibcoXslHelper;
        }
    }
}

