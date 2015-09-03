namespace EaiConverter.Builder
{
    using System.Text.RegularExpressions;

    using EaiConverter.CodeGenerator.Utils;

    public class XpathBuilder : IXpathBuilder
    {
        public string Build(string expression)
        {
            // TODO : Get what between $ and next / to Convert to variable name ? at least remove the . in the process name

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
                expression = expression.Replace(variableNameToModify, VariableHelper.ToVariableName( variableNameToModify.Replace('-','_')));
            }

            expression = expression.Replace("$", string.Empty);

            expression = expression.Replace('\'', '"');
            expression = expression.Replace('/', '.');

            //TODO swith format and date
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
            expression = expression.Replace("current-dateTime()","DateTime.Now");

            // exemple of string-lenght : string-lenght (myvariable)
            expression = expression.Replace("string-length(", "TibcoXslHelper.StringLength(");

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

            // usage upper-case : upper-case (mystring)
            expression = expression.Replace("upper-case(", "TibcoXslHelper.UpperCase(");

            expression = expression.Replace("lower-case(", "TibcoXslHelper.LowerCase(");

            // tib:getCurrentProcessName(<processID>): Fetches the process name associated with the specified <processID>
            expression = expression.Replace("tib:getCurrentProcessName(", "TibcoXslHelper.GetCurrentProcessName(");

            //tib:getCurrentActivityName(<processID>): Fetches the activity name associated with the specified <processID>
            expression = expression.Replace("tib:getCurrentActivityName(", "TibcoXslHelper.GetCurrentActivityName(");

            //tib:getHostName()
            expression = expression.Replace("tib:getHostName(", "TibcoXslHelper.GetHostName(");

            //for JbdcQueryActivity
            expression = expression.Replace(".resultSet.Record[", "ResultSet[");
            expression = expression.Replace(".resultSet.Record.nb", "ResultSet.Count()");

            expression = expression.Replace("true()", "true");
            expression = expression.Replace("false()", "false");

            expression = expression.Replace("&quot;", @"""");
            expression = expression.Replace(" div ", " / ");
            expression = expression.Replace(" or ", " || ");
            expression = expression.Replace(" and ", " && ");
            expression = expression.Replace("\ndiv\n", " / ");
            expression = expression.Replace("\nor\n", " || ");
            expression = expression.Replace("\nor ", " || ");
            expression = expression.Replace("\nand\n", " && ");
            expression = expression.Replace("\nand ", " && ");
            expression = expression.Replace(" = ", " == ");
            


            return expression;
        }
    }
}

