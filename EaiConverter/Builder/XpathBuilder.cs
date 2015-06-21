using System;

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
            expression = expression.Replace("tib:parse-dateTime(","TibcoXslHelper.DateTime(");
            expression = expression.Replace("tib:parse-date(","TibcoXslHelper.DateTime(");

            expression = expression.Replace("number(","TibcoXslHelper.Number(");
            expression = expression.Replace("tib:round-fraction(","Math.Round(");

            // concat in xsl is used like that concat(' add this', variable1, '  to that', variable2) and must be replace by something similar
            // why not
            expression = expression.Replace("concat(","TibcoXslHelper.Concat(");

            // exemple of contains : contains ('this string', mystring)
            expression = expression.Replace("contains(","TibcoXslHelper.Contains(");

            // exemple of exists : exists ('this string', mycollection)
            expression = expression.Replace("exists(","TibcoXslHelper.Exist(");

            // exemple of translate : translate (myvaraible/value, '&#xA;', '')
            expression = expression.Replace("translate(","TibcoXslHelper.Exist(");

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
            expression = expression.Replace("tib:compare-date(","TibcoXslHelper.TranslateTimezone(");

            // usage upper-case : upper-case (mystring)
            expression = expression.Replace("upper-case(","TibcoXslHelper.UpperCase(");

            expression = expression.Replace("&quot;",@"""");
            expression = expression.Replace(" div "," / ");
            expression = expression.Replace(" or "," || ");
            expression = expression.Replace(" and "," && ");

            //TODO tib:trim(, , tib:translate-timezone(, 
            return expression;
        }

    }
}

