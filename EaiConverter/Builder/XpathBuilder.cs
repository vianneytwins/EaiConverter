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
            expression = expression.Replace("tib:parse-dateTime(","DateTime.ParseExact(");
            expression = expression.Replace("tib:parse-date(","DateTime.ParseExact(");

            expression = expression.Replace("number(","double.Parse(");
            expression = expression.Replace("tib:round-fraction(","Math.Round(");

            expression = expression.Replace("&quot;",@"""");
            expression = expression.Replace(" div "," / ");
            expression = expression.Replace(" or "," || ");
            expression = expression.Replace(" and "," && ");

            //TODO : string-lenght, tib:trim(, exists( , tib:translate-timezone(, tib:round-fraction(

            return expression;
        }
    }
}

