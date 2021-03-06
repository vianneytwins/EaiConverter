﻿namespace EaiConverter.Builder
{
    using System.Text.RegularExpressions;

    using EaiConverter.CodeGenerator.Utils;

    public class XpathBuilder : IXpathBuilder
    {
        public string Build(string expression)
        {

            expression = ChangeStartActivityVariableName(expression);

            // TODO : Get what between $ and next / to Convert to variable name ? at least remove the . in the process name

            expression = FormatActivityNameInXpath(expression);

            expression = ManageGlobalVariable(expression);

            expression = RemovePrefix(expression);

            expression = expression.Replace("''", "\"\"");
            expression = Regex.Replace(expression, "'([^']+)'", m => "\"" + m.Groups[1].Value.Replace(@"""", @"\""") + "\"");
            //expression = expression.Replace('\'', '"');
            expression = expression.Replace('/', '.');
   
            expression = ManageTibcoXpathFunction(expression);

            expression = ManageXpathFunctions(expression);

            expression = expression.Replace(".output.GetProcessInstanceInfo", string.Empty);
            expression = expression.Replace(".output.GetProcessInstanceExceptions", string.Empty);

            // for JbdcQueryActivity
            expression = expression.Replace(".resultSet.Record[", "[");
            expression = expression.Replace(".resultSet.Record.nb", ".Count()");
            expression = expression.Replace(".resultSet.Record.Length", ".Count()");
            expression = expression.Replace(".Record", string.Empty);
            expression = expression.Replace(".resultSet.outputSet", "");
            expression = expression.Replace(".resultSet", "");

            // for java activity ouput
            expression = expression.Replace("javaCodeActivityOutput.", string.Empty);

            expression = ManageBooleanValues(expression);

            expression = ManageMathOperation(expression);

            expression = ManageCondition(expression);

            return expression;
        }

        public static string ChangeStartActivityVariableName(string expression)
        {
            expression = expression.Replace("$Start/", "$start_");
            expression = expression.Replace("$start/", "$start_");
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
            expression = expression.Replace(" div ", " / ");
            expression = expression.Replace("\ndiv\n", " / ");
            expression = expression.Replace(" or ", " || ");
            expression = expression.Replace(" or\n", " || ");
            expression = expression.Replace("\nor\n", " || ");
            expression = expression.Replace("\nor ", " || ");
            expression = expression.Replace(" and ", " && ");
            expression = expression.Replace("\nand\n", " && ");
            expression = expression.Replace("\nand ", " && ");
            expression = expression.Replace(" and\n", " && ");
            expression = expression.Replace("=", "==");
            expression = expression.Replace("!==", "!=");
            return expression;
        }

        private static string ManageBooleanValues(string expression)
        {
            expression = expression.Replace("'true()'", "true");
            expression = expression.Replace("true()", "true");
            expression = expression.Replace("'false()'", "false");
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

            // exemple of string-length : string-length (myvariable)
            expression = expression.Replace("string-length(", "TibcoXslHelper.StringLength(");

            // usage upper-case : upper-case (mystring)
            expression = expression.Replace("upper-case(", "TibcoXslHelper.UpperCase(");

            expression = expression.Replace("lower-case(", "TibcoXslHelper.LowerCase(");

            expression = expression.Replace("substring(", "TibcoXslHelper.Substring(");

            expression = expression.Replace("round(", "TibcoXslHelper.Round(");

            expression = expression.Replace("count(", "TibcoXslHelper.Count(");
            expression = expression.Replace("starts-with(", "TibcoXslHelper.StartsWith(");

            expression = expression.Replace("not(", "!(");

            return expression;
        }

        private static string ManageTibcoXpathFunction(string expression)
        {
            expression = expression.Replace("tib:left(", "TibcoXslHelper.Left(");
            expression = expression.Replace("tib:index-of(", "TibcoXslHelper.IndexOf(");
            expression = expression.Replace("tib:add-to-date(", "TibcoXslHelper.AddToDate(");
            expression = expression.Replace("tib:tokenize(", "TibcoXslHelper.Tokenize(");

            // return a string, usage sample : tib:string-round-fraction($Start/root/inputdata, 2)
            // for exemple tib:string-round-fraction(round(1.100), 2) Output as 1.00
            expression = expression.Replace("tib:string-round-fraction(", "TibcoXslHelper.StringRoundFraction(");

            //return a string, usage sample : tib:render-xml(myvariable, true()) 
            expression = expression.Replace("tib:render-xml(", "TibcoXslHelper.RenderXml(");

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

        private static string FormatActivityNameInXpath(string expression)
        {
            // @"\$([^/]*)\/"
            // \$             # Escaped parenthesis, means "starts with a '$' character"
            //    (           # Parentheses in a regex mean "put (capture) the stuff in between into the Groups array"     
            //        [^/"")]    # Any character that is not a '/', '"', ',', ')' character 
            //        *       # Zero or more occurrences of the aforementioned chars"
            //    )           # Close the capturing group
            //    \/"")          # "Ends with a '/', '"', ',', ')' character 
            //var regex = new Regex(@"\$([^/]*)\/");
            var regex = new Regex(@"\$([^/"",)]*)[/"",)]");
            var variables = regex.Matches(expression);
            foreach (Match variable in variables)
            {
                string variableNameToModify = variable.Groups[1].ToString();
                expression = expression.Replace("$" + variableNameToModify, VariableHelper.ToVariableName(variableNameToModify.Replace("$", string.Empty).Replace('-', '_').Replace('.', '_')));
            }

            //expression = expression.Replace("$", string.Empty);
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
        
        public static string RemovePrefix(string expression)
        {
            // @"pfx([^:]*)\:"
            // pfx # Escaped parenthesis, means "starts with a 'pfx' "
            //    (               # Parentheses in a regex mean "put (capture) the stuff in between into the Groups array"     
            //        [^:]        # Any character that is not a ':' character
            //        *           # Zero or more occurrences of the aforementioned "non ':' char"
            //    )               # Close the capturing group
            //    \:              # "Ends with a ':' character"

            //(?<=This is)(.*)(?=sentence)
            var regex = new Regex(@"pfx([^:]*)\:");
            //var regex = new Regex(@"(?<=pfx)(.*)(?=:)");
            var variables = regex.Match(expression);
            if (variables.Success)
            {
                string variableNameToModify = variables.Groups[0].ToString();
                expression = expression.Replace(variableNameToModify, string.Empty);
                expression = RemovePrefix(expression);
            }

            return expression;
        }
    }
}

