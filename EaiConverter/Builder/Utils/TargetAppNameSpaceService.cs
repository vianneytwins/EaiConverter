﻿namespace EaiConverter.Builder.Utils
{
    using EaiConverter.Processor;

    public class TargetAppNameSpaceService
    {
        public const string myAppName = "MyApp";
        public const string domainServiceNamespaceName = myAppName + ".Mydomain.Service";
        public const string dataAccessNamespace = myAppName + ".Mydomain.DataAccess";
        public const string dataAccessCommonNamespace = myAppName + ".Mydomain.DataAccess.Common";
        public const string loggerNameSpace = myAppName + ".Tools.Logging";
        public const string xmlToolsNameSpace = myAppName + ".Tools.Xml";
        public const string sharedVariableNameSpace = myAppName + ".Tools.SharedVariable";
        public const string EventSourcingNameSpace = myAppName + ".Tools.EventSourcing";
        public const string domainContractNamespaceName = myAppName + ".Mydomain.Service.Contract";
        public const string javaToolsNameSpace = myAppName + ".Tools.Java";

        public const string EngineCommandNamespace = myAppName + ".Tools.EngineCommand";

        public static string ConvertXsdImportToNameSpace(string schemaLocation)
		{
		    var initialProjectPath = ConfigurationApp.GetProperty(MainClass.ProjectDirectory);
            if (initialProjectPath != null && schemaLocation.StartsWith(initialProjectPath))
		    {
		        schemaLocation = schemaLocation.Replace(initialProjectPath, string.Empty);
		    }

		    if (schemaLocation.Contains("/"))
			{
				string filePath = schemaLocation.Substring(0, schemaLocation.LastIndexOf("/"));
				filePath = filePath.Remove(0, 1);
				filePath = filePath.Remove(0, filePath.IndexOf("/") + 1);
				return filePath.Replace("/", ".");
			}

            if (schemaLocation.Contains("\\"))
            {
                string filePath = schemaLocation.Substring(0, schemaLocation.LastIndexOf("\\"));
                filePath = filePath.Remove(0, 1);
                filePath = filePath.Remove(0, filePath.IndexOf("\\") + 1);
                return filePath.Replace("\\", ".");
            }

			return schemaLocation;
		}

        public static string RemoveFirstDot(string shortNameSpace)
        {
            if (shortNameSpace.StartsWith("."))
            {
                shortNameSpace = shortNameSpace.Remove(0, 1);
            }

            return shortNameSpace;
        }
    }
}

