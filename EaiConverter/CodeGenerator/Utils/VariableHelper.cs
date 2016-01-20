namespace EaiConverter.CodeGenerator.Utils
{
    public class VariableHelper
    {
        public static string ToVariableName(string variableNameToFormat)
        {
            if (string.IsNullOrWhiteSpace(variableNameToFormat))
            {
                return string.Empty;
            }

            variableNameToFormat = ToSafeType(variableNameToFormat);

            variableNameToFormat = RemoveSpecialChar(variableNameToFormat);

            var firstCharOfTheVariable = variableNameToFormat.Substring(0, 1);
            var endOfTheVariable = variableNameToFormat.Substring(1, variableNameToFormat.Length - 1);
            return (firstCharOfTheVariable.ToLowerInvariant() + endOfTheVariable).Replace(" ", string.Empty);
        }

        private static string RemoveSpecialChar(string variableNameToFormat)
        {
            variableNameToFormat = variableNameToFormat.Replace("-", "_");
            variableNameToFormat = variableNameToFormat.Replace("&", "");
            return variableNameToFormat;
        }

        public static string ToClassName(string name)
        {
            var className = name.Substring(0, 1).ToUpper() + name.Substring(1, name.Length - 1).Replace(" ", string.Empty);
            className = RemoveSpecialChar(className);
            return className;
        }

        public static string ToSafeType(string variableNameToFormat)
        {
            if (string.IsNullOrWhiteSpace(variableNameToFormat))
            {
                return string.Empty;
            }

            if (variableNameToFormat == "interface")
            {
                return "@interface";
            }

            if (variableNameToFormat == "object")
            {
                return "@object";
            }

            if (variableNameToFormat == "param")
            {
                return "@param";
            }
            
            if (variableNameToFormat == "params")
            {
                return "@params";
            }

            if (variableNameToFormat[0] >= '0' && variableNameToFormat[0] <= '9')
            {
                return "a" + variableNameToFormat;
            }

            return variableNameToFormat;
        }

        public static string ToSafeType(string parent, string variableNameToFormat)
        {
            var variableNameToLowerCase = RemoveSpecialChar(variableNameToFormat).ToLower();
            if (parent != null && (variableNameToLowerCase == "interface"
                || variableNameToLowerCase == "object"
                || variableNameToLowerCase == "param"
                || variableNameToLowerCase == "params"
                || variableNameToLowerCase == "subtypeinfos"
                || variableNameToLowerCase == "keys"))
            {
                return parent + ToClassName(variableNameToFormat);
            }


/**            if (variableNameToFormat[0] >= '0' && variableNameToFormat[0] <= '9')
            {
                return "a" + variableNameToFormat;
            }*/

            return ToSafeType(variableNameToFormat);
        }


    }
}

