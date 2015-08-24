using System;
using System.CodeDom;

namespace EaiConverter.Utils
{
    public class CodeDomUtils
    {
        public static CodeTypeMember GenerateProperty(string name, string type)
        {
            CodeSnippetTypeMember snippet = new CodeSnippetTypeMember();
            snippet.Text= "public " + type + " " + name +" { get; set; }";

            return snippet;
        }

        public static CodeTypeMember GeneratePropertyWithoutSetter(string name, string type)
		{
			CodeMemberProperty property = new CodeMemberProperty();
            property.Attributes = MemberAttributes.Final;
			property.Name = name;
			property.HasGet = true;
			property.HasSet = false;
			property.Type = new CodeTypeReference(type);

			return property;
		}
    }
}

