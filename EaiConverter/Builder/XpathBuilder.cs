namespace EaiConverter.Builder
{
    public class XpathBuilder : IXpathBuilder
    {
        public string Build(string expression){
            // TODO : Get what between $ and next / to Convert to variable name ? at least remove the . in the process name
            expression = expression.Replace("$",string.Empty);

            expression = expression.Replace('\'', '"');
            expression = expression.Replace('/','.');

            //TODO swith format and date
            expression = expression.Replace("tib:parse-dateTime(","TibcoXslHelper.ParseDateTime(");
            expression = expression.Replace("tib:parse-date(","TibcoXslHelper.ParseDateTime(");
            expression = expression.Replace("tib:parse-time(","TibcoXslHelper.ParseDateTime(");


            //tib:format-dateTime(<<format>>, <<dateTime>>)
            expression = expression.Replace("tib:format-dateTime(","TibcoXslHelper.FormatDateTime(");
            expression = expression.Replace("tib:format-date(","TibcoXslHelper.FormatDateTime(");
            expression = expression.Replace("tib:format-time(","TibcoXslHelper.FormatDateTime(");

            expression = expression.Replace("number(","TibcoXslHelper.ParseNumber(");
            expression = expression.Replace("tib:round-fraction(","Math.Round(");

            // concat in xsl is used like that concat(' add this', variable1, '  to that', variable2) and must be replace by something similar
            // why not
            expression = expression.Replace("concat(","TibcoXslHelper.Concat(");

            // exemple of contains : contains ('this string', mystring)
            expression = expression.Replace("contains(","TibcoXslHelper.Contains(");

            // exemple of exists : exists ('this string', mycollection)
            expression = expression.Replace("exists(","TibcoXslHelper.Exist(");

            // exemple of translate : translate (myvaraible/value, '&#xA;', '')
            expression = expression.Replace("translate(","TibcoXslHelper.Translate(");

            // exemple of current-dateTime()
            expression = expression.Replace("current-dateTime()","DateTime.Now");

            // exemple of string-lenght : string-lenght (myvariable)
            expression = expression.Replace("string-lenght(","TibcoXslHelper.StringLength(");

            //return a string, usage sample : tib:render-xml(myvariable, true()) 
            expression = expression.Replace("tib:render-xml(","TibcoXslHelper.RenderXml((");

            // usage tib:trim : tib:trim(myvariable) 
            expression = expression.Replace("tib:trim(","TibcoXslHelper.Trim(");

            // usage tib:translate-timezone( : tib:translate-timezone(
            expression = expression.Replace("tib:translate-timezone(","TibcoXslHelper.TranslateTimezone(");


            // usage tib:compare-date( : tib:compare-date(date1, date2) , return 0 if equals
            expression = expression.Replace("tib:compare-date(","TibcoXslHelper.CompareDate(");

            // usage upper-case : upper-case (mystring)
            expression = expression.Replace("upper-case(","TibcoXslHelper.UpperCase(");

            expression = expression.Replace("lower-case(","TibcoXslHelper.LowerCase(");

            // tib:getCurrentProcessName(<processID>): Fetches the process name associated with the specified <processID>
            expression = expression.Replace("tib:getCurrentProcessName(","TibcoXslHelper.GetCurrentProcessName(");

            //tib:getCurrentActivityName(<processID>): Fetches the activity name associated with the specified <processID>
            expression = expression.Replace("tib:getCurrentActivityName(","TibcoXslHelper.GetCurrentActivityName(");

            //tib:getHostName()
            expression = expression.Replace("tib:getHostName(","TibcoXslHelper.GetHostName(");

            expression = expression.Replace("&quot;",@"""");
            expression = expression.Replace(" div "," / ");
            expression = expression.Replace(" or "," || ");
            expression = expression.Replace(" and "," && ");

            return expression;
        }
    }
}

