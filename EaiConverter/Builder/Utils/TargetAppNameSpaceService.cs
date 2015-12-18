namespace EaiConverter.Builder.Utils
{
    using EaiConverter.Processor;

    public class TargetAppNameSpaceService
    {
        private static string myAppFirstName = "MyApp";

        public static string MyAppName { get; set; }

        public static string myAppName()
        {
            return MyAppName == null ? myAppFirstName: MyAppName ;
        }

        public static string domainServiceNamespaceName()
        {
            return myAppName() + ".Mydomain.Service";
        }

        public static string dataAccessNamespace()
        {
            return myAppName() + ".Mydomain.DataAccess";
        }

        public static string dataAccessCommonNamespace()
        {
            return myAppName() + ".Mydomain.DataAccess.Common";
        }

        public static string loggerNameSpace()
        {
            return myAppName() + ".Tools.Logging";
        }

        public static string xmlToolsNameSpace()
        {
            return myAppName() + ".Tools.Xml";
        }

        public static string sharedVariableNameSpace()
        {
            return myAppName() + ".Tools.SharedVariable";
        }

        public static string EventSourcingNameSpace()
        {
            return myAppName() + ".Tools.EventSourcing";
        }

        public static string domainContractNamespaceName()
        {
            return myAppName() + ".Mydomain.Service.Contract";
        }

        public static string javaToolsNameSpace()
        {
            return myAppName() + ".Tools.Java";
        }

        public static string EngineCommandNamespace()
        {
            return myAppName() + ".Tools.EngineCommand";
        }

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

