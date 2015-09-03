namespace EaiConverter.Utils
{
    using System;
    using System.CodeDom;

    public static class CodeDomUtils
    {
        public static CodeTypeMember GenerateProperty(string name, string type)
        {
            var snippet = new CodeSnippetTypeMember();
            snippet.Text = "public " + type + " " + name + " { get; set; }";

            return snippet;
        }

        public static CodeTypeMember GeneratePropertyWithoutSetter(string name, string type)
		{
            var snippet = new CodeSnippetTypeMember();
            snippet.Text = "public " + type + " " + name + " { get; private set; }";

            return snippet;
		}
    }
}

