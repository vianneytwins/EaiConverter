namespace EaiConverter.Builder
{
    using EaiConverter.Model;
    public class ActivityBuilderFactory
    {
        public IActivityBuilder Get(ActivityType activityType)
        {
            var jdbcQueryBuilderUtils = new JdbcQueryBuilderUtils();
            var xslBuilder = new XslBuilder(new XpathBuilder());
            var jdbcQueryActivityBuilder = new JdbcQueryActivityBuilder(new DataAccessBuilder(jdbcQueryBuilderUtils), new DataAccessServiceBuilder(jdbcQueryBuilderUtils), new DataAccessInterfacesCommonBuilder(), xslBuilder);
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
                return new XmlParseActivityBuilder(xslBuilder, new XmlParserHelperBuilder());
            }
            
            if (activityType == ActivityType.mapperActivityType)
            {
                return new MapperActivityBuilder(xslBuilder);
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
            
            return new DefaultActivityBuilder(xslBuilder);
        }
    }
}
