namespace EaiConverter.Model
{
	public class EngineCommandActivity : Activity
	{
        public EngineCommandActivity(string name, ActivityType type) : base(name, type)
        {
        }

        public EngineCommandActivity() 
        {
        }

        public string Command
        {
            get;
            set;
        }
	}


}

