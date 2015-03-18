using System;
using System.Xml.Linq;

namespace EaiConverter.Parser.Utils
{
	public class XElementParserUtils
	{

		public static string GetStringValue (XElement element)
		{
			if (element == null)
			{
				return null;
			}
			if (string.IsNullOrEmpty(element.Value))
			{
				return null;
			}
			return element.Value;
		}

		public static int? GetIntValue (XElement element)
		{
			int result;
			if (element == null)
			{
				return null;
			}
			if (string.IsNullOrEmpty(element.Value))
			{
				return null;
			}
			int.TryParse (element.Value, out result);
			return result;
		}

		public static bool? GetBoolValue (XElement element)
		{
			Boolean result;
			if (element == null)
			{
				return null;
			}
			if (string.IsNullOrEmpty(element.Value))
			{
				return null;
			}
			bool.TryParse (element.Value, out result);
			return result;
		}
	}
}

