using System;
using System.Collections.Generic;

namespace EaiConverter.Processor
{
	public static class ConfigurationApp
	{
		static Dictionary<string,string> properties = new Dictionary<string, string> ();

		public static string GetProperty (string propertyName)
		{
			string propertyValue = String.Empty;
			properties.TryGetValue (propertyName, out propertyValue);
			return propertyValue;
		}

		public static void SaveProperty (string propertyName, string propertyValue)
		{
			if (properties.ContainsKey (propertyName)){
				properties[propertyName] =  propertyValue;
			} else {
					properties.Add(propertyName, propertyValue);
			}
		}
	}


}
