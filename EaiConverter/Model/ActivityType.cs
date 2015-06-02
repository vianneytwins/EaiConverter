using System;
using System.Collections.Generic;

namespace EaiConverter.Model
{
    public sealed class ActivityType {

        private readonly String name;

        private static readonly Dictionary<string, ActivityType> instance = new Dictionary<string,ActivityType>();

        public static readonly ActivityType jdbcCallActivityType = new ActivityType ("com.tibco.plugin.jdbc.JDBCCallActivity");
        public static readonly ActivityType jdbcUpdateActivityType = new ActivityType ("com.tibco.plugin.jdbc.JDBCUpdateActivity");
        public static readonly ActivityType jdbcQueryActivityType = new ActivityType ("com.tibco.plugin.jdbc.JDBCQueryActivity");
        public static readonly ActivityType xmlParseActivityType = new ActivityType ("com.tibco.plugin.xml.XMLParseActivity");
        public static readonly ActivityType mapperActivityType = new ActivityType ("com.tibco.plugin.mapper.MapperActivity");
        public static readonly ActivityType callProcessActivityType = new ActivityType ("com.tibco.pe.core.CallProcessActivity");
        public static readonly ActivityType assignActivityType = new ActivityType ("com.tibco.pe.core.AssignActivity");
        public static readonly ActivityType writeToLogActivityType = new ActivityType ("com.tibco.pe.core.WriteToLogActivity");
        public static readonly ActivityType startType = new ActivityType ("startType");
        public static readonly ActivityType endType = new ActivityType ("endType");
        public static readonly ActivityType NotHandleYet = new ActivityType ("NotHandleYet");

        private ActivityType(String name){
            this.name = name;
            instance[name] = this;
        }

        public override String ToString(){
            return name;
        }

        public static explicit operator ActivityType(string str)
        {
            ActivityType result;
            if (instance.TryGetValue(str, out result))
                return result;
            else
                throw new InvalidCastException();
        }

    }
}

