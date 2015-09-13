namespace EaiConverter.Model
{
    using System;
    using System.Collections.Generic;

    public sealed class ActivityType
    {

        private readonly String name;
        private static readonly Dictionary<string, ActivityType> instance = new Dictionary<string, ActivityType>();


        // Activity
        public static readonly ActivityType jdbcCallActivityType = new ActivityType("com.tibco.plugin.jdbc.JDBCCallActivity");
        public static readonly ActivityType jdbcUpdateActivityType = new ActivityType("com.tibco.plugin.jdbc.JDBCUpdateActivity");
        public static readonly ActivityType jdbcQueryActivityType = new ActivityType("com.tibco.plugin.jdbc.JDBCQueryActivity");

        public static readonly ActivityType xmlParseActivityType = new ActivityType("com.tibco.plugin.xml.XMLParseActivity");

        public static readonly ActivityType mapperActivityType = new ActivityType("com.tibco.plugin.mapper.MapperActivity");

        public static readonly ActivityType callProcessActivityType = new ActivityType("com.tibco.pe.core.CallProcessActivity");
        public static readonly ActivityType assignActivityType = new ActivityType("com.tibco.pe.core.AssignActivity");
        public static readonly ActivityType writeToLogActivityType = new ActivityType("com.tibco.pe.core.WriteToLogActivity");
        public static readonly ActivityType generateErrorActivity = new ActivityType("com.tibco.pe.core.GenerateErrorActivity");
        public static readonly ActivityType setSharedVariableActivityType = new ActivityType("com.tibco.pe.core.SetSharedVariableActivity");
        public static readonly ActivityType getSharedVariableActivityType = new ActivityType("com.tibco.pe.core.GetSharedVariableActivity");
        public static readonly ActivityType loopGroupActivityType = new ActivityType("com.tibco.pe.core.LoopGroup");
        public static readonly ActivityType criticalSectionGroupActivityType = new ActivityType("com.tibco.pe.core.CriticalSectionGroup");

        public static readonly ActivityType javaActivityType = new ActivityType("com.tibco.plugin.java.JavaActivity");

        public static readonly ActivityType nullActivityType = new ActivityType("com.tibco.plugin.timer.NullActivity");
        public static readonly ActivityType sleepActivity = new ActivityType("com.tibco.plugin.timer.SleepActivity");

        public static readonly ActivityType rdvPubActivityType = new ActivityType("com.plugin.tibrv.RVPubActivity");

        public static readonly ActivityType ConfirmActivityType = new ActivityType("com.tibco.pe.core.ConfirmActivity");

        // Starter activity type
        public static readonly ActivityType RdvEventSourceActivityType = new ActivityType("com.plugin.tibrv.RVEventSource");

        public static readonly ActivityType TimerEventSource = new ActivityType("com.tibco.plugin.timer.TimerEventSource");
        
        public static readonly ActivityType OnStartupEventSource = new ActivityType("com.tibco.pe.core.OnStartupEventSource");

        public static readonly ActivityType AeSubscriberActivity = new ActivityType("com.tibco.plugin.ae.AESubscriberActivity");

        // Start
        public static readonly ActivityType startType = new ActivityType("startType");
        public static readonly ActivityType endType = new ActivityType("endType");

        public static readonly ActivityType NotHandleYet = new ActivityType("NotHandleYet");

        private ActivityType(String name)
        {
            this.name = name;
            instance[name] = this;
        }

        public override String ToString()
        {
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

