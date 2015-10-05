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

        private readonly SubscriberInterfaceBuilder subscriberBuilder;

        private readonly EngineCommandServiceHelperBuilder engineCommandServiceHelperBuilder;

        public ActivityBuilderFactory()
		{
			this.xpathBuilder = new XpathBuilder ();
			this.xslBuilder = new XslBuilder(this.xpathBuilder);
			this.dataAccessBuilder = new DataAccessBuilder ();
			this.dataAccessServiceBuilder = new DataAccessServiceBuilder ();
			this.dataAccessInterfacesCommonBuilder = new DataAccessInterfacesCommonBuilder ();
			this.resultSetBuilder = new ResultSetBuilder ();
			this.xmlParserHelperBuilder = new XmlParserHelperBuilder ();
            this.engineCommandServiceHelperBuilder = new EngineCommandServiceHelperBuilder();
			this.xsdBuilder =  new XsdBuilder();
			this.xsdParser = new XsdParser();
            this.subscriberBuilder = new SubscriberInterfaceBuilder();
		}

		public IActivityBuilder Get(ActivityType activityType)
        {
			var jdbcQueryActivityBuilder = new JdbcQueryActivityBuilder (this.dataAccessBuilder, this.dataAccessServiceBuilder, this.dataAccessInterfacesCommonBuilder, this.xslBuilder, this.resultSetBuilder);
            if (activityType == ActivityType.jdbcQueryActivityType || activityType == ActivityType.jdbcCallActivityType || activityType == ActivityType.jdbcUpdateActivityType)
            {
                return jdbcQueryActivityBuilder;
            }
            else if (activityType == ActivityType.assignActivityType)
            {
                return new AssignActivityBuilder(this.xslBuilder);
            }
			else if (activityType == ActivityType.xmlParseActivityType)
            {
				return new XmlParseActivityBuilder (this.xslBuilder, this.xmlParserHelperBuilder, this.xsdBuilder, this.xsdParser);
            }
			else if (activityType == ActivityType.EngineCommandActivityType)
            {
				return new EngineCommandActivityBuilder(this.xslBuilder, this.engineCommandServiceHelperBuilder, this.xsdBuilder);
            }
            else if (activityType == ActivityType.mapperActivityType)
            {
				return new MapperActivityBuilder(this.xslBuilder, this.xsdBuilder, this.xsdParser);
            }
			else if (activityType == ActivityType.nullActivityType)
            {
                return new NullActivityBuilder();
            }
			else if (activityType == ActivityType.javaActivityType)
            {
                return new JavaActivityBuilder(this.xslBuilder);
            }
            else if (activityType == ActivityType.callProcessActivityType)
            {
                return new CallProcessActivityBuilder(this.xslBuilder);
            }
			else if (activityType == ActivityType.writeToLogActivityType)
            {
                return new WriteToLogActivityBuilder(this.xslBuilder);
            }
            else if (activityType == ActivityType.generateErrorActivity)
            {
                return new GenerateErrorActivityBuilder(this.xslBuilder);
            }
            else if (activityType == ActivityType.sleepActivity)
            {
                return new SleepActivityBuilder(this.xslBuilder);
            }
            else if (activityType == ActivityType.loopGroupActivityType || activityType == ActivityType.criticalSectionGroupActivityType)
            {
                return new GroupActivityBuilder(this.xslBuilder);
            }
			else if (activityType == ActivityType.RdvEventSourceActivityType)
			{
                return new RdvEventSourceActivityBuilder(this.subscriberBuilder);
			}
			else if (activityType == ActivityType.rdvPubActivityType)
			{
				return new RdvPublishActivityBuilder(this.xslBuilder);
			}
            else if (activityType == ActivityType.TimerEventSource)
			{
                return new TimerActivityBuilder(this.subscriberBuilder);
			}
            else if (activityType == ActivityType.AeSubscriberActivity || activityType == ActivityType.OnStartupEventSource)
            {
                return new DefaultSubscriberBuilder(this.subscriberBuilder);
            }
            else if (activityType == ActivityType.setSharedVariableActivityType )
            {
                return new SetSharedVariableActivityBuilder(this.xslBuilder);
            }
            else if (activityType == ActivityType.getSharedVariableActivityType )
            {
                return new GetSharedVariableActivityBuilder(this.xslBuilder);
            }
            else if (activityType == ActivityType.ConfirmActivityType)
            {
                return new ConfirmActivityBuilder();
            }

            return new DefaultActivityBuilder(this.xslBuilder);
        }
    }
}
