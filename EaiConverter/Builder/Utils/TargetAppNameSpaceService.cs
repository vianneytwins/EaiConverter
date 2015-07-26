namespace EaiConverter.Builder.Utils
{
    public class TargetAppNameSpaceService
    {
        public const string processNameSpace = "MyApp.Mydomain.Process";
        public const string domainServiceNamespaceName = "MyApp.Mydomain.Service";
        public const string dataAccessNamespace = "MyApp.Mydomain.DataAccess";
        public const string dataAccessCommonNamespace = "MyApp.Mydomain.DataAccess.Common";
        public const string loggerNameSpace = "MyApp.Tools.Logging";
        public const string xmlToolsNameSpace = "MyApp.Tools.Xml";
		public const string EventSourcingNameSpace = "MyApp.Tools.EventSourcing";
        public const string domainContractNamespaceName = "MyApp.Mydomain.Service.Contract";
        public const string javaToolsNameSpace = "MyApp.Tools.Java";

		public static string ConvertXsdImportToNameSpace(string schemaLocation)
		{
			if (schemaLocation.Contains("/"))
			{
				string filePath = schemaLocation.Substring(0, schemaLocation.LastIndexOf("/"));
				filePath = filePath.Remove(0, 1);
				filePath = filePath.Remove(0, filePath.IndexOf("/") + 1);
				return filePath.Replace("/", ".");
			}

			return schemaLocation;
		}
    }
}

