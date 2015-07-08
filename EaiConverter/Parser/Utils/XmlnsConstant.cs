using System.Xml.Linq;

namespace EaiConverter.Parser.Utils
{
    public class XmlnsConstant
    {
        public static XNamespace tibcoProcessNameSpace = "http://xmlns.tibco.com/bw/process/2003";
        public static string xslNameSpace = "http://(www.)?w3.org/1999/XSL/Transform";
        public static XNamespace xsiNameSpace = "http://www.w3.org/2001/XMLSchema-instance";
        public static XNamespace xsdNameSpace = "http://www.w3.org/2001/XMLSchema";
        public static XNamespace writeToLogActivityNameSpace = "http://www.tibco.com/pe/WriteToLogActivitySchema";
        public static XNamespace generateErrorActivityNameSpace = "http://www.tibco.com/pe/GenerateErrorActivitySchema";
        public static XNamespace globalVariableNameSpace = "http://www.tibco.com/xmlns/repo/types/2002";
        public static XNamespace sleeptibcoActivityNameSpace = "www.tibco.com/plugin/Sleep";
    }
}

