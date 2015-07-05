using System;
using System.CodeDom;

namespace EaiConverter.Utils
{
    public class CodeDomUtils
    {
        public static CodeMemberProperty GenerateProperty(string name, string type)
        {
            CodeMemberProperty property = new CodeMemberProperty();
            property.Attributes = MemberAttributes.Public;
            property.Name = name;
            property.HasGet = true;
            property.HasSet = true;
            property.Type = new CodeTypeReference(type);

            return property;
        }
    }
}

