namespace EaiConverter.Builder.Utils
{
    using EaiConverter.Processor;

    public class TargetAppNameSpaceService
    {
        public static string myAppName = "MyApp";
        public static string domainServiceNamespaceName = myAppName + ".Mydomain.Service";
        public static string dataAccessNamespace = myAppName + ".Mydomain.DataAccess";
        public static string dataAccessCommonNamespace = myAppName + ".Mydomain.DataAccess.Common";
        public static string loggerNameSpace = myAppName + ".Tools.Logging";
        public static string xmlToolsNameSpace = myAppName + ".Tools.Xml";
        public static string sharedVariableNameSpace = myAppName + ".Tools.SharedVariable";
        public static string EventSourcingNameSpace = myAppName + ".Tools.EventSourcing";
        public static string domainContractNamespaceName = myAppName + ".Mydomain.Service.Contract";
        public static string javaToolsNameSpace = myAppName + ".Tools.Java";

        public static string EngineCommandNamespace = myAppName + ".Tools.EngineCommand";

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

