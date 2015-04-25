using System;
using System.CodeDom;

namespace EaiConverter.Model
{
    public class ActivityCodeDom
    {
        public CodeNamespaceCollection ClassesToGenerate { get; set; }

        public CodeMethodInvokeExpression InvocationCode { get; set; }
    }
}

