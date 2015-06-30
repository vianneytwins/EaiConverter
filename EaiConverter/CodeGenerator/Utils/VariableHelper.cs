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
            var firstCharOfTheVariable = variableNameToFormat.Substring(0, 1);
            var endOfTheVariable = variableNameToFormat.Substring(1, variableNameToFormat.Length - 1);
            return (firstCharOfTheVariable.ToLowerInvariant() + endOfTheVariable).Replace(" ", string.Empty);
        }

        public static string ToClassName(string name)
        {
            return name.Substring(0, 1).ToUpper() + name.Substring(1, name.Length - 1).Replace(" ", string.Empty);
        }
    }
}

