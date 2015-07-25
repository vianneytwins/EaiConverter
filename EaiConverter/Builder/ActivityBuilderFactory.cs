using EaiConverter.Parser;

namespace EaiConverter.Builder
{
    using EaiConverter.Model;
    public class ActivityBuilderFactory
    {
		private readonly XpathBuilder xpathBuilder;

		private readonly XslBuilder xslBuilder;

		private readonly DataAccessBuilder dataAccessBuilder;

		private readonly DataAccessServiceBuilder dataAccessServiceBuilder;

		private readonly DataAccessInterfacesCommonBuilder dataAccessInterfacesCommonBuilder;

		private readonly ResultSetBuilder resultSetBuilder;

		private readonly XmlParserHelperBuilder xmlParserHelperBuilder;
		private readonly XsdBuilder xsdBuilder;
		private readonly XsdParser xsdParser;
        
		public ActivityBuilderFactory()
		{
			this.xpathBuilder = new XpathBuilder ();
			this.xslBuilder = new XslBuilder(this.xpathBuilder);
			this.dataAccessBuilder = new DataAccessBuilder ();
			this.dataAccessServiceBuilder = new DataAccessServiceBuilder ();
			this.dataAccessInterfacesCommonBuilder = new DataAccessInterfacesCommonBuilder ();
			this.resultSetBuilder = new ResultSetBuilder ();
			this.xmlParserHelperBuilder = new XmlParserHelperBuilder ();
			this.xsdBuilder =  new XsdBuilder();
			this.xsdParser = new XsdParser();
		}

		public IActivityBuilder Get(ActivityType activityType)
        {
			var jdbcQueryActivityBuilder = new JdbcQueryActivityBuilder (this.dataAccessBuilder, this.dataAccessServiceBuilder, this.dataAccessInterfacesCommonBuilder, this.xslBuilder, this.resultSetBuilder);
            if (activityType == ActivityType.jdbcQueryActivityType || activityType == ActivityType.jdbcCallActivityType || activityType == ActivityType.jdbcUpdateActivityType)
            {
                return jdbcQueryActivityBuilder;
            }
            if (activityType == ActivityType.assignActivityType)
            {
                return new AssignActivityBuilder(xslBuilder);
            }
            
            if (activityType == ActivityType.xmlParseActivityType)
            {
				return new XmlParseActivityBuilder (xslBuilder, xmlParserHelperBuilder, this.xsdBuilder, this.xsdParser);
            }
            
            if (activityType == ActivityType.mapperActivityType)
            {
				return new MapperActivityBuilder(this.xslBuilder, this.xsdBuilder, this.xsdParser);
            }
            
            if (activityType == ActivityType.nullActivityType)
            {
                return new NullActivityBuilder(xslBuilder);
            }
            
            if (activityType == ActivityType.javaActivityType)
            {
                return new JavaActivityBuilder(xslBuilder);
            }
            
            if (activityType == ActivityType.writeToLogActivityType)
            {
                return new WriteToLogActivityBuilder(xslBuilder);
            }
            
            if (activityType == ActivityType.generateErrorActivity)
            {
                return new GenerateErrorActivityBuilder(xslBuilder);
            }
            if (activityType == ActivityType.sleepActivity)
            {
                return new SleepActivityBuilder(xslBuilder);
            }
            if (activityType == ActivityType.loopGroupActivityType || activityType == ActivityType.criticalSectionGroupActivityType)
            {
                return new GroupActivityBuilder(xslBuilder);
            }
			if (activityType == ActivityType.rdvEventSourceActivityType)
			{
				return new RdvEventSourceActivityBuilder();
			}
            
            return new DefaultActivityBuilder(xslBuilder);
        }
    }
}
