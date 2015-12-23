namespace EaiConverter.Builder.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    using EaiConverter.CodeGenerator.Utils;

    public class XpathUtils
    {
        public List<string> GetVariableNames(IEnumerable<XNode> inputBindings)
        {
            var variables = new List<string>();
            foreach (var inputNode in inputBindings)
            {
                var element = (XElement)inputNode;
                var expression = element.ToString();
                variables.AddRange(this.GetVariableNames(expression));

            }
            return variables.Distinct().ToList();
        }

        public List<string> GetVariableNames(string expression)
        {
            var names = new List<string>();
            
            // @"\$([^/]*)\/"
            // \$             # Escaped parenthesis, means "starts with a '$' character"
            //    (           # Parentheses in a regex mean "put (capture) the stuff in between into the Groups array"     
            //        [^/""]    # Any character that is not a '/' character or a '"' character 
            //        *       # Zero or more occurrences of the aforementioned "non '/' char"
            //    )           # Close the capturing group
            //    \/          # "Ends with a '/' character" or a '"' character
            //var regex = new Regex(@"\$([^/]*)\/");
            var regex = new Regex(@"\$([^/""]*)[/""]");
            var variables = regex.Matches(expression);
            var array = new Match[variables.Count];
            variables.CopyTo(array, 0);

            foreach (Match variable in variables)
            {
                string variableNameToModify = variable.Groups[0].ToString();
                expression = variableNameToModify.Replace(
                    variableNameToModify,
                    VariableHelper.ToVariableName(variableNameToModify.Replace("$", string.Empty).Replace('-', '_').Replace('.', '_').Replace("/", string.Empty).Replace(@"""", string.Empty)));
                names.Add(expression);
            }
            
            return names;
        }

    }
}
