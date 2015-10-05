namespace EaiConverter.Model
{
	public class SleepActivity : Activity
	{
        public SleepActivity(string name, ActivityType type) : base(name, type)
        {
        }

        public SleepActivity() 
        {
        }

        public int TimerDuration
        {
            get;
            set;
        }
	}


}

