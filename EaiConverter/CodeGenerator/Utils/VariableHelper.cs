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

            variableNameToFormat = variableNameToFormat.Replace("-", "_");
            variableNameToFormat = variableNameToFormat.Replace("&", "");

            var firstCharOfTheVariable = variableNameToFormat.Substring(0, 1);
            var endOfTheVariable = variableNameToFormat.Substring(1, variableNameToFormat.Length - 1);
            return (firstCharOfTheVariable.ToLowerInvariant() + endOfTheVariable).Replace(" ", string.Empty);
        }

        public static string ToClassName(string name)
        {
            var className = name.Substring(0, 1).ToUpper() + name.Substring(1, name.Length - 1).Replace(" ", string.Empty);
            className = className.Replace("-", "_");
            className = className.Replace("&", "");
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
    }
}

