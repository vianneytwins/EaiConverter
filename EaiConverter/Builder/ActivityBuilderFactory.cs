using System;
using EaiConverter.Model;
using EaiConverter.Builder.Utils;

namespace EaiConverter.Builder
{
    public class ActivityBuilderFactory
    {
        public IActivityBuilder Get(ActivityType activityType){
            var jdbcQueryBuilderUtils = new JdbcQueryBuilderUtils ();
            var xslBuilder = new XslBuilder (new XpathBuilder());
            var jdbcQueryActivityBuilder = new JdbcQueryActivityBuilder (new DataAccessBuilder(jdbcQueryBuilderUtils), new DataAccessServiceBuilder(jdbcQueryBuilderUtils), new DataAccessInterfacesCommonBuilder(), xslBuilder);

            if (activityType == ActivityType.jdbcQueryActivityType || activityType == ActivityType.jdbcCallActivityType || activityType == ActivityType.jdbcUpdateActivityType)
            {
                return jdbcQueryActivityBuilder;

            }
            else if (activityType == ActivityType.assignActivityType)
            {
                return new AssignActivityBuilder(xslBuilder);
            }
            else if (activityType == ActivityType.xmlParseActivityType)
            {
                return new XmlParseActivityBuilder(xslBuilder, new XmlParserHelperBuilder());
            }
            else if (activityType == ActivityType.mapperActivityType)
            {
                return new MapperActivityBuilder(xslBuilder);
            }
            else if (activityType == ActivityType.nullActivityType)
            {
                return new NullActivityBuilder(xslBuilder);
            }
            else if (activityType == ActivityType.javaActivityType)
            {
                return new JavaActivityBuilder(xslBuilder);
            }
            else if (activityType == ActivityType.writeToLogActivityType)
            {
                return new WriteToLogActivityBuilder(xslBuilder);
            }
            else if (activityType == ActivityType.generateErrorActivity)
            {
                return new GenerateErrorActivityBuilder(xslBuilder);
            }
            else
            {
               return new DefaultActivityBuilder(xslBuilder);
            }
               
        }
    }
}

