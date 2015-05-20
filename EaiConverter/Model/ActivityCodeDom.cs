using System;
using System.CodeDom;

namespace EaiConverter.Model
{
    public class ActivityCodeDom
    {
        public CodeNamespaceCollection ClassesToGenerate { get; set; }

        public CodeStatementCollection InvocationCode { get; set; }
    }
}

