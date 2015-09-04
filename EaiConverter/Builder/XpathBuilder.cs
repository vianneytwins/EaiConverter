namespace EaiConverter.Builder
{
    using System.Text.RegularExpressions;

    using EaiConverter.CodeGenerator.Utils;

    public class XpathBuilder : IXpathBuilder
    {
        public string Build(string expression)
        {

            expression = ChangeStartActivtyVariableName(expression);

            // TODO : Get what between $ and next / to Convert to variable name ? at least remove the . in the process name

            expression = FormatActivityNameInXpath(expression);

            expression = ManageGlobalVariable(expression);

            expression = expression.Replace('\'', '"');
            expression = expression.Replace('/', '.');
   
            expression = ManageTibcoXpathFunction(expression);

            expression = ManageXpathFunctions(expression);

            //for JbdcQueryActivity
            expression = expression.Replace(".resultSet.Record[", "ResultSet[");
            expression = expression.Replace(".resultSet.Record.nb", "ResultSet.Count()");
            expression = expression.Replace(".resultSet", "ResultSet");

            expression = ManageBooleanValues(expression);

            expression = ManageMathOperation(expression);

            expression = ManageCondition(expression);

            return expression;
        }

        private static string ManageCondition(string expression)
        {
            expression = expression.Replace(" if", string.Empty);
            expression = expression.Replace("	if ", string.Empty);
            expression = expression.Replace("\nif ", string.Empty);
            expression = expression.Replace(" then ", "?");
            expression = expression.Replace(" else ", ":");
            return expression;
        }

        private static string ManageMathOperation(string expression)
        {
            //expression = expression.Replace("&quot;", "\"");
            expression = expression.Replace(" div ", " / ");
            expression = expression.Replace(" or ", " || ");
            expression = expression.Replace(" and ", " && ");
            expression = expression.Replace("\ndiv\n", " / ");
            expression = expression.Replace("\nor\n", " || ");
            expression = expression.Replace("\nor ", " || ");
            expression = expression.Replace("\nand\n", " && ");
            expression = expression.Replace("\nand ", " && ");
            expression = expression.Replace(" = ", " == ");
            expression = expression.Replace(@" =""", @" == """);
            expression = expression.Replace(" =\"", " == \"");
            return expression;
        }

        private static string ManageBooleanValues(string expression)
        {
            expression = expression.Replace("true()", "true");
            expression = expression.Replace("false()", "false");
            return expression;
        }

        private static string ManageXpathFunctions(string expression)
        {
            // concat in xsl is used like that concat(' add this', variable1, '  to that', variable2) and must be replace by something similar
            // why not
            expression = expression.Replace("concat(", "TibcoXslHelper.Concat(");

            // exemple of contains : contains ('this string', mystring)
            expression = expression.Replace("contains(", "TibcoXslHelper.Contains(");

            // exemple of exists : exists ('this string', mycollection)
            expression = expression.Replace("exists(", "TibcoXslHelper.Exist(");

            // exemple of translate : translate (myvaraible/value, '&#xA;', '')
            expression = expression.Replace("translate(", "TibcoXslHelper.Translate(");

            // exemple of current-dateTime()
            expression = expression.Replace("current-dateTime()", "DateTime.Now");

            // exemple of string-lenght : string-lenght (myvariable)
            expression = expression.Replace("string-length(", "TibcoXslHelper.StringLength(");

            // usage upper-case : upper-case (mystring)
            expression = expression.Replace("upper-case(", "TibcoXslHelper.UpperCase(");

            expression = expression.Replace("lower-case(", "TibcoXslHelper.LowerCase(");
            return expression;
        }

        private static string ManageTibcoXpathFunction(string expression)
        {
            // return a string, usage sample : tib:string-round-fraction($Start/root/inputdata, 2)
            // for exemple tib:string-round-fraction(round(1.100), 2) Output as 1.00
            expression = expression.Replace("tib:string-round-fraction(", "TibcoXslHelper.StringRoundFraction(");

            //return a string, usage sample : tib:render-xml(myvariable, true()) 
            expression = expression.Replace("tib:render-xml(", "TibcoXslHelper.RenderXml((");

            // usage tib:trim : tib:trim(myvariable) 
            expression = expression.Replace("tib:trim(", "TibcoXslHelper.Trim(");

            // usage tib:translate-timezone( : tib:translate-timezone(
            expression = expression.Replace("tib:translate-timezone(", "TibcoXslHelper.TranslateTimezone(");

            // usage tib:compare-date( : tib:compare-date(date1, date2) , return 0 if equals
            expression = expression.Replace("tib:compare-date(", "TibcoXslHelper.CompareDate(");

            // tib:getCurrentProcessName(<processID>): Fetches the process name associated with the specified <processID>
            expression = expression.Replace("tib:getCurrentProcessName(", "TibcoXslHelper.GetCurrentProcessName(");

            //tib:getCurrentActivityName(<processID>): Fetches the activity name associated with the specified <processID>
            expression = expression.Replace("tib:getCurrentActivityName(", "TibcoXslHelper.GetCurrentActivityName(");

            //tib:getHostName()
            expression = expression.Replace("tib:getHostName(", "TibcoXslHelper.GetHostName(");

            expression = expression.Replace("tib:parse-dateTime(", "TibcoXslHelper.ParseDateTime(");
            expression = expression.Replace("tib:parse-date(", "TibcoXslHelper.ParseDateTime(");
            expression = expression.Replace("tib:parse-time(", "TibcoXslHelper.ParseDateTime(");

            // tib:format-dateTime(<<format>>, <<dateTime>>)
            expression = expression.Replace("tib:format-dateTime(", "TibcoXslHelper.FormatDateTime(");
            expression = expression.Replace("tib:format-date(", "TibcoXslHelper.FormatDateTime(");
            expression = expression.Replace("tib:format-time(", "TibcoXslHelper.FormatDateTime(");

            // tib:validate-dateTime(<<format>>, <<string>>) return bool
            expression = expression.Replace("tib:validate-dateTime(", "TibcoXslHelper.ValidateDateTime(");

            expression = expression.Replace("number(", "TibcoXslHelper.ParseNumber(");
            expression = expression.Replace("number (", "TibcoXslHelper.ParseNumber(");

            expression = expression.Replace("tib:round-fraction(", "Math.Round(");
            return expression;
        }

        private static string ChangeStartActivtyVariableName(string expression)
        {
            expression = expression.Replace("$Start/", "start_");
            expression = expression.Replace("$start/", "start_");
            return expression;
        }

        private static string FormatActivityNameInXpath(string expression)
        {
            // @"\$([^/]*)\/"
            // \$             # Escaped parenthesis, means "starts with a '$' character"
            //    (           # Parentheses in a regex mean "put (capture) the stuff in between into the Groups array"     
            //        [^/]    # Any character that is not a '/' character
            //        *       # Zero or more occurrences of the aforementioned "non '/' char"
            //    )           # Close the capturing group
            //    \/          # "Ends with a '/' character"
            var regex = new Regex(@"\$([^/]*)\/");
            var variables = regex.Match(expression);
            if (variables.Success)
            {
                string variableNameToModify = variables.Groups[1].ToString();
                expression = expression.Replace(
                    variableNameToModify,
                    VariableHelper.ToVariableName(variableNameToModify.Replace('-', '_')));
            }

            expression = expression.Replace("$", string.Empty);
            return expression;
        }


        private static string ManageGlobalVariable(string expression)
        {
            // @"_globalVariables/([^:]*)\:"
            // _globalVariables/ # Escaped parenthesis, means "starts with a '_globalvariable/' "
            //    (               # Parentheses in a regex mean "put (capture) the stuff in between into the Groups array"     
            //        [^:]        # Any character that is not a ':' character
            //        *           # Zero or more occurrences of the aforementioned "non ':' char"
            //    )               # Close the capturing group
            //    \:              # "Ends with a ':' character"

            //(?<=This is)(.*)(?=sentence)
            //var regex = new Regex(@"_globalVariables/([^:]*)\:");
            var regex = new Regex(@"(?<=_globalVariables/)(.*)(?=:)");
            var variables = regex.Match(expression);
            if (variables.Success)
            {
                string variableNameToModify = variables.Groups[1].ToString();
                expression = expression.Replace(variableNameToModify + ":", string.Empty);
                expression = expression.Replace("_globalVariables/", string.Empty);
            }

            return expression;
        }
    }
}

