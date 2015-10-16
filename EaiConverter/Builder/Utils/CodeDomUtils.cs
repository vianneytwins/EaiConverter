namespace EaiConverter.Utils
{
    using System;
    using System.CodeDom;

    public static class CodeDomUtils
    {
        public static CodeTypeMember GenerateProperty(string name, string type)
        {
            var snippet = new CodeSnippetTypeMember();
            snippet.Text = "        public " + type + " " + name + " { get; set; }";

            return snippet;
        }
        
        public static CodeTypeMember GenerateStaticProperty(string name, string type)
        {
            var snippet = new CodeSnippetTypeMember();
            snippet.Text = "        public static " + type + " " + name + " { get; set; }";

            return snippet;
        }

        public static CodeTypeMember GeneratePropertyWithoutSetter(string name, string type)
		{
            var snippet = new CodeSnippetTypeMember();
            snippet.Text = "        public " + type + " " + name + " { get; private set; }";

            return snippet;
		}

        public static CodeTypeMember GeneratePropertyWithoutSetterForInterface(string name, string type)
        {
            var snippet = new CodeSnippetTypeMember();
            snippet.Text = "        " + type + " " + name + " { get; }";

            return snippet;
        }


        public static bool IsBasicType(string type)
        {
            switch (type)
            {
                case "string":
                    return true;
                case CSharpTypeConstant.SystemInt32:
                    return true;
                case CSharpTypeConstant.SystemString:
                    return true;
                case CSharpTypeConstant.SystemDateTime:
                    return true;
                case CSharpTypeConstant.SystemDouble:
                    return true;
                case CSharpTypeConstant.SystemBoolean:
                    return true;
                case "int":
                    return true;
                case "DateTime":
                    return true;
                case "bool":
                    return true;
                case "double":
                    return true;
                default:
                    return false;
            }
        }

        public static string GetCorrectBasicType(string type)
        {
            switch (type)
            {
                case "string":
                    return CSharpTypeConstant.SystemString;
                case "int":
                    return CSharpTypeConstant.SystemInt32;
                case "bool":
                    return CSharpTypeConstant.SystemBoolean;
                case "double":
                    return CSharpTypeConstant.SystemDouble;
                default:
                    return type;
            }
        }
    }
}

