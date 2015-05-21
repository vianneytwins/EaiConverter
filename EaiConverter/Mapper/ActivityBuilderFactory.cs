using System;
using EaiConverter.Model;
using EaiConverter.Mapper.Utils;

namespace EaiConverter.Mapper
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

                }else if(activityType == ActivityType.assignActivityType){
                    return new AssignActivityBuilder(xslBuilder);
                }
                else
                {
                   return new DefaultActivityBuilder(xslBuilder);
                }
               
        }
    }
}

