namespace EaiConverter.Model
{
    public class RdvEventSourceActivity : Activity
	{

        public RdvEventSourceActivity (string name, ActivityType type) : base (name, type)
        {
        }

        public RdvEventSourceActivity () 
        {
        }

        public string Subject
        {
            get;
            set;
        }

        public string SharedChannel
        {
            get;
            set;
        }

        public string XsdString
        {
            get;
            set;
        }
	}

}

