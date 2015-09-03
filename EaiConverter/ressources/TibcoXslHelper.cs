namespace MyApp.Tools.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class TibcoXslHelper
    {
        public static DateTime ParseDateTime(string format, string inputDate)
        {
            return DateTime.ParseExact(inputDate, format, null);
        }

        public static string FormatDateTime(string format, DateTime inputDate)
        {
            return string.Format(format, inputDate);
        }

        public static double ParseNumber(string numberInString)
        {
            return double.Parse(numberInString);
        }
            
        public static string Concat(params object[] list)
        {
            return string.Concat(list);
        }
            
        public static bool Contains(string value, string inputString)
        {
            return inputString.Contains(value);
        }

        // usage of exists : exists ('this string', mycollection)
        public static bool Exist<T>(T value, List<T> collection)
        {
            return collection.Contains(value);
        }

        // usage of translate : translate (myvaraible/value, '&#xA;', '')
        public static string Translate(string inputString, string oldstring, string newstring)
        {
            return inputString.Replace(oldstring, newstring);
        }

        // usage of string-lenght : string-lenght (myvariable)
        public static int StringLength(string inputString)
        {
            return inputString.Length;
        }

        //usage a string, usage sample : tib:render-xml(myvariable, true()) 
        public static string RenderXml(string inputString, bool isSomething)
        {
            return inputString;
        }

        // usage tib:trim : tib:trim(myvariable)
        public static string Trim(string inputString)
        {
            return inputString.Trim();
        }

        // usage tib:translate-timezone( : tib:translate-timezone(
        // TODO find usage exemple
        public static string TranslateTimezone(string timezone)
        {
            return timezone;
        }

        // usage tib:compare-date( : tib:compare-date(date1, date2) , return 0 if equals
        //expression = expression.Replace("tib:compare-date(","TibcoXslHelper.CompareDate(");
        public static int CompareDate(DateTime date1, DateTime date2)
        {
            return date1.CompareTo(date2);
        }

        // usage upper-case : upper-case (mystring)
        public static string UpperCase(string inputString)
        {
            return inputString.ToUpper();
        }

        public static string LowerCase(string inputString)
        {
            return inputString.ToLower();
        }

        // tib:validate-dateTime(<<format>>, <<string>>) return bool
        public static bool ValidateDateTime(string format, string inputDate)
        {
            try
            {
                DateTime.ParseExact(inputDate, format, null);
            }
            catch (System.FormatException ex)
            {
                return false;
            }

            return true;
        }

        // return a string, usage sample : tib:string-round-fraction($Start/root/inputdata, 2)
        // for exemple tib:string-round-fraction(round(1.100), 2) Output as 1.00
        public string StringRoundFraction(string myNumber, int nbDecimal)
        {
            return Math.Round(decimal.Parse(myNumber), nbDecimal).ToString(CultureInfo.InvariantCulture);
        }
    }
}

