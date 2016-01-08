namespace EaiConverter.Builder.Utils
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;


    public class XpathUtils
    {
        public List<string> GetVariableNames(IEnumerable<XNode> inputBindings)
        {
            if (inputBindings == null)
            {
                return new List<string>();
            }
            var variables = this.GetAllVariables(inputBindings);
            var variablesToRemove = this.GetAllLocalVariables(inputBindings);

            foreach (var variableToRemove in variablesToRemove)
            {
                variables.Remove(variableToRemove);
            }
            variables.Remove("_globalVariables");
            return variables;
        }

        public List<string> GetAllLocalVariables(IEnumerable<XNode> inputBindings)
        {
            var variables = new List<string>();
            foreach (var inputNode in inputBindings)
            {
                var item = (XElement)inputNode;
                if (item.Name.LocalName == "variable")
                {
                    variables.Add(item.Attribute("name").Value);
                }
                if (item.HasElements)
                {
                    variables.AddRange(this.GetAllLocalVariables(item.Elements()));
                }
            }
            return variables.Distinct().ToList();
        }

        private List<string> GetAllVariables(IEnumerable<XNode> inputBindings)
        {
            var variables = new List<string>();
            foreach (var inputNode in inputBindings)
            {
                var element = (XElement)inputNode;
                var expression = element.ToString();
                expression = XpathBuilder.ChangeStartActivityVariableName(expression);
                expression = XpathBuilder.RemovePrefix(expression);
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
            //        [^/"")]    # Any character that is not a '/' character or a '"' character 
            //        *       # Zero or more occurrences of the aforementioned chars"
            //    )           # Close the capturing group
            //    \/"")          # "Ends with a '/' character" or a '"' character 
            //var regex = new Regex(@"\$([^/]*)\/");
            var regex = new Regex(@"\$([^/"",)+]*)[/"",)+]");
            var variables = regex.Matches(expression);
            var array = new Match[variables.Count];
            variables.CopyTo(array, 0);

            foreach (Match variable in variables)
            {
                string variableNameToModify = variable.Groups[1].ToString();
                expression = variableNameToModify.Replace(
                    variableNameToModify,
                    variableNameToModify.Replace("$", string.Empty).Replace('-', '_').Replace('.', '_').Replace("/", string.Empty).Replace(@"""", string.Empty));
                names.Add(expression);
            }
            
            return names;
        }

    }
}
